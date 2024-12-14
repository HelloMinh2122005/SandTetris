using CommunityToolkit.Maui.Storage;
using SandTetris.Entities;
using SandTetris.Interfaces;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace SandTetris.Services;

public class ExportFilePDF
{
    private readonly ISalaryDetailRepository _iSalaryRepo;
    private readonly IDepartmentRepository _iDepartmentRepo;

    public ExportFilePDF(ISalaryDetailRepository iSalaryRepo, IDepartmentRepository departmentRepository)
    {
        _iSalaryRepo = iSalaryRepo;
        _iDepartmentRepo = departmentRepository;
    }

    public async Task ExportPDF(string departmentID, int month, int year)
    {
        IEnumerable<SalaryDetail> listSalariesDepartment = [];

        listSalariesDepartment = await _iSalaryRepo.GetSalaryDetailsForDepartmentAsync(departmentID, month, year);

        long TotalMoneySpent = 0;
        foreach (var item in listSalariesDepartment)
            TotalMoneySpent += item.FinalSalary;

        if (_iDepartmentRepo == null)
        {
            await Shell.Current.DisplayAlert("Error", "No department repository found", "OK");
            return;
        }
        if (departmentID == null)
        {
            await Shell.Current.DisplayAlert("Error", "No department ID found", "OK");
            return;
        }

        Department department = await _iDepartmentRepo.GetDepartmentByIdAsync(departmentID);
        if (department == null)
        {
            await Shell.Current.DisplayAlert("Error", "No department found", "OK");
            return;
        }

        // Pick a folder to store the file        
        var result = await FolderPicker.Default.PickAsync();

        if (result != null && result.Folder != null && !string.IsNullOrEmpty(result.Folder.Path))
        {
            string folderPath = result.Folder.Path;

            // 1) Create the PDF Document
            using PdfDocument document = new PdfDocument();
            document.Info.Title = "Salary Details PDF";

            // 2) Add a page
            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4; 
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Prepare fonts
            XFont titleFont = new XFont("Arial", 14, XFontStyle.Bold);
            XFont headerFont = new XFont("Arial", 8, XFontStyle.Bold);
            XFont normalFont = new XFont("Arial", 8, XFontStyle.Regular);

            double marginLeft = 40;
            double marginTop = 40;
            double lineHeight = 20;
            double currentY = marginTop;

            string title = "Expenditure Report for Department";
            gfx.DrawString(title, titleFont, XBrushes.Black, new XRect(marginLeft, currentY, page.Width - marginLeft * 2, lineHeight),
                           XStringFormats.Center);
            currentY += lineHeight + 10; // extra spacing after title

            // 4) Department info
            gfx.DrawString("Department ID:", headerFont, XBrushes.Black, new XRect(marginLeft, currentY, 100, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString(department.Id, normalFont, XBrushes.Black, new XRect(marginLeft + 110, currentY, 200, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight;

            gfx.DrawString("Department Name:", headerFont, XBrushes.Black, new XRect(marginLeft, currentY, 110, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString(department.Name, normalFont, XBrushes.Black, new XRect(marginLeft + 110, currentY, 200, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight;

            gfx.DrawString("Month/Year:", headerFont, XBrushes.Black, new XRect(marginLeft, currentY, 110, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString($"{month}/{year}", normalFont, XBrushes.Black, new XRect(marginLeft + 110, currentY, 200, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight;

            gfx.DrawString("Total Spent:", headerFont, XBrushes.Black, new XRect(marginLeft, currentY, 110, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString(TotalMoneySpent.ToString(), normalFont, XBrushes.Black, new XRect(marginLeft + 110, currentY, 200, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight + 10; // spacing before table

            // 5) Draw table headers
            // Let's define some column widths for a 7-column layout
            double[] columnWidths = { 50, 120, 80, 50, 50, 60, 80 };
            string[] headerNames = { "ID", "Employee Name", "Base Salary", "Workings", "Absents", "On Leaves", "Final Salary" };

            double tableX = marginLeft;
            double headerY = currentY;

            // Draw header background rectangles & text
            XBrush headerBrush = new XSolidBrush(XColors.LightGray);
            double rowHeight = 18;

            for (int col = 0; col < headerNames.Length; col++)
            {
                gfx.DrawRectangle(headerBrush, tableX, headerY, columnWidths[col], rowHeight);
                gfx.DrawRectangle(XPens.Black, tableX, headerY, columnWidths[col], rowHeight); // border
                gfx.DrawString(headerNames[col], headerFont, XBrushes.Black,
                               new XRect(tableX + 2, headerY + 2, columnWidths[col], rowHeight),
                               XStringFormats.TopLeft);
                tableX += columnWidths[col];
            }

            currentY += rowHeight; // after drawing header row
            tableX = marginLeft;   // reset for row data

            // 6) Draw each salary detail row
            foreach (var item in listSalariesDepartment)
            {
                int dayInMonth = DateTime.DaysInMonth(year, month);

                string[] rowData = new string[]
                {
                        item.EmployeeId,
                        item.Employee?.FullName ?? "",
                        item.BaseSalary.ToString(),
                        (dayInMonth - item.DaysAbsent - item.DaysOnLeave).ToString(),
                        item.DaysAbsent.ToString(),
                        item.DaysOnLeave.ToString(),
                        item.FinalSalary.ToString()
                };

                tableX = marginLeft;
                for (int col = 0; col < rowData.Length; col++)
                {
                    // draw border
                    gfx.DrawRectangle(XPens.Black, tableX, currentY, columnWidths[col], rowHeight);
                    // draw text
                    gfx.DrawString(rowData[col], normalFont, XBrushes.Black,
                                   new XRect(tableX + 2, currentY + 2, columnWidths[col], rowHeight),
                                   XStringFormats.TopLeft);
                    tableX += columnWidths[col];
                }
                currentY += rowHeight;
            }

            // 7) Save PDF to MemoryStream
            using var stream = new MemoryStream();
            document.Save(stream, false);  // false = do not close stream
            stream.Seek(0, SeekOrigin.Begin);

            // 8) Determine file name & write the final PDF
            int fileIndex = 0;
            string baseFileName = "Salary Details for department.pdf";
            string possibleFilePath = Path.Combine(folderPath, baseFileName);

            while (File.Exists(possibleFilePath))
            {
                fileIndex++;
                var testFileName = $"Salary Details for department ({fileIndex}).pdf";
                possibleFilePath = Path.Combine(folderPath, testFileName);
            }

            using var fileStream = new FileStream(possibleFilePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);

            await Shell.Current.DisplayAlert("Notification", "File exported successfully", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No valid folder selected.", "OK");
        }
    }

    public async Task ExportPDF(int month, int year)
    {
        IEnumerable<SalaryDetailSummary> listSalarySummaries = await _iSalaryRepo.GetSalaryDetailSummariesAsync(month, year);

        long TotalMoneySpent = listSalarySummaries.Sum(x => x.TotalSpent);

        // Pick a folder to store the file        
        var result = await FolderPicker.Default.PickAsync();

        if (result != null && result.Folder != null && !string.IsNullOrEmpty(result.Folder.Path))
        {
            string folderPath = result.Folder.Path;

            // 1) Create PDF document
            using PdfDocument document = new PdfDocument();
            document.Info.Title = "Expenditure Details PDF";

            // 2) Add page
            PdfPage page = document.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // 3) Fonts
            XFont titleFont = new XFont("Arial", 14, XFontStyle.Bold);
            XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);
            XFont normalFont = new XFont("Arial", 10, XFontStyle.Regular);

            double marginLeft = 40;
            double marginTop = 40;
            double lineHeight = 20;
            double currentY = marginTop;

            // 4) Title
            string title = $"Salary Expenditure Report for {month}/{year}";
            gfx.DrawString(title, titleFont, XBrushes.Black,
                           new XRect(marginLeft, currentY, page.Width - marginLeft * 2, lineHeight),
                           XStringFormats.Center);
            currentY += lineHeight + 10;

            // 5) Summary information
            gfx.DrawString("Number of departments:", headerFont, XBrushes.Black,
                           new XRect(marginLeft, currentY, 200, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString(listSalarySummaries.Count().ToString(), normalFont, XBrushes.Black,
                           new XRect(marginLeft + 200, currentY, 100, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight;

            gfx.DrawString("Month/Year:", headerFont, XBrushes.Black,
                           new XRect(marginLeft, currentY, 200, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString($"{month}/{year}", normalFont, XBrushes.Black,
                           new XRect(marginLeft + 200, currentY, 100, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight;

            gfx.DrawString("Total Expenditure:", headerFont, XBrushes.Black,
                           new XRect(marginLeft, currentY, 200, lineHeight), XStringFormats.TopLeft);
            gfx.DrawString(TotalMoneySpent.ToString(), normalFont, XBrushes.Black,
                           new XRect(marginLeft + 200, currentY, 100, lineHeight), XStringFormats.TopLeft);
            currentY += lineHeight + 10;

            // 6) Table headers
            string[] headers = { "ID", "Name", "Head name", "Employees", "Total spent" };
            double[] colWidths = { 80, 120, 120, 80, 80 };
            double tableX = marginLeft;
            double headerY = currentY;
            double rowHeight = 20;

            // Gray header background
            XBrush headerBrush = new XSolidBrush(XColors.LightGray);
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(headerBrush, tableX, headerY, colWidths[i], rowHeight);
                gfx.DrawRectangle(XPens.Black, tableX, headerY, colWidths[i], rowHeight);
                gfx.DrawString(headers[i], headerFont, XBrushes.Black,
                               new XRect(tableX + 5, headerY + 3, colWidths[i], rowHeight),
                               XStringFormats.TopLeft);
                tableX += colWidths[i];
            }

            currentY += rowHeight;

            // 7) Each department row
            foreach (var item in listSalarySummaries)
            {
                var dep = await _iDepartmentRepo.GetDepartmentByIdAsync(item.DepartmentId);
                if (dep == null)
                    continue; // or handle error

                string headName = dep.HeadOfDepartment?.FullName ?? "None";
                int employeeCount = dep.Employees?.Count ?? 0;

                string[] rowData = new string[]
                {
                item.DepartmentId,
                item.DepartmentName,
                headName,
                employeeCount.ToString(),
                item.TotalSpent.ToString()
                };

                tableX = marginLeft;
                for (int col = 0; col < rowData.Length; col++)
                {
                    // draw cell border
                    gfx.DrawRectangle(XPens.Black, tableX, currentY, colWidths[col], rowHeight);
                    // draw text
                    gfx.DrawString(rowData[col], normalFont, XBrushes.Black,
                                   new XRect(tableX + 5, currentY + 3, colWidths[col], rowHeight),
                                   XStringFormats.TopLeft);

                    tableX += colWidths[col];
                }
                currentY += rowHeight;
            }

            // 8) Save document to MemoryStream
            using var stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);

            // 9) Check file name collisions & save to disk
            int fileIndex = 0;
            string baseFileName = "Expenditure Details.pdf";
            string possibleFilePath = Path.Combine(folderPath, baseFileName);

            while (File.Exists(possibleFilePath))
            {
                fileIndex++;
                var testFileName = $"Expenditure Details ({fileIndex}).pdf";
                possibleFilePath = Path.Combine(folderPath, testFileName);
            }

            using var fileStream = new FileStream(possibleFilePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);

            await Shell.Current.DisplayAlert("Notification", "File exported successfully", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No valid folder selected.", "OK");
        }
    }


}
