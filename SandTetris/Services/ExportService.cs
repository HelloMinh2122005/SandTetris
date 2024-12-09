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
        IEnumerable<SalaryDetail> listSalariesDepartment = Enumerable.Empty<SalaryDetail>();

        if (month == 0 && year == 0)
            listSalariesDepartment = await _iSalaryRepo.GetSalaryDetailsForDepartmentAsync(departmentID);
        else
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

            // Set up worksheet columns and headers
            worksheet.Cells.SetColumnWidth(0, 20);
            worksheet.Cells.SetColumnWidth(1, 20);
            worksheet.Cells.SetColumnWidth(2, 20);
            worksheet.Cells.SetColumnWidth(3, 12);
            worksheet.Cells.SetColumnWidth(4, 12);
            worksheet.Cells.SetColumnWidth(5, 15);
            worksheet.Cells.SetColumnWidth(6, 15);

            worksheet.Cells[0, 0].PutValue("Department ID:");
            worksheet.Cells[0, 1].PutValue(department.Id);
            worksheet.Cells[1, 0].PutValue("Department Name:");
            worksheet.Cells[1, 1].PutValue(department.Name);
            worksheet.Cells[2, 0].PutValue("Month/Year:");
            if (month == 0 && year == 0)
                worksheet.Cells[2, 1].PutValue("From the beginning.");
            else
                worksheet.Cells[2, 1].PutValue($"{month}/{year}");
            worksheet.Cells[3, 0].PutValue("Total Expenditure:");
            worksheet.Cells[3, 1].PutValue(TotalMoneySpent.ToString());

            worksheet.Cells[5, 0].PutValue("ID");
            worksheet.Cells[5, 1].PutValue("Employee Name");
            worksheet.Cells[5, 2].PutValue("Base Salary");
            worksheet.Cells[5, 3].PutValue("Days Worked");
            worksheet.Cells[5, 4].PutValue("Days Absent");
            worksheet.Cells[5, 5].PutValue("Days on Leave");
            worksheet.Cells[5, 6].PutValue("Final Salary");

            int rowindex = 6;

            foreach (var item in listSalariesDepartment)
            {
                int DayInMonth = DateTime.DaysInMonth(year, month);

                worksheet.Cells[rowindex, 0].PutValue(item.EmployeeId);
                worksheet.Cells[rowindex, 1].PutValue(item.Employee.FullName);
                worksheet.Cells[rowindex, 2].PutValue(item.BaseSalary);
                worksheet.Cells[rowindex, 3].PutValue(DayInMonth - item.DaysAbsent - item.DaysOnLeave);
                worksheet.Cells[rowindex, 4].PutValue(item.DaysAbsent);
                worksheet.Cells[rowindex, 5].PutValue(item.DaysOnLeave);
                worksheet.Cells[rowindex, 6].PutValue(item.FinalSalary);

                rowindex++;
            }

            var headerStyle = workbook.CreateStyle();
            headerStyle.Font.IsBold = true;
            headerStyle.ForegroundColor = System.Drawing.Color.LightGray;
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;

            worksheet.Cells[5, 0].SetStyle(headerStyle);
            worksheet.Cells[5, 1].SetStyle(headerStyle);
            worksheet.Cells[5, 2].SetStyle(headerStyle);
            worksheet.Cells[5, 3].SetStyle(headerStyle);
            worksheet.Cells[5, 4].SetStyle(headerStyle);
            worksheet.Cells[5, 5].SetStyle(headerStyle);
            worksheet.Cells[5, 6].SetStyle(headerStyle);

            var borderStyle = workbook.CreateStyle();
            borderStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            borderStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

            for (int i = 6; i < rowindex; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    worksheet.Cells[i, j].SetStyle(borderStyle);
                }
            }

            var topHeaderStyle = workbook.CreateStyle();
            topHeaderStyle.Font.IsBold = true;
            topHeaderStyle.HorizontalAlignment = TextAlignmentType.Left;

            worksheet.Cells[0, 1].SetStyle(topHeaderStyle);
            worksheet.Cells[1, 1].SetStyle(topHeaderStyle);
            worksheet.Cells[2, 1].SetStyle(topHeaderStyle);
            worksheet.Cells[3, 1].SetStyle(topHeaderStyle);

            using var stream = new MemoryStream();
            workbook.Save(stream, SaveFormat.Pdf);
            stream.Seek(0, SeekOrigin.Begin);

            int fileIndex = 0;
            while (File.Exists(Path.Combine(folderPath, $"SalaryDetails ({fileIndex}).pdf")))
            {
                fileIndex++;
            }

            var fileName = $"SalaryDetails ({fileIndex}).pdf";
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

        if (month == 0 && year == 0)
            listSalarySummaries = await _iSalaryRepo.GetAllSalaryDetailSummariesAsync();
        else
            listSalarySummaries = await _iSalaryRepo.GetSalaryDetailSummariesAsync(month, year);


    }
}
