using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Jira
{
    public class ExcelUtils
    {
        public static List<FieldsIssue> buildDataIssue(String pathFile)
        {
            var result = new List<FieldsIssue>();
            FileInfo fileInfo = new FileInfo(pathFile);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            // get number of rows and columns in the sheet
            int rows = worksheet.Dimension.Rows; // 20
            for(int i=1;i< rows; i++)
            {
                FieldsIssue issue = new FieldsIssue();
                issue.summary = getDataCell(worksheet.Cells[i, 1]);
                issue.description = getDataCell(worksheet.Cells[i, 2]);
                issue.labels = getDataCell(worksheet.Cells[i, 3]).Split(',');
                string[] componentStrings = getDataCell(worksheet.Cells[i, 4]).Split(',');
                List<FieldByName> components = new List<FieldByName>();
                foreach(var p in componentStrings)
                {
                    components.Add(new FieldByName { name = p });
                }
                issue.components = components;
                string[] versionsString = getDataCell(worksheet.Cells[i, 5]).Split(',');
                List<FieldByName> versions = new List<FieldByName>();
                foreach(var p in versionsString)
                {
                    versions.Add(new FieldByName { name = p });
                }
                issue.versions = versions;
                issue.priority = new FieldByName { name = getDataCell(worksheet.Cells[i, 6]) };
                issue.environment = getDataCell(worksheet.Cells[i, 7]);
                issue.testEnvironment = getDataCell(worksheet.Cells[i, 8]);
                issue.testPlanKey = getDataCell(worksheet.Cells[i, 9]);
            }
            return result;
        }

        private static String getDataCell(ExcelRange cells)
        {
            return cells == null ? "" : cells.Value.ToString();
        }
    }
}
