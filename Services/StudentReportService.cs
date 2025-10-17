using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VaxSync.Web.Data;
using Microsoft.EntityFrameworkCore;

// iText7
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace VaxSync.Web.Services;

public class StudentReportService
{
    private readonly ApplicationDbContext _db;

    public StudentReportService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<byte[]> GenerateAsync(string studentId)
    {
        var s = await _db.Students
            .Include(x => x.Vaccines)
                .ThenInclude(vx => vx.Vaccine)
            .FirstOrDefaultAsync(x => x.Id == studentId);

        if (s is null)
            throw new InvalidOperationException("Student not found.");

        using var ms = new MemoryStream();
        using var writer = new PdfWriter(ms);
        using var pdf = new PdfDocument(writer);
        var doc = new Document(pdf);

        // fonts
        var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        // title
        doc.Add(new Paragraph("VaxSync Student Report")
            .SetFont(fontBold)
            .SetFontSize(18));

        doc.Add(new Paragraph($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}")
            .SetFont(fontRegular));
        doc.Add(new Paragraph(" "));

        // student header
        doc.Add(new Paragraph($"Student: {s.FirstName} {s.LastName}").SetFont(fontRegular));
        doc.Add(new Paragraph($"Student ID: {s.Id}").SetFont(fontRegular));
        doc.Add(new Paragraph($"School ID: {s.SchoolId}").SetFont(fontRegular));
        doc.Add(new Paragraph($"Date of Birth: {s.DateOfBirth:yyyy-MM-dd}").SetFont(fontRegular));
        doc.Add(new Paragraph(" "));

        // table header
        doc.Add(new Paragraph("Vaccination Records").SetFont(fontBold));

        if (s.Vaccines.Any())
        {
            var table = new Table(new float[] { 4, 2, 3 }).UseAllAvailableWidth();
            table.AddHeaderCell(new Cell().Add(new Paragraph("Vaccine").SetFont(fontBold)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Dose").SetFont(fontBold)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Date Given").SetFont(fontBold)));

            foreach (var v in s.Vaccines)
            {
                table.AddCell(new Paragraph(v.Vaccine?.Name ?? "(Unknown)").SetFont(fontRegular));
                table.AddCell(new Paragraph(v.DoseNumber.ToString()).SetFont(fontRegular));
                table.AddCell(new Paragraph(v.DateGiven?.ToString("yyyy-MM-dd") ?? "Pending").SetFont(fontRegular));
            }

            doc.Add(table);
        }
        else
        {
            doc.Add(new Paragraph("No vaccine records found.").SetFont(fontRegular));
        }

        doc.Close();
        return ms.ToArray();
    }
}
