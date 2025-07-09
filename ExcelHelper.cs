using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace TariffComparison.Helpers;

public static class ExcelHelper
{
    public static IActionResult ProcessExcelFromBlob(string sasUrl, out List<Dictionary<string, object>> rows)
    {
        rows = new List<Dictionary<string, object>>();

        var blobClient = new BlobClient(new Uri(sasUrl));

        if (!blobClient.Exists())
        {
            return new NotFoundObjectResult("Blob not found at the provided SAS URL.");
        }

        using var memoryStream = new MemoryStream();
        blobClient.DownloadTo(memoryStream);
        memoryStream.Position = 0;

        using var package = new ExcelPackage(memoryStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
        {
            return new BadRequestObjectResult("No worksheet found in the Excel file.");
        }

        var columnNames = new List<string>();

        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            columnNames.Add(worksheet.Cells[1, col].Text);
        }

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var rowData = new Dictionary<string, object>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                rowData[columnNames[col - 1]] = worksheet.Cells[row, col].Text;
            }
            rows.Add(rowData);
        }

        return null; // Indicates success
    }
}
