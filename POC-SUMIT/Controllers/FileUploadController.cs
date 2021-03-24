using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC_SUMIT.Helper;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            List<string> xs = new List<string>();
            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = @"d:\" + formFile.FileName.ToString();
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                  
                    UpdateExcelUsingOpenXMLSDK(filePath);
                }
            }
            return Ok(new { count = files.Count, size, filePaths });
        }

        public static void UpdateExcelUsingOpenXMLSDK(string fileName)
        {
            ExcelHelper help = new ExcelHelper();
    
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(fileName, true))
            {
                // Access the main Workbook part, which contains all references.
                WorkbookPart workbookPart = spreadSheet.WorkbookPart;

                // get sheet by name
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == "Feuil1").FirstOrDefault();

                // get worksheetpart by sheet id
                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart;

                // The SheetData object will contain all the data.
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                Cell cell = help.GetCell(worksheetPart.Worksheet, "A", 1);

                cell.CellValue = new CellValue("Test");
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);

                // Save the worksheet.
                worksheetPart.Worksheet.Save();
            }
        }




    }

}

