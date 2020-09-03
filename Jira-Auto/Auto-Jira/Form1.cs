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
            btnExecute.Enabled = false;
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
                    List<FieldsTestExecution> fieldsIssues = ExcelUtils.getDataTestExecution(txtDataPath.Text, txtProjectKey.Text);
                    List<ResponseInfo> responseInfos = new List<ResponseInfo>();
                    foreach(var p in fieldsIssues)
                    {
                        TestExecution issue = new TestExecution { fields = p };
                        ResponseInfo responseInfo = RestShapUtils.getInstance().doRequestCreateTestExecution(issue);
                        responseInfos.Add(responseInfo);
                        txtInfo.Text += "\n"+ responseInfo.message;
                    }
                    saveFile(responseInfos);
                }
                else
                {
                    List<FieldsSubTestExecution> fieldsSubTests = ExcelUtils.getDataSubTestExecution(txtDataPath.Text, txtProjectKey.Text);
                    List<ResponseInfo> responseInfos = new List<ResponseInfo>();
                    foreach (var p in fieldsSubTests)
                    {
                        SubTestExecution issue = new SubTestExecution { fields = p };
                        ResponseInfo responseInfo = RestShapUtils.getInstance().doRequestCreateSubTestExecution(issue);
                        responseInfos.Add(responseInfo);
                        txtInfo.Text += "\n" + responseInfo.message;
                    }
                    saveFile(responseInfos);
                }              
            }
            btnExecute.Enabled = true;
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
        private static void saveFile(List<ResponseInfo> testExecutionInfos)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "TestExecutionResult" + DateTime.Now.Millisecond.ToString() + ".xlsx";
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                ExcelUtils.writeResultToExcel(savefile.FileName, testExecutionInfos);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String issueTypeChoose = cmbIssueType.SelectedItem.ToString();
            if (issueTypeChoose.Equals("Sub-TestExection")){
                issueType = IssueType.SubTestExecution;
            }
            else
            {
                issueType = IssueType.TestExecution;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
