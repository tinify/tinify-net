
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtAPIKEY = new System.Windows.Forms.TextBox();
            this.btnSaveAPI = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDest = new System.Windows.Forms.TextBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnDest = new System.Windows.Forms.Button();
            this.lblNetSpeed = new System.Windows.Forms.Label();
            this.chkMove = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(797, 391);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 96);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test Compress";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select  a folder (source root folder)";
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderPath.Location = new System.Drawing.Point(13, 65);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(810, 23);
            this.txtFolderPath.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(829, 65);
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
            this.lblInfo.Location = new System.Drawing.Point(11, 192);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(15, 15);
            this.lblInfo.TabIndex = 4;
            this.lblInfo.Text = "[]";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalculate.Enabled = false;
            this.btnCalculate.Location = new System.Drawing.Point(752, 159);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(115, 23);
            this.btnCalculate.TabIndex = 5;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnComAll
            // 
            this.btnComAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnComAll.Enabled = false;
            this.btnComAll.Location = new System.Drawing.Point(752, 188);
            this.btnComAll.Name = "btnComAll";
            this.btnComAll.Size = new System.Drawing.Size(115, 23);
            this.btnComAll.TabIndex = 6;
            this.btnComAll.Text = "Compress all";
            this.btnComAll.UseVisualStyleBackColor = true;
            this.btnComAll.Click += new System.EventHandler(this.btnComAll_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Your API KEY";
            // 
            // txtAPIKEY
            // 
            this.txtAPIKEY.Location = new System.Drawing.Point(12, 21);
            this.txtAPIKEY.Name = "txtAPIKEY";
            this.txtAPIKEY.Size = new System.Drawing.Size(354, 23);
            this.txtAPIKEY.TabIndex = 2;
            // 
            // btnSaveAPI
            // 
            this.btnSaveAPI.Location = new System.Drawing.Point(385, 21);
            this.btnSaveAPI.Name = "btnSaveAPI";
            this.btnSaveAPI.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAPI.TabIndex = 7;
            this.btnSaveAPI.Text = "Save API?";
            this.btnSaveAPI.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(269, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Select  a destinition folder (destinition root folder)";
            // 
            // txtDest
            // 
            this.txtDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDest.Location = new System.Drawing.Point(13, 112);
            this.txtDest.Name = "txtDest";
            this.txtDest.Size = new System.Drawing.Size(810, 23);
            this.txtDest.TabIndex = 2;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(12, 159);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(15, 15);
            this.lblSummary.TabIndex = 4;
            this.lblSummary.Text = "[]";
            // 
            // btnDest
            // 
            this.btnDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDest.Location = new System.Drawing.Point(829, 112);
            this.btnDest.Name = "btnDest";
            this.btnDest.Size = new System.Drawing.Size(38, 23);
            this.btnDest.TabIndex = 3;
            this.btnDest.Text = "...";
            this.btnDest.UseVisualStyleBackColor = true;
            this.btnDest.Click += new System.EventHandler(this.btnDest_Click);
            // 
            // lblNetSpeed
            // 
            this.lblNetSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNetSpeed.AutoSize = true;
            this.lblNetSpeed.Location = new System.Drawing.Point(11, 229);
            this.lblNetSpeed.Name = "lblNetSpeed";
            this.lblNetSpeed.Size = new System.Drawing.Size(15, 15);
            this.lblNetSpeed.TabIndex = 4;
            this.lblNetSpeed.Text = "[]";
            // 
            // chkMove
            // 
            this.chkMove.AutoSize = true;
            this.chkMove.Location = new System.Drawing.Point(13, 137);
            this.chkMove.Name = "chkMove";
            this.chkMove.Size = new System.Drawing.Size(188, 19);
            this.chkMove.TabIndex = 8;
            this.chkMove.Text = "Move source after compressed";
            this.chkMove.UseVisualStyleBackColor = true;
            // 
            // FormTinifyClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 357);
            this.Controls.Add(this.chkMove);
            this.Controls.Add(this.btnSaveAPI);
            this.Controls.Add(this.btnComAll);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.lblNetSpeed);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnDest);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtAPIKEY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "FormTinifyClient";
            this.Text = "TinyPNG";
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAPIKEY;
        private System.Windows.Forms.Button btnSaveAPI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDest;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnDest;
        private System.Windows.Forms.Label lblNetSpeed;
        private System.Windows.Forms.CheckBox chkMove;
    }
}

