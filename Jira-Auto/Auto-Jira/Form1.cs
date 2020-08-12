using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Auto_Jira
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtTestPlanKey.Enabled = true;
        }

        private static String issueType;
        private void btnBrowserData_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "C# Corner Open File Dialog";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "All files (*.*)|*.*|Excel files (*.*)|*.xlsx*";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                txtDataPath.Text = fdlg.FileName;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            txtInfo.Text = "";
            if (validate().Length > 0)
            {
                txtInfo.Text = validate();
                txtInfo.ForeColor = Color.Red;
            }
            else
            {
                List<IssueInfo> testExecutionInfos = buildData(txtDataPath.Text, txtProjectKey.Text);
                List<IssueInfo> testExecutionCreated = new List<IssueInfo>();
                foreach (IssueInfo p in testExecutionInfos)
                {
                    IssueInfo testExecution = RestSharpServices.getInstance().doRequestCreateTestExecution(p);
                    testExecutionCreated.Add(testExecution);
                    if (testExecution.isSuccessfully)
                    {
                        txtInfo.Text += testExecution.toString() + " success!!!\n";
                    }
                }
                List<String> testExecutionId = testExecutionCreated.Select(x => x.id).ToList();
                if (!string.IsNullOrEmpty(txtTestPlanKey.Text))
                {
                    try
                    {
                        IRestResponse response = RestSharpServices.getInstance().doRequestAddTestExecutionToTestPlan(txtTestPlanKey.Text, testExecutionId);
                    }
                    catch (Exception ex)
                    {

                    }
                }               
                saveFile(testExecutionCreated);
                txtTestPlanKey.Enabled = true;
            }         
        }

        private String validate()
        {          
            String result = "";
            if (String.IsNullOrEmpty(txtDataPath.Text))
            {
                result += "Data file path is require\n";
            }
            if (String.IsNullOrEmpty(txtProjectKey.Text))
            {
                result += "Project key is require\n";
            }

            return result;
        }

        private static List<IssueInfo> buildData(String excelFile, String projectKey)
        {
            var result = new List<IssueInfo>();
            FileInfo fileInfo = new FileInfo(excelFile);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            // get number of rows and columns in the sheet
            int rows = worksheet.Dimension.Rows; // 20
            int columns = worksheet.Dimension.Columns; // 7

            // loop through the worksheet rows and columns
            for (int i = 2; i <= rows; i++)
            {
                string summary = worksheet.Cells[i, 1].Value.ToString();
                string description = worksheet.Cells[i, 2].Value != null ? worksheet.Cells[i, 2].Value.ToString() : "";
                string labels = worksheet.Cells[i, 3].Value.ToString();
                string component = worksheet.Cells[i, 4].Value.ToString();
                string versions = worksheet.Cells[i, 5].Value.ToString();
                string priority = worksheet.Cells[i, 6].Value.ToString();
                string environment = worksheet.Cells[i, 7].Value.ToString();
                result.Add(new IssueInfo
                {
                    fields = new FieldsPojo
                    {
                        project = new Field { name = projectKey },
                        priority = new Field { name = priority },
                        labels = labels.Split(','),
                        summary = summary,
                        description = description,
                        component = new Field { name = component },
                        versions = new Field { name = versions },
                        environment = new Field { name = environment }
                    }
                });               
            }
            return result;
        }

        private static void saveFile(List<IssueInfo> testExecutionInfos)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "TestExecutionResult" + DateTime.Now.Millisecond.ToString() + ".xlsx";
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                writeResultToExcel(savefile.FileName, testExecutionInfos);
            }
        }
        private static void writeResultToExcel(String filePath, List<IssueInfo> testExecutionInfos)
        {
            var newFile = new FileInfo(filePath);
            using (var excelPackage = new ExcelPackage(newFile))
            {
                var workSheet= excelPackage.Workbook.Worksheets.Add("First Sheet");
                workSheet.Cells[1, 1].Value = "Status";
                workSheet.Cells[1, 2].Value = "Key";
                workSheet.Cells[1, 3].Value = "Id";
                workSheet.Cells[1, 4].Value = "Summay";
                workSheet.Cells[2, 1].LoadFromCollection(testExecutionInfos);
                excelPackage.Save();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTestPlanKey.Enabled = true;
            String issueTypeChoose = cmbIssueType.SelectedItem.ToString();
            issueType = issueTypeChoose;
            if (issueTypeChoose.Equals("Sub-TestExection")){
                txtTestPlanKey.Enabled = false;
            }
            else
            {
                txtTestPlanKey.Enabled = true;
            }
        }
    }

}
