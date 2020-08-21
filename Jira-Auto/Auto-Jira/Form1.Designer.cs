namespace Auto_Jira
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtProjectKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowserData = new System.Windows.Forms.Button();
            this.txtDataPath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTestPlanKey = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbIssueType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtProjectKey
            // 
            this.txtProjectKey.Location = new System.Drawing.Point(187, 129);
            this.txtProjectKey.Name = "txtProjectKey";
            this.txtProjectKey.Size = new System.Drawing.Size(121, 20);
            this.txtProjectKey.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Project key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(112, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Data";
            // 
            // btnBrowserData
            // 
            this.btnBrowserData.Location = new System.Drawing.Point(419, 169);
            this.btnBrowserData.Name = "btnBrowserData";
            this.btnBrowserData.Size = new System.Drawing.Size(75, 23);
            this.btnBrowserData.TabIndex = 4;
            this.btnBrowserData.Text = "Browser";
            this.btnBrowserData.UseVisualStyleBackColor = true;
            this.btnBrowserData.Click += new System.EventHandler(this.btnBrowserData_Click);
            // 
            // txtDataPath
            // 
            this.txtDataPath.Location = new System.Drawing.Point(187, 171);
            this.txtDataPath.Name = "txtDataPath";
            this.txtDataPath.Size = new System.Drawing.Size(217, 20);
            this.txtDataPath.TabIndex = 3;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(248, 461);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(115, 227);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.Size = new System.Drawing.Size(379, 176);
            this.txtInfo.TabIndex = 7;
            this.txtInfo.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(404, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Test plan key";
            // 
            // txtTestPlanKey
            // 
            this.txtTestPlanKey.Location = new System.Drawing.Point(479, 93);
            this.txtTestPlanKey.Name = "txtTestPlanKey";
            this.txtTestPlanKey.Size = new System.Drawing.Size(75, 20);
            this.txtTestPlanKey.TabIndex = 8;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(74, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Choose Issue Type";
            // 
            // cmbIssueType
            // 
            this.cmbIssueType.FormattingEnabled = true;
            this.cmbIssueType.Items.AddRange(new object[] {
            "TestExecution",
            "Sub-TestExection"});
            this.cmbIssueType.Location = new System.Drawing.Point(187, 92);
            this.cmbIssueType.Name = "cmbIssueType";
            this.cmbIssueType.Size = new System.Drawing.Size(121, 21);
            this.cmbIssueType.TabIndex = 11;
            this.cmbIssueType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 499);
            this.Controls.Add(this.cmbIssueType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTestPlanKey);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowserData);
            this.Controls.Add(this.txtDataPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtProjectKey);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtProjectKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowserData;
        private System.Windows.Forms.TextBox txtDataPath;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.RichTextBox txtInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTestPlanKey;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbIssueType;
    }
}

