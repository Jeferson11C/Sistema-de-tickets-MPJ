using System.Collections.Generic;
using System.Drawing.Printing;

public class PrinterInfo
{
    public string Id { get; set; }  // El nombre de la impresora como ID válido
    public string Name { get; set; }
}

public class PrinterService
{
    public static List<PrinterInfo> GetInstalledPrinters()
    {
        var printers = new List<PrinterInfo>();

        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printers.Add(new PrinterInfo
            {
                Id = printer,  // Usar el nombre real como identificador
                Name = printer
            });
        }
        return printers;
    }

    public static PrinterInfo GetPrinterById(string id)
    {
        var printers = GetInstalledPrinters();
        return printers.Find(p => p.Id == id);
    }
}