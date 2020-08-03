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
            this.txtConfigInfo = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTestPlanKey = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtProjectKey
            // 
            this.txtProjectKey.Location = new System.Drawing.Point(127, 33);
            this.txtProjectKey.Name = "txtProjectKey";
            this.txtProjectKey.Size = new System.Drawing.Size(75, 20);
            this.txtProjectKey.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Project key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Data";
            // 
            // btnBrowserData
            // 
            this.btnBrowserData.Location = new System.Drawing.Point(359, 73);
            this.btnBrowserData.Name = "btnBrowserData";
            this.btnBrowserData.Size = new System.Drawing.Size(75, 23);
            this.btnBrowserData.TabIndex = 4;
            this.btnBrowserData.Text = "Browser";
            this.btnBrowserData.UseVisualStyleBackColor = true;
            this.btnBrowserData.Click += new System.EventHandler(this.btnBrowserData_Click);
            // 
            // txtDataPath
            // 
            this.txtDataPath.Location = new System.Drawing.Point(127, 75);
            this.txtDataPath.Name = "txtDataPath";
            this.txtDataPath.Size = new System.Drawing.Size(217, 20);
            this.txtDataPath.TabIndex = 3;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(188, 365);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            // 
            // txtConfigInfo
            // 
            this.txtConfigInfo.Location = new System.Drawing.Point(55, 131);
            this.txtConfigInfo.Name = "txtConfigInfo";
            this.txtConfigInfo.ReadOnly = true;
            this.txtConfigInfo.Size = new System.Drawing.Size(379, 176);
            this.txtConfigInfo.TabIndex = 7;
            this.txtConfigInfo.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(250, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Test plan key";
            // 
            // txtTestPlanKey
            // 
            this.txtTestPlanKey.Location = new System.Drawing.Point(325, 33);
            this.txtTestPlanKey.Name = "txtTestPlanKey";
            this.txtTestPlanKey.Size = new System.Drawing.Size(75, 20);
            this.txtTestPlanKey.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTestPlanKey);
            this.Controls.Add(this.txtConfigInfo);
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
        private System.Windows.Forms.RichTextBox txtConfigInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTestPlanKey;
    }
}

