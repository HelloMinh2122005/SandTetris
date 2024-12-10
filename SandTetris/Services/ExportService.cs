using Aspose.Cells;
using CommunityToolkit.Maui.Storage;
using SandTetris.Entities;
using SandTetris.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Folder is valid, proceed with exporting to PDF
            string folderPath = result.Folder.Path;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            worksheet.Cells[0, 0].PutValue($"Expenditure Report for Department");
            var titleStyle = workbook.CreateStyle();
            titleStyle.Font.IsBold = true;
            titleStyle.Font.Size = 14;
            titleStyle.HorizontalAlignment = TextAlignmentType.Center;
            worksheet.Cells[0, 0].SetStyle(titleStyle);
            worksheet.Cells.Merge(0, 0, 1, 7);  // Merge title cell across all columns

            // Set up worksheet columns and headers
            worksheet.Cells.SetRowHeight(0, 20);
            worksheet.Cells.SetColumnWidth(0, 5);
            worksheet.Cells.SetColumnWidth(1, 12);
            worksheet.Cells.SetColumnWidth(2, 9);
            worksheet.Cells.SetColumnWidth(3, 8);
            worksheet.Cells.SetColumnWidth(4, 8);
            worksheet.Cells.SetColumnWidth(5, 8);
            worksheet.Cells.SetColumnWidth(6, 9);

            worksheet.Cells[1, 0].PutValue("Department ID:");
            worksheet.Cells.Merge(1, 0, 1, 1);
            worksheet.Cells[1, 2].PutValue(department.Id);
            worksheet.Cells[2, 0].PutValue("Department Name:");
            worksheet.Cells.Merge(2, 0, 1, 1);
            worksheet.Cells[2, 2].PutValue(department.Name);
            worksheet.Cells[3, 0].PutValue("Month/Year:");
            worksheet.Cells.Merge(3, 0, 1, 1);
            worksheet.Cells[3, 2].PutValue($"{month}/{year}");
            worksheet.Cells[4, 0].PutValue("Total Spent:");
            worksheet.Cells.Merge(4, 0, 1, 1);
            worksheet.Cells[4, 2].PutValue(TotalMoneySpent.ToString());

            worksheet.Cells[6, 0].PutValue("ID");
            worksheet.Cells[6, 1].PutValue("Employee Name");
            worksheet.Cells[6, 2].PutValue("Base Salary");
            worksheet.Cells[6, 3].PutValue("Workings");
            worksheet.Cells[6, 4].PutValue("Absents");
            worksheet.Cells[6, 5].PutValue("On Leaves");
            worksheet.Cells[6, 6].PutValue("Final Salary");

            int rowindex = 7;

            foreach (var item in listSalariesDepartment)
            {
                int DayInMonth = DateTime.DaysInMonth(year, month);

                worksheet.Cells[rowindex, 0].PutValue(item.EmployeeId);
                worksheet.Cells[rowindex, 1].PutValue(item.Employee.FullName);
                worksheet.Cells[rowindex, 2].PutValue(item.BaseSalary.ToString());
                worksheet.Cells[rowindex, 3].PutValue((DayInMonth - item.DaysAbsent - item.DaysOnLeave).ToString());
                worksheet.Cells[rowindex, 4].PutValue(item.DaysAbsent.ToString());
                worksheet.Cells[rowindex, 5].PutValue(item.DaysOnLeave.ToString());
                worksheet.Cells[rowindex, 6].PutValue(item.FinalSalary.ToString());

                rowindex++;
            }

            var headerStyle = workbook.CreateStyle();
            headerStyle.Font.IsBold = true;
            headerStyle.Font.Size = 8;
            headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;

            worksheet.Cells[6, 0].SetStyle(headerStyle);
            worksheet.Cells[6, 1].SetStyle(headerStyle);
            worksheet.Cells[6, 2].SetStyle(headerStyle);
            worksheet.Cells[6, 3].SetStyle(headerStyle);
            worksheet.Cells[6, 4].SetStyle(headerStyle);
            worksheet.Cells[6, 5].SetStyle(headerStyle);
            worksheet.Cells[6, 6].SetStyle(headerStyle);

            var borderStyle = workbook.CreateStyle();
            borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

            for (int i = 7; i < rowindex; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    worksheet.Cells[i, j].SetStyle(borderStyle);
                }
            }

            var topHeaderStyle = workbook.CreateStyle();
            topHeaderStyle.Font.IsBold = true;
            topHeaderStyle.HorizontalAlignment = TextAlignmentType.Left;

            worksheet.Cells[1, 0].SetStyle(topHeaderStyle);
            worksheet.Cells[2, 0].SetStyle(topHeaderStyle);
            worksheet.Cells[3, 0].SetStyle(topHeaderStyle);
            worksheet.Cells[4, 0].SetStyle(topHeaderStyle);

            using var stream = new MemoryStream();
            workbook.Save(stream, SaveFormat.Pdf);
            stream.Seek(0, SeekOrigin.Begin);

            int fileIndex = 0;
            if (File.Exists(Path.Combine(folderPath, "Salary Details for department.pdf")))
            {
                fileIndex++;

                while (File.Exists(Path.Combine(folderPath, $"Salary Details for department ({fileIndex}).pdf")))
                {
                    fileIndex++;
                }
            }

            string fileName;
            if (fileIndex == 0)
                fileName = "Salary Details for department.pdf";
            else
                fileName = $"Salary Details for department ({fileIndex}).pdf";

            var filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
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
        IEnumerable<SalaryDetailSummary> listSalarySummaries = [];

        listSalarySummaries = await _iSalaryRepo.GetSalaryDetailSummariesAsync(month, year);

        long TotalMoneySpent = 0;
        foreach (var item in listSalarySummaries)
            TotalMoneySpent += item.TotalSpent;

        // Pick a folder to store the file        
        var result = await FolderPicker.Default.PickAsync();

        if (result != null && result.Folder != null && !string.IsNullOrEmpty(result.Folder.Path))
        {
            // Folder is valid, proceed with exporting to PDF
            string folderPath = result.Folder.Path;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            // Add Title to PDF
            worksheet.Cells[0, 0].PutValue($"Salary Expenditure Report for {month}/{year}");
            var titleStyle = workbook.CreateStyle();
            titleStyle.Font.IsBold = true;
            titleStyle.Font.Size = 14;
            titleStyle.HorizontalAlignment = TextAlignmentType.Center;
            worksheet.Cells[0, 0].SetStyle(titleStyle);
            worksheet.Cells.Merge(0, 0, 1, 5);  // Merge title cell across all columns

            // Adjust spacing
            worksheet.Cells.SetRowHeight(0, 30);  // Height for the title row

            // Set up other headers for summary information
            worksheet.Cells[1, 0].PutValue("Number of departments:");
            worksheet.Cells.Merge(1, 0, 1, 1);
            worksheet.Cells[1, 2].PutValue(listSalarySummaries.Count().ToString());
            worksheet.Cells[2, 0].PutValue("Month/Year:");
            worksheet.Cells.Merge(2, 0, 1, 1);
            worksheet.Cells[2, 2].PutValue($"{month}/{year}");
            worksheet.Cells[3, 0].PutValue("Total Expenditure:");
            worksheet.Cells.Merge(1, 0, 1, 1);
            worksheet.Cells[3, 2].PutValue(TotalMoneySpent.ToString());

            // Style for summary headers
            var summaryStyle = workbook.CreateStyle();
            summaryStyle.Font.IsBold = true;
            summaryStyle.Font.Size = 10;
            summaryStyle.HorizontalAlignment = TextAlignmentType.Left;

            worksheet.Cells[1, 0].SetStyle(summaryStyle);
            worksheet.Cells[2, 0].SetStyle(summaryStyle);
            worksheet.Cells[3, 0].SetStyle(summaryStyle);

            worksheet.Cells.SetColumnWidth(0, 10);
            worksheet.Cells.SetColumnWidth(1, 13);
            worksheet.Cells.SetColumnWidth(2, 13);
            worksheet.Cells.SetColumnWidth(3, 12);
            worksheet.Cells.SetColumnWidth(4, 12);

            // Set up table headers
            worksheet.Cells[5, 0].PutValue("ID");
            worksheet.Cells[5, 1].PutValue("Name");
            worksheet.Cells[5, 2].PutValue("Head name");
            worksheet.Cells[5, 3].PutValue("Employees");
            worksheet.Cells[5, 4].PutValue("Total spent");

            var headerStyle = workbook.CreateStyle();
            headerStyle.Font.IsBold = true;
            headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;

            // Apply header style to all columns in the table
            for (int i = 0; i <= 4; i++)
            {
                worksheet.Cells[5, i].SetStyle(headerStyle);
            }

            int rowindex = 6;
            foreach (var item in listSalarySummaries)
            {
                var salary = await _iDepartmentRepo.GetDepartmentByIdAsync(item.DepartmentId);

                if (salary == null) return;

                worksheet.Cells[rowindex, 0].PutValue(item.DepartmentId);
                worksheet.Cells[rowindex, 1].PutValue(item.DepartmentName);
                worksheet.Cells[rowindex, 2].PutValue(salary.HeadOfDepartment?.FullName ?? "None");
                worksheet.Cells[rowindex, 3].PutValue(salary.Employees.Count.ToString());
                worksheet.Cells[rowindex, 4].PutValue(item.TotalSpent.ToString());

                rowindex++;
            }

            // Apply borders to the table rows
            var borderStyle = workbook.CreateStyle();
            borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

            for (int i = 6; i < rowindex; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    worksheet.Cells[i, j].SetStyle(borderStyle);
                }
            }

            using var stream = new MemoryStream();
            workbook.Save(stream, SaveFormat.Pdf);
            stream.Seek(0, SeekOrigin.Begin);

            int fileIndex = 0;
            if (File.Exists(Path.Combine(folderPath, "Expenditure Details.pdf")))
            {
                fileIndex++;

                while (File.Exists(Path.Combine(folderPath, $"Expenditure Details ({fileIndex}).pdf")))
                {
                    fileIndex++;
                }
            }

            string fileName;
            if (fileIndex == 0)
                fileName = "Expenditure Details.pdf";
            else
                fileName = $"Expenditure Details ({fileIndex}).pdf";

            var filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);

            await Shell.Current.DisplayAlert("Notification", "File exported successfully", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No valid folder selected.", "OK");
        }
    }

}
