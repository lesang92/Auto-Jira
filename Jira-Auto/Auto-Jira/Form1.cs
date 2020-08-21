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
using System.Windows.Forms.Design;
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
        public IssueType issueType;
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
               
                if (issueType == IssueType.TestExecution)
                {
                    List<FieldsIssue> fieldsIssues = ExcelUtils.buildDataIssue(txtDataPath.Text);
                    List<ResponseInfo> responseInfos = new List<ResponseInfo>();
                    foreach(var p in fieldsIssues)
                    {
                        Issue issue = new Issue { fields = p };
                        ResponseInfo responseInfo = RestShapUtils.getInstance().doRequestCreateIssue(issue, IssueType.TestExecution, txtProjectKey.Text);
                        responseInfos.Add(responseInfo);
                        txtInfo.Text += "\n"+ responseInfo.message;
                    }
                    saveFile(responseInfos);
                    txtTestPlanKey.Enabled = true;
                }
                else
                {
                    List<SubTestExecutionInfo> subTestExecutionCreated = craeteSubTestSuiteExecution();
                    saveFileSubTestExecution(subTestExecutionCreated);
                    txtTestPlanKey.Enabled = true;
                }
               
            }         
        }

        private List<IssueInfo> createTestExecution()
        {
            List<ExcelData> testExecutionInfos = buildData(txtDataPath.Text, txtProjectKey.Text);
            List<IssueInfo> testExecutionCreated = new List<IssueInfo>();
            foreach (ExcelData p in testExecutionInfos)
            {
                IssueInfo testExecution = new IssueInfo();
                var dataTest = buildDataTestExecution(p);
                testExecution = RestSharpServices.getInstance().doRequestCreateTestExecution(dataTest);
                testExecutionCreated.Add(testExecution);
                if (testExecution.isSuccessfully)
                {
                    txtInfo.Text += testExecution.toString() + " success!!!\n";
                    try
                    {
                        RestSharpServices.getInstance().doRequestAddTestEnvironment(p.projectKey, testExecution.id, p.testEnvironment);
                    }
                    catch (Exception e)
                    {

                    }
                    if (!String.IsNullOrEmpty(p.testPlanKey))
                    {
                        try
                        {
                            IRestResponse response = RestSharpServices.getInstance().doRequestAddTestExecutionToTestPlan(txtTestPlanKey.Text, new List<string> { testExecution.id });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
               
            }
            return testExecutionCreated;
        }


        private List<SubTestExecutionInfo> craeteSubTestSuiteExecution()
        {
            List<SubTestExecutionInfo> testExecutionInfos = buildSubTestExecutionData(txtDataPath.Text, txtProjectKey.Text);
            List<SubTestExecutionInfo> testExecutionCreated = new List<SubTestExecutionInfo>();
            foreach (SubTestExecutionInfo p in testExecutionInfos)
            {
                SubTestExecutionInfo testExecution = new SubTestExecutionInfo();
                testExecution = RestSharpServices.getInstance().doRequestCreateSubTestExecution(p);               
                testExecutionCreated.Add(testExecution);
                if (testExecution.isSuccessfully)
                {
                    txtInfo.Text += testExecution.toString() + " success!!!\n";
                }
            }
            return testExecutionCreated;
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

        private static List<ExcelData> buildData(String excelFile, String projectKey)
        {
            var result = new List<ExcelData>();
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
                string labels = worksheet.Cells[i, 3].Value != null ? worksheet.Cells[i, 3].Value.ToString() : "";
                string component = worksheet.Cells[i, 4].Value != null ? worksheet.Cells[i, 4].Value.ToString() : "";
                string versions = worksheet.Cells[i, 5].Value != null ? worksheet.Cells[i, 5].Value.ToString() : "";
                string priority = worksheet.Cells[i, 6].Value != null ? worksheet.Cells[i, 6].Value.ToString() : "";
                string environment = worksheet.Cells[i, 7].Value != null ? worksheet.Cells[i, 7].Value.ToString() : "";
                string testEnvironment = worksheet.Cells[i, 8].Value != null ? worksheet.Cells[i, 8].Value.ToString() : "";
                string testPlanKey = worksheet.Cells[i, 9].Value != null ? worksheet.Cells[i, 9].Value.ToString() : "";
                result.Add(new ExcelData
                {
                        projectKey = projectKey ,
                        priority = priority ,
                        labels = labels,
                        summary = summary,
                        description = description,
                        components = component  ,
                        versions = versions ,
                        environment = environment,
                        testEnvironment = testEnvironment,
                        testPlanKey = testPlanKey
                });               
            }
            return result;
        }

        public static IssueInfo buildDataTestExecution(ExcelData excelData)
        {
            IssueInfo issueInfo = new IssueInfo();
            FieldsPojo fieldsPojo = new FieldsPojo();
            fieldsPojo.project = new Project { key = excelData.projectKey };
            fieldsPojo.summary = excelData.summary;
            if (!String.IsNullOrEmpty(excelData.labels))
            {
                fieldsPojo.labels = excelData.labels.Split(',');
            }
            if (!String.IsNullOrEmpty(excelData.description))
            {
                fieldsPojo.description = excelData.description;
            }
            if (!String.IsNullOrEmpty(excelData.components))
            {
                fieldsPojo.components = new List<Field> { new Field { name = excelData.components } };
            }
            if (!String.IsNullOrEmpty(excelData.versions))
            {
                fieldsPojo.versions = new List<Field> { new Field { name = excelData.versions } };
            }
            if (!String.IsNullOrEmpty(excelData.environment))
            {
                fieldsPojo.environment = excelData.environment;
            }
            issueInfo.fields = fieldsPojo;
            return issueInfo;
           
        }

       // private static
        private static List<SubTestExecutionInfo> buildSubTestExecutionData(String excelFile, String projectKey)
        {
            var result = new List<SubTestExecutionInfo>();
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
                string testExecutionKey = worksheet.Cells[i, 2].Value.ToString();
                string testExecutionId = RestSharpServices.getInstance().doRequestGetIssueId(testExecutionKey);
                result.Add(new SubTestExecutionInfo
                {
                    fields = new FieldSubTestExecution
                    {
                        project = new Project { key = projectKey },                      
                        summary = summary,
                        parent = new Parrent { id = testExecutionId }
                    }
                });
            }
            return result;
        }

        private static void saveFile(List<ResponseInfo> testExecutionInfos)
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


        private static void saveFileSubTestExecution(List<SubTestExecutionInfo> testExecutionInfos)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "SubTestExecutionResult" + DateTime.Now.Millisecond.ToString() + ".xlsx";
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                writeResultSubtestExecutionToExcel(savefile.FileName, testExecutionInfos);
            }
        }
        private static void writeResultToExcel(String filePath, List<ResponseInfo> testExecutionInfos)
        {
            var newFile = new FileInfo(filePath);
            using (var excelPackage = new ExcelPackage(newFile))
            {
                var workSheet= excelPackage.Workbook.Worksheets.Add("First Sheet");
                workSheet.Cells[1, 1].Value = "Status";
                workSheet.Cells[1, 2].Value = "Key";
                workSheet.Cells[1, 3].Value = "Id";
                workSheet.Cells[1, 4].Value = "Summay";
                workSheet.Cells[1, 5].Value = "Comment";
                workSheet.Cells[2, 1].LoadFromCollection(testExecutionInfos);
                excelPackage.Save();
            }
        }


        private static void writeResultSubtestExecutionToExcel(String filePath, List<SubTestExecutionInfo> subTestExecutionInfos)
        {
            var newFile = new FileInfo(filePath);
            using (var excelPackage = new ExcelPackage(newFile))
            {
                var workSheet = excelPackage.Workbook.Worksheets.Add("First Sheet");
                workSheet.Cells[1, 1].Value = "Status";
                workSheet.Cells[1, 2].Value = "Key";
                workSheet.Cells[1, 3].Value = "Id";
                workSheet.Cells[2, 1].LoadFromCollection(subTestExecutionInfos);
                excelPackage.Save();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTestPlanKey.Enabled = true;
            String issueTypeChoose = cmbIssueType.SelectedItem.ToString();
            if (issueTypeChoose.Equals("Sub-TestExection")){
                txtTestPlanKey.Enabled = false;
                issueType = IssueType.SubTestExecution;
            }
            else
            {
                txtTestPlanKey.Enabled = true;
                issueType = IssueType.TestExecution;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }

}
