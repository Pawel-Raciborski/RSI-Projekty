using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using System;
using static System.Net.Mime.MediaTypeNames;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Rezerwacjabilet.Helpers
{
    public static class PDFGenerator
    {
        public static byte[] GenerateReservationPdf(string tytul, string data, string godzina, string miejsca)
        {
            if (string.IsNullOrEmpty(tytul)) tytul = "Nieznany film";
            if (string.IsNullOrEmpty(data)) data = "Nieznana data";
            if (string.IsNullOrEmpty(godzina)) godzina = "Nieznana godzina";
            if (string.IsNullOrEmpty(miejsca)) miejsca = "Brak wybranych miejsc";

            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);

                gfx.DrawString("Potwierdzenie Rezerwacji", font, XBrushes.Black, new XPoint(20, 20));
                gfx.DrawString($"Film: {tytul}", font, XBrushes.Black, new XPoint(20, 40));
                gfx.DrawString($"Data: {data}", font, XBrushes.Black, new XPoint(20, 60));
                gfx.DrawString($"Godzina: {godzina}", font, XBrushes.Black, new XPoint(20, 80));
                gfx.DrawString($"Zarezerwowane miejsce: {miejsca}", font, XBrushes.Black, new XPoint(20, 100));
                gfx.DrawString($"Data rezerwacji: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", font, XBrushes.Black, new XPoint(20, 120));

                using (var stream = new MemoryStream())
                {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}


