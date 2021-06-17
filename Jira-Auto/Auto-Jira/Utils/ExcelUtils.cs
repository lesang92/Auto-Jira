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
        public static List<FieldsTestExecution> getDataTestExecution(String pathFile, string projectKey)
        {
            var result = new List<FieldsTestExecution>();
            FileInfo fileInfo = new FileInfo(pathFile);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            // get number of rows and columns in the sheet
            int rows = worksheet.Dimension.Rows; // 20
            for(int i=2;i<= rows; i++)
            {
                FieldsTestExecution issue = new FieldsTestExecution();
                issue.summary = getDataCell(worksheet.Cells[i, 1]);
                issue.description = getDataCell(worksheet.Cells[i, 2]);
                issue.labels = getDataCell(worksheet.Cells[i, 3]).Split(',');
                string component = getDataCell(worksheet.Cells[i, 4]);
                List<FieldByName> components = new List<FieldByName>();
                if (!string.IsNullOrEmpty(component.Trim()))
                {
                    string[] componentStrings = component.Split(',');
                    foreach (var p in componentStrings)
                    {
                        if (!string.IsNullOrEmpty(p))
                        {
                            components.Add(new FieldByName { name = p });
                        }
                    }                 
                }
                issue.components = components.Count > 0 ? components : null;              
                string version = getDataCell(worksheet.Cells[i, 5]); 
                List<FieldByName> versions = new List<FieldByName>();
                if (!string.IsNullOrEmpty(version))
                {
                    string[] versionsString = version.Split(',');                   
                    foreach (var p in versionsString)
                    {
                        versions.Add(new FieldByName { name = p });
                    }
                }
                issue.versions = versions.Count > 0 ? versions : null;
                issue.priority = new FieldByName { name = getDataCell(worksheet.Cells[i, 6]) };
                issue.environment = getDataCell(worksheet.Cells[i, 7]);
                issue.testEnvironment = getDataCell(worksheet.Cells[i, 8]);
                issue.testPlanKey = getDataCell(worksheet.Cells[i, 9]);
                issue.project = new FieldByKey { key = projectKey };
                result.Add(issue);
            }
            return result;
        }


        public static List<FieldsSubTestExecution> getDataSubTestExecution(String pathFile, string projectKey)
        {
            var result = new List<FieldsSubTestExecution>();
            FileInfo fileInfo = new FileInfo(pathFile);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            // get number of rows and columns in the sheet
            int rows = worksheet.Dimension.Rows; // 20
            for (int i = 2; i <= rows; i++)
            {
                FieldsSubTestExecution issue = new FieldsSubTestExecution();
                issue.summary = getDataCell(worksheet.Cells[i, 1]);
                issue.description = getDataCell(worksheet.Cells[i, 2]);
                issue.labels = getDataCell(worksheet.Cells[i, 3]).Split(',');
                string component = getDataCell(worksheet.Cells[i, 4]);
                List<FieldByName> components = new List<FieldByName>();
                if (!string.IsNullOrEmpty(component.Trim()))
                {
                    string[] componentStrings = component.Split(',');
                    foreach (var p in componentStrings)
                    {
                        if (!string.IsNullOrEmpty(p))
                        {
                            components.Add(new FieldByName { name = p });
                        }
                    }
                }
                issue.components = components.Count > 0 ? components : null;
                issue.parent = new FieldByKey { key = getDataCell(worksheet.Cells[i, 5]) };
                issue.priority = new FieldByName { name = getDataCell(worksheet.Cells[i, 6]) };
                issue.testEnvironment = getDataCell(worksheet.Cells[i, 7]);
                issue.testPlanKey = getDataCell(worksheet.Cells[i, 8]);
                issue.project = new FieldByKey { key = projectKey };
                result.Add(issue);
            }
            return result;
        }

        private static String getDataCell(ExcelRange cells)
        {
            return cells != null && cells.Value !=null ? cells.Value.ToString() : "";
        }

        public static void writeResultToExcel(String filePath, List<ResponseInfo> testExecutionInfos)
        {
            var newFile = new FileInfo(filePath);
            using (var excelPackage = new ExcelPackage(newFile))
            {
                var workSheet = excelPackage.Workbook.Worksheets.Add("First Sheet");
                workSheet.Cells[1, 1].Value = "Id";
                workSheet.Cells[1, 2].Value = "Key";
                workSheet.Cells[1, 3].Value = "Status";
                workSheet.Cells[1, 4].Value = "Summay";
                workSheet.Cells[1, 5].Value = "Comment";
                workSheet.Cells[2, 1].LoadFromCollection(testExecutionInfos);
                excelPackage.Save();
            }
        }
    }
}
