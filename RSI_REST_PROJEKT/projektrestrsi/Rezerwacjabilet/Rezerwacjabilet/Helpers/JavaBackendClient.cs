using System.Text.Json;
using Rezerwacjabilet.Models;

namespace Rezerwacjabilet.Helpers
{
    public class JavaBackendClient
    {
        private readonly HttpClient _httpClient;
        

        public JavaBackendClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://192.168.43.89:8181/RezerwacjabiletService/service/");

        }

        public async Task<IEnumerable<FilmViewModel>> GetMoviesAsync()
        {
            var response = await _httpClient.GetAsync("getFilmy");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<FilmViewModel>>(json);
        }
        public async Task<string> MakeReservationAsync(string userId, string filmTitle, string[] seats)
        {
            var url = $"makeReservation?userId={userId}&filmTitle={Uri.EscapeDataString(filmTitle)}";
            var content = JsonContent.Create(seats);
            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IEnumerable<UserReservation>> GetUserReservationsAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"getUserReservations?userId={userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<UserReservation>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<string> CancelReservationAsync(string userId, string filmTitle)
        {
            var url = $"cancelReservation?userId={userId}&filmTitle={Uri.EscapeDataString(filmTitle)}";
            // wysyłamy POST z pustą treścią (lub można też wysłać null content)
            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ModifyReservationAsync(string userId, string filmTitle, string[] newSeats)
        {
            var url = $"modifyReservation?userId={userId}&filmTitle={Uri.EscapeDataString(filmTitle)}";
            var content = JsonContent.Create(newSeats);
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }

}
