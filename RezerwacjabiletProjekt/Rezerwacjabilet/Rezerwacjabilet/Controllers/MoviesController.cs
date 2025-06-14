using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Linq;
using Rezerwacjabilet.Helpers;
using Rezerwacjabilet.Models;
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        session.SetString(key, json);
    }

    public static T? Get<T>(this ISession session, string key)
    {
        var json = session.GetString(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

}

namespace Rezerwacjabilet.Controllers
{
    public class MoviesController : Controller
    {
        private readonly HelloWorldImplClient _client;

        public MoviesController(HelloWorldImplClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var movies = HttpContext.Session.Get<List<Film>>("MoviesList");

            if (movies == null)
            {
                var response = await _client.getMoviesAsync();
                movies = response.@return.ToList();
                HttpContext.Session.Set("MoviesList", movies);
            }

            return View(movies.ToArray());
        }



        public async Task<IActionResult> Reserve(string filmId)
        {
            var movies = HttpContext.Session.Get<List<Film>>("MoviesList");
            var movie = movies?.FirstOrDefault(m => m.tytul == filmId);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost]
        public IActionResult ConfirmReservation(string filmId, string[] selectedSeats)
        {
            var movies = HttpContext.Session.Get<List<Film>>("MoviesList");
            if (movies == null)
                return RedirectToAction("Index");

            var movie = movies.FirstOrDefault(m => m.tytul == filmId);
            if (movie != null)
            {
                var miejscaList = movie.miejsca.ToList();

                var unavailable = selectedSeats.Except(miejscaList).ToList();
                if (unavailable.Any())
                {
                    TempData["ReservationMessage"] = $"Niektóre miejsca są już zarezerwowane: {string.Join(", ", unavailable)}";
                    return RedirectToAction("Index");
                }

                if (!miejscaList.Any())
                {
                    TempData["ReservationMessage"] = "Brak dostępnych miejsc.";
                    return RedirectToAction("Index");
                }

                foreach (var seat in selectedSeats)
                {
                    miejscaList.Remove(seat);
                }

                movie.miejsca = miejscaList.ToArray();
                HttpContext.Session.Set("MoviesList", movies);

                var userReservations = HttpContext.Session.Get<List<UserReservation>>("UserReservations") ?? new List<UserReservation>();

                userReservations.Add(new UserReservation
                {
                    Tytul = movie.tytul,
                    Data = movie.data,
                    Godzina = movie.godzina,
                    Miejsca = selectedSeats
                });

                HttpContext.Session.Set("UserReservations", userReservations);


                var pdfBytes = PDFGenerator.GenerateReservationPdf(movie.tytul, movie.data, movie.godzina, string.Join(", ", selectedSeats));
                var fileName = $"Rezerwacja_{movie.tytul}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                TempData["ReservationMessage"] = $"Rezerwacja została dokonana na film: {movie.tytul}.";
                Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);

                return File(pdfBytes, "application/pdf");
            }

            TempData["ReservationMessage"] = "Film nie został znaleziony.";
            return RedirectToAction("Index");
        }


        public IActionResult MyReservations()
        {
            var reservations = HttpContext.Session.Get<List<UserReservation>>("UserReservations") ?? new List<UserReservation>();
            return View(reservations);
        }

        public IActionResult CancelReservation(string filmId)
        {
            var userReservations = HttpContext.Session.Get<List<UserReservation>>("UserReservations") ?? new List<UserReservation>();
            var reservation = userReservations.FirstOrDefault(r => r.Tytul == filmId);

            if (reservation != null)
            {
                userReservations.Remove(reservation);

                var movies = HttpContext.Session.Get<List<Film>>("MoviesList");
                var movie = movies?.FirstOrDefault(m => m.tytul == filmId);
                if (movie != null)
                {
                    var miejsca = movie.miejsca.ToList();
                    miejsca.AddRange(reservation.Miejsca);
                    movie.miejsca = miejsca.Distinct().ToArray();
                    HttpContext.Session.Set("MoviesList", movies);
                }

                HttpContext.Session.Set("UserReservations", userReservations);
                TempData["ReservationMessage"] = $"Rezerwacja na film {filmId} została anulowana.";
            }
            else
            {
                TempData["ReservationMessage"] = "Rezerwacja nie została znaleziona.";
            }

            return RedirectToAction("MyReservations");
        }

        public IActionResult ModifyReservation(string filmId)
        {
            var userReservations = HttpContext.Session.Get<List<UserReservation>>("UserReservations");
            var reservation = userReservations?.FirstOrDefault(r => r.Tytul == filmId);

            if (reservation == null)
                return NotFound();

            var movies = HttpContext.Session.Get<List<Film>>("MoviesList");
            var movie = movies?.FirstOrDefault(m => m.tytul == filmId);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost]
        public IActionResult UpdateReservation(string filmId, string[] selectedSeats)
        {
            var movies = HttpContext.Session.Get<List<Film>>("MoviesList");
            var movie = movies?.FirstOrDefault(m => m.tytul == filmId);
            if (movie == null)
                return NotFound();

            var userReservations = HttpContext.Session.Get<List<UserReservation>>("UserReservations") ?? new List<UserReservation>();
            var reservation = userReservations.FirstOrDefault(r => r.Tytul == filmId);

            if (reservation == null)
                return NotFound();

            var miejscaList = movie.miejsca.ToList();
            miejscaList.AddRange(reservation.Miejsca);

            var unavailable = selectedSeats.Except(miejscaList).ToList();
            if (unavailable.Any())
            {
                TempData["ReservationMessage"] = $"Niektóre miejsca są już zajęte: {string.Join(", ", unavailable)}";
                return RedirectToAction("MyReservations");
            }

            foreach (var seat in selectedSeats)
            {
                miejscaList.Remove(seat);
            }

            reservation.Miejsca = selectedSeats;
            movie.miejsca = miejscaList.ToArray();

            HttpContext.Session.Set("MoviesList", movies);
            HttpContext.Session.Set("UserReservations", userReservations);

            TempData["ReservationMessage"] = $"Rezerwacja dla filmu {filmId} została zmodyfikowana.";

            return RedirectToAction("MyReservations");
        }
    }
}
