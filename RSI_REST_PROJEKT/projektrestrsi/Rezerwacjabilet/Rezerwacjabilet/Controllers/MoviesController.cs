using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Rezerwacjabilet.Helpers;
using Rezerwacjabilet.Models;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rezerwacjabilet.Controllers
{
    public class MoviesController : Controller
    {
        private readonly JavaBackendClient _client;

        public MoviesController(JavaBackendClient client)
        {
            _client = client;
        }

        private string GetUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                userId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("UserId", userId);
            }
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _client.GetMoviesAsync();
            var model = movies.Select(f => new FilmViewModel
            {
                tytul = f.tytul,
                data = f.data,
                godzina = f.godzina,
                miejsca = f.miejsca,
                zdjecie = f.zdjecie
            }).ToArray();
            return View(model);
        }

        public async Task<IActionResult> Reserve(string filmId)
        {
            var movies = await _client.GetMoviesAsync();
            var movie = movies.FirstOrDefault(m => m.tytul == filmId);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmReservation(string filmId, string[] selectedSeats)
        {
            if (string.IsNullOrEmpty(filmId) || selectedSeats == null || selectedSeats.Length == 0)
            {
                TempData["ReservationMessage"] = "Nie wybrano filmu lub miejsc.";
                return RedirectToAction("Index");
            }

            var userId = GetUserId();
            var result = await _client.MakeReservationAsync(userId, filmId, selectedSeats);

            if (result == "Reservation successful.")
            {
                var movies = await _client.GetMoviesAsync();
                var movie = movies.FirstOrDefault(m => m.tytul == filmId);
                var miejscaString = string.Join(", ", selectedSeats);

                var pdfBytes = PDFGenerator.GenerateReservationPdf(
                    tytul: filmId,
                    data: movie?.data ?? "Nieznana data",
                    godzina: movie?.godzina ?? "Nieznana godzina",
                    miejsca: miejscaString
                );

                return File(pdfBytes, "application/pdf", $"PotwierdzenieRezerwacji_{filmId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            else
            {
                TempData["ReservationMessage"] = $"Błąd rezerwacji: {result}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> MyReservations()
        {
            var userId = GetUserId();
            var reservations = await _client.GetUserReservationsAsync(userId);

            var model = reservations.Select(r => new UserReservation
            {
                Tytul = r.Tytul,
                Data = r.Data,
                Godzina = r.Godzina,
                Miejsca = r.Miejsca
            }).ToList();
            foreach (var r in reservations)
            {
                Console.WriteLine($"Film: {r.Tytul}, Miejsca: {(r.Miejsca == null ? "NULL" : string.Join(",", r.Miejsca))}");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(string filmTitle)
        {
            var userId = GetUserId();
            var result = await _client.CancelReservationAsync(userId, filmTitle);

            if (result == "Reservation cancelled.")
            {
                TempData["ReservationMessage"] = $"Rezerwacja na film {filmTitle} została anulowana.";
            }
            else
            {
                TempData["ReservationMessage"] = "Rezerwacja nie została znaleziona.";
            }

            return RedirectToAction("MyReservations"); // lepiej wrócić do listy rezerwacji
        }


        public async Task<IActionResult> ModifyReservation(string filmId)
        {
            var userId = GetUserId();
            var reservations = await _client.GetUserReservationsAsync(userId);
            var userReservation = reservations.FirstOrDefault(r => r.Tytul == filmId);
            if (userReservation == null)
                return NotFound();

            var movies = await _client.GetMoviesAsync();
            var movie = movies.FirstOrDefault(m => m.tytul == filmId);
            if (movie == null)
                return NotFound();

            var model = new ModifyReservationViewModel
            {
                FilmId = filmId,
                FilmTitle = movie.tytul,
                AvailableSeats = movie.miejsca,
                ReservedSeats = userReservation.Miejsca
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyReservation(ModifyReservationViewModel model)
        {
            var userId = GetUserId();

            if (model.SelectedSeats == null || model.SelectedSeats.Length == 0)
            {
                ModelState.AddModelError("", "Musisz wybrać przynajmniej jedno miejsce.");
                var movies = await _client.GetMoviesAsync();
                var movie = movies.FirstOrDefault(m => m.tytul == model.FilmId);
                model.AvailableSeats = movie?.miejsca ?? new string[] { };
                var reservations = await _client.GetUserReservationsAsync(userId);
                var res = reservations.FirstOrDefault(r => r.Tytul == model.FilmId);
                model.ReservedSeats = res?.Miejsca ?? new string[] { };
                return View(model);
            }

            var result = await _client.ModifyReservationAsync(userId, model.FilmId, model.SelectedSeats);

            if (result == "Reservation modified.")
            {
                TempData["ReservationMessage"] = $"Rezerwacja dla filmu {model.FilmTitle} została zmodyfikowana.";
                return RedirectToAction("MyReservations");
            }
            else
            {
                ModelState.AddModelError("", result);
                var movies = await _client.GetMoviesAsync();
                var movie = movies.FirstOrDefault(m => m.tytul == model.FilmId);
                model.AvailableSeats = movie?.miejsca ?? new string[] { };
                var reservations = await _client.GetUserReservationsAsync(userId);
                var res = reservations.FirstOrDefault(r => r.Tytul == model.FilmId);
                model.ReservedSeats = res?.Miejsca ?? new string[] { };
                return View(model);
            }
        }
    }
}