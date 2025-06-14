namespace Rezerwacjabilet.Models
{
    public class FilmViewModel
    {
        public string tytul { get; set; }
        public string data { get; set; }
        public string godzina { get; set; }
        public string[] miejsca { get; set; }
        public byte[] zdjecie { get; set; }
    }

}
