namespace TOOL_TW_DDN
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
            this.components = new System.ComponentModel.Container();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.templateComboBox = new System.Windows.Forms.ComboBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.btnSelectDumpFolder = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.authorInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendFeedbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";

            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.authorInfoToolStripMenuItem,
                this.sendFeedbackToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";

            // 
            // authorInfoToolStripMenuItem
            // 
            //this.authorInfoToolStripMenuItem.Name = "authorInfoToolStripMenuItem";
            //this.authorInfoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            //this.authorInfoToolStripMenuItem.Text = "Author Info";
            //this.authorInfoToolStripMenuItem.Click += new System.EventHandler(this.AuthorInfoToolStripMenuItem_Click);

            // 
            // sendFeedbackToolStripMenuItem
            // 
            //this.sendFeedbackToolStripMenuItem.Name = "sendFeedbackToolStripMenuItem";
            //this.sendFeedbackToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            //this.sendFeedbackToolStripMenuItem.Text = "Send Feedback";
            //this.sendFeedbackToolStripMenuItem.Click += new System.EventHandler(this.SendFeedbackToolStripMenuItem_Click);

            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.btnSelectDumpFolder);
            this.headerPanel.Controls.Add(this.generateButton);
            this.headerPanel.Controls.Add(this.templateComboBox);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 24);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(800, 100);
            this.headerPanel.TabIndex = 1;

            // 
            // templateComboBox
            // 
            this.templateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.templateComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.templateComboBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.templateComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.templateComboBox.Items.AddRange(new object[] {
                "AddCell_L1874",
                "AddCell_L1874-IFHO",
                "AddCell_L900",
                "AddCell_L900-IFHO",
                "ENDC Non-CA",
                "ENDC CA",
                "TWCA"});
            this.templateComboBox.Location = new System.Drawing.Point(20, 50);
            this.templateComboBox.Name = "templateComboBox";
            this.templateComboBox.Size = new System.Drawing.Size(300, 29);
            this.templateComboBox.TabIndex = 0;
            this.templateComboBox.SelectedIndex = 0;

            // 
            // generateButton
            // 
            this.generateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.generateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generateButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateButton.ForeColor = System.Drawing.Color.White;
            this.generateButton.Location = new System.Drawing.Point(330, 50);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(160, 40);
            this.generateButton.TabIndex = 1;
            this.generateButton.Text = "Generate XML";
            this.generateButton.UseVisualStyleBackColor = false;
            this.generateButton.Click += new System.EventHandler(this.GenerateXml);

            // 
            // btnSelectDumpFolder
            // 
            this.btnSelectDumpFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.btnSelectDumpFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectDumpFolder.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectDumpFolder.ForeColor = System.Drawing.Color.White;
            this.btnSelectDumpFolder.Location = new System.Drawing.Point(500, 50);
            this.btnSelectDumpFolder.Name = "btnSelectDumpFolder";
            this.btnSelectDumpFolder.Size = new System.Drawing.Size(160, 40);
            this.btnSelectDumpFolder.TabIndex = 2;
            this.btnSelectDumpFolder.Text = "Select Dump";
            this.btnSelectDumpFolder.UseVisualStyleBackColor = false;
            this.btnSelectDumpFolder.Click += new System.EventHandler(this.BtnSelectDumpFolder_Click);

            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip.SetToolTip(this.templateComboBox, "Chọn template:\n- AddCell: Thêm cell mới (L1874, L900)\n- ENDC: Cấu hình 5G\n- TWCA: Carrier Aggregation 4G");

            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressBar.Location = new System.Drawing.Point(20, 100);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(760, 23);
            this.progressBar.TabIndex = 5;
            this.headerPanel.Controls.Add(this.progressBar);

            // Thêm nút Open Results Folder
            this.btnOpenResultsFolder = new System.Windows.Forms.Button();
            this.btnOpenResultsFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.btnOpenResultsFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenResultsFolder.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenResultsFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenResultsFolder.Location = new System.Drawing.Point(670, 50);
            this.btnOpenResultsFolder.Name = "btnOpenResultsFolder";
            this.btnOpenResultsFolder.Size = new System.Drawing.Size(110, 40);
            this.btnOpenResultsFolder.TabIndex = 3;
            this.btnOpenResultsFolder.Text = "Open Results";
            this.btnOpenResultsFolder.UseVisualStyleBackColor = false;
            this.btnOpenResultsFolder.Click += new System.EventHandler(this.BtnOpenResultsFolder_Click);

            this.headerPanel.Controls.Add(this.btnOpenResultsFolder);

            // Thêm TextBox cho delimiter
            this.delimiterTextBox = new System.Windows.Forms.TextBox();
            this.delimiterTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delimiterTextBox.Location = new System.Drawing.Point(330, 20);
            this.delimiterTextBox.Name = "delimiterTextBox";
            this.delimiterTextBox.Size = new System.Drawing.Size(50, 27);
            this.delimiterTextBox.TabIndex = 4;
            this.delimiterTextBox.Text = ",";
            this.delimiterTextBox.MaxLength = 1;

            this.headerPanel.Controls.Add(this.delimiterTextBox);

            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.logTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logTextBox.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.logTextBox.Location = new System.Drawing.Point(20, 134);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(760, 446);
            this.logTextBox.TabIndex = 2;
            this.logTextBox.Text = "";

            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.titleLabel.Location = new System.Drawing.Point(20, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(246, 37);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "CSV to XML Generator";

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(600, 450);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TW_DDN - CSV to XML Generator";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.ComboBox templateComboBox;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Button btnSelectDumpFolder;
        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem authorInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendFeedbackToolStripMenuItem;
        private System.Windows.Forms.Button btnOpenResultsFolder;
        private System.Windows.Forms.TextBox delimiterTextBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}