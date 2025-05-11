using System.Diagnostics;
using generar_ticket.ticket.Interfaces.REST.Resources;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Path = System.IO.Path;

namespace generar_ticket.ticket.Domain.Services;

public class PrintService
{
    public async Task PrintTicketAsync(TicketResource ticket)
    {
        var pdfPath = Path.Combine(Path.GetTempPath(), "ticket_print.pdf");

        using (var writer = new PdfWriter(pdfPath))
        using (var pdf = new PdfDocument(writer))
        {
            var ticketWidth = 220f; 
            var ticketHeight = 250f; 

            var document = new Document(pdf, new PageSize(ticketWidth, ticketHeight));
            var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);

            var mainTable = new Table(UnitValue.CreatePercentArray(1)) // Set width to 100%
                .SetWidth(UnitValue.CreatePercentValue(100)) // Make the table use the full width
                .SetBorder(new SolidBorder(1)) // Add a border to the table
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            mainTable.AddCell(new Cell().Add(new Paragraph("MUNICIPALIDAD PROVINCIAL DE JAEN")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14)
                    .SetFont(font)
                    .SetBold())
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.CENTER));

            mainTable.AddCell(new Cell().Add(new Paragraph($"N°: {ticket.NumeroTicket}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(15)
                    .SetFont(font)
                    .SetBold())
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.CENTER));

            var contentTable = new Table(1)
                .SetWidth(ticketWidth - 11)
                .SetBorder(Border.NO_BORDER)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            contentTable.AddCell(new Cell().Add(new Paragraph($"AREA: {ticket.AreaNombre}")
                    .SetFontSize(11)
                    .SetFont(font)
                    .SetBold())
                .SetBorder(Border.NO_BORDER));

            contentTable.AddCell(new Cell().Add(new Paragraph($"FECHA: {ticket.Fecha:dd-MM-yyyy}")
                    .SetFontSize(11)
                    .SetFont(font))
                .SetBorder(Border.NO_BORDER));

            contentTable.AddCell(new Cell().Add(new Paragraph($"HORA: {ticket.Fecha:HH:mm}")
                    .SetFontSize(11)
                    .SetFont(font))
                .SetBorder(Border.NO_BORDER));

            mainTable.AddCell(new Cell().Add(contentTable).SetBorder(Border.NO_BORDER));

            document.Add(mainTable);
            document.Close();
        }
       

        // Print the PDF directly
        PrintPdf(pdfPath);
    }


    private void PrintPdf(string pdfPath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = pdfPath,
                UseShellExecute = true,
                Verb = "print" // Enviar directamente a la impresora predeterminada
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al imprimir el PDF: {ex.Message}");
        }

        
        
    }




}