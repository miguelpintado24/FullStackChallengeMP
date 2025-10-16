using CsvHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TvShowTracker.Api.Data;

namespace TvShowTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ExportController(AppDbContext context) => _context = context;

        [HttpGet("csv")]
        public IActionResult ExportCsv()
        {
            var shows = _context.TvShows.ToList();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(shows);
            writer.Flush();
            return File(memoryStream.ToArray(), "text/csv", "tvshows.csv");
        }

        [HttpGet("pdf")]
        public IActionResult ExportPdf()
        {
            var shows = _context.TvShows.ToList();
            using var memoryStream = new MemoryStream();
            var document = new Document();
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            document.Add(new Paragraph("TV Shows List"));
            foreach (var show in shows)
            {
                document.Add(new Paragraph($"- {show.Title} ({show.Genre})"));
            }
            document.Close();

            return File(memoryStream.ToArray(), "application/pdf", "tvshows.pdf");
        }
    }
}
