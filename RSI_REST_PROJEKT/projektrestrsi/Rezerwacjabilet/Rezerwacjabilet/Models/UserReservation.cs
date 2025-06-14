using System.Text.Json.Serialization;

namespace Rezerwacjabilet.Models
{

    public class UserReservation
    {
        [JsonPropertyName("filmTitle")]
        public string Tytul { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("godzina")]
        public string Godzina { get; set; }

        [JsonPropertyName("miejsca")]
        public string[] Miejsca { get; set; }
    }


}
