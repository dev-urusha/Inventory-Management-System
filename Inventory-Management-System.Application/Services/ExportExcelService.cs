using Inventory_Management_System.Application.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Inventory_Management_System.Application.Services
{
    public class ExportExcelService: IExportExcelService
    {
        public byte[] CreateFile<T>(List<T> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("No data available for export.");

            using var workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Products");

            var properties = typeof(T).GetProperties();
            var headers = properties.Select(p => p.GetCustomAttribute<DisplayAttribute>()?.Name ?? p.Name).ToArray();

            // Create Bold Header Style
            ICellStyle boldStyle = workbook.CreateCellStyle();
            IFont boldFont = workbook.CreateFont();
            boldFont.IsBold = true; 
            boldStyle.SetFont(boldFont);

            // Create Header Row
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < headers.Length; i++)
            {
                ICell cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]); 
                cell.CellStyle = boldStyle;   
            }

            // Insert Data Rows
            for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                IRow row = sheet.CreateRow(rowIndex + 1);
                var item = data[rowIndex];

                for (int colIndex = 0; colIndex < properties.Length; colIndex++)
                {
                    var value = properties[colIndex].GetValue(item)?.ToString();
                    row.CreateCell(colIndex).SetCellValue(value ?? "");
                }
            }

            // Auto-size columns for better readability
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            //content start
            var rowNum = 1;
            foreach (var item in data)
            {
                var rowContent = sheet.CreateRow(rowNum);

                var colContentIndex = 0;
                foreach (var property in properties)
                {
                    var cellContent = rowContent.CreateCell(colContentIndex);
                    var value = property.GetValue(item, null);

                    if (value == null)
                    {
                        cellContent.SetCellValue("");
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        cellContent.SetCellValue(value.ToString());
                    }
                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        cellContent.SetCellValue(Convert.ToInt32(value));
                    }
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                    {
                        cellContent.SetCellValue(Convert.ToDouble(value));
                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        var dateValue = (DateTime)value;
                        cellContent.SetCellValue(dateValue.ToString("yyyy-MM-dd"));
                    }
                    else cellContent.SetCellValue(value.ToString());

                    colContentIndex++;
                }

                rowNum++;
            }
            //content end

            var stream = new MemoryStream();
            workbook.Write(stream);
            return stream.ToArray();
        }
    }
}
