
namespace TinifyClient
{
    partial class FormTinifyClient
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnComAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(688, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 96);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test Compress";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select  a folder";
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderPath.Location = new System.Drawing.Point(13, 65);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(1029, 23);
            this.txtFolderPath.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(1048, 65);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(38, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "...";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(13, 141);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(15, 15);
            this.lblInfo.TabIndex = 4;
            this.lblInfo.Text = "[]";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Enabled = false;
            this.btnCalculate.Location = new System.Drawing.Point(971, 104);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(115, 23);
            this.btnCalculate.TabIndex = 5;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnComAll
            // 
            this.btnComAll.Location = new System.Drawing.Point(971, 133);
            this.btnComAll.Name = "btnComAll";
            this.btnComAll.Size = new System.Drawing.Size(115, 23);
            this.btnComAll.TabIndex = 6;
            this.btnComAll.Text = "Compress all";
            this.btnComAll.UseVisualStyleBackColor = true;
            this.btnComAll.Click += new System.EventHandler(this.btnComAll_Click);
            // 
            // FormTinifyClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 409);
            this.Controls.Add(this.btnComAll);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "FormTinifyClient";
            this.Text = "Test TinifyClient";
            this.Load += new System.EventHandler(this.FormTinifyClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnComAll;
    }
}

