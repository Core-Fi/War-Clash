namespace ExcelConfigExport
{
    partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.groupBox_Log = new System.Windows.Forms.GroupBox();
			this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
			this.button_OpenExcel = new System.Windows.Forms.Button();
			this.button_Export = new System.Windows.Forms.Button();
			this.label_ClientTarget = new System.Windows.Forms.Label();
			this.label_ServerTarget = new System.Windows.Forms.Label();
			this.textBox_ClientConfig = new System.Windows.Forms.TextBox();
			this.textBox_ServerSrc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox_Sheets = new System.Windows.Forms.ListBox();
			this.textBox_ClientSrc = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox_ServerConfig = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkBox_ExportConfig = new System.Windows.Forms.CheckBox();
			this.checkBox_ExportSrc = new System.Windows.Forms.CheckBox();
			this.button_CheckData = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox_ClassNameCS = new System.Windows.Forms.TextBox();
			this.numericUpDown_StepSize = new System.Windows.Forms.NumericUpDown();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBox_ManagerCS = new System.Windows.Forms.ComboBox();
			this.numericUpDown_KeyCount = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown_GroupSize = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown_Capacity = new System.Windows.Forms.NumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox_ExcelList = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboBox_ManagerCPP = new System.Windows.Forms.ComboBox();
			this.textBox_ClassNameCPP = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.groupBox_Log.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_StepSize)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_KeyCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_GroupSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Capacity)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox_Log
			// 
			this.groupBox_Log.Controls.Add(this.richTextBox_Log);
			this.groupBox_Log.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox_Log.Location = new System.Drawing.Point(0, 476);
			this.groupBox_Log.Name = "groupBox_Log";
			this.groupBox_Log.Size = new System.Drawing.Size(809, 185);
			this.groupBox_Log.TabIndex = 13;
			this.groupBox_Log.TabStop = false;
			this.groupBox_Log.Text = "日志";
			// 
			// richTextBox_Log
			// 
			this.richTextBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox_Log.Location = new System.Drawing.Point(3, 17);
			this.richTextBox_Log.Name = "richTextBox_Log";
			this.richTextBox_Log.Size = new System.Drawing.Size(803, 165);
			this.richTextBox_Log.TabIndex = 12;
			this.richTextBox_Log.Text = "";
			// 
			// button_OpenExcel
			// 
			this.button_OpenExcel.Location = new System.Drawing.Point(105, 38);
			this.button_OpenExcel.Name = "button_OpenExcel";
			this.button_OpenExcel.Size = new System.Drawing.Size(89, 24);
			this.button_OpenExcel.TabIndex = 1;
			this.button_OpenExcel.Text = "打开Excel(&O)";
			this.button_OpenExcel.UseVisualStyleBackColor = true;
			this.button_OpenExcel.Click += new System.EventHandler(this.button_OpenExcel_Click);
			// 
			// button_Export
			// 
			this.button_Export.Enabled = false;
			this.button_Export.Location = new System.Drawing.Point(708, 443);
			this.button_Export.Name = "button_Export";
			this.button_Export.Size = new System.Drawing.Size(89, 27);
			this.button_Export.TabIndex = 11;
			this.button_Export.Text = "导出文件(&S)";
			this.button_Export.UseVisualStyleBackColor = true;
			this.button_Export.Click += new System.EventHandler(this.button_Export_Click);
			// 
			// label_ClientTarget
			// 
			this.label_ClientTarget.AutoSize = true;
			this.label_ClientTarget.Location = new System.Drawing.Point(8, 21);
			this.label_ClientTarget.Name = "label_ClientTarget";
			this.label_ClientTarget.Size = new System.Drawing.Size(95, 12);
			this.label_ClientTarget.TabIndex = 7;
			this.label_ClientTarget.Text = "客户端配置文件:";
			// 
			// label_ServerTarget
			// 
			this.label_ServerTarget.AutoSize = true;
			this.label_ServerTarget.Location = new System.Drawing.Point(8, 73);
			this.label_ServerTarget.Name = "label_ServerTarget";
			this.label_ServerTarget.Size = new System.Drawing.Size(77, 12);
			this.label_ServerTarget.TabIndex = 8;
			this.label_ServerTarget.Text = "C++代码文件:";
			// 
			// textBox_ClientConfig
			// 
			this.textBox_ClientConfig.Enabled = false;
			this.textBox_ClientConfig.Location = new System.Drawing.Point(10, 40);
			this.textBox_ClientConfig.Name = "textBox_ClientConfig";
			this.textBox_ClientConfig.Size = new System.Drawing.Size(590, 21);
			this.textBox_ClientConfig.TabIndex = 2;
			// 
			// textBox_ServerSrc
			// 
			this.textBox_ServerSrc.Enabled = false;
			this.textBox_ServerSrc.Location = new System.Drawing.Point(10, 91);
			this.textBox_ServerSrc.Name = "textBox_ServerSrc";
			this.textBox_ServerSrc.Size = new System.Drawing.Size(590, 21);
			this.textBox_ServerSrc.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 12);
			this.label2.TabIndex = 11;
			this.label2.Text = "载入Excel文件:";
			// 
			// listBox_Sheets
			// 
			this.listBox_Sheets.FormattingEnabled = true;
			this.listBox_Sheets.ItemHeight = 12;
			this.listBox_Sheets.Location = new System.Drawing.Point(3, 66);
			this.listBox_Sheets.Name = "listBox_Sheets";
			this.listBox_Sheets.Size = new System.Drawing.Size(191, 400);
			this.listBox_Sheets.TabIndex = 13;
			this.listBox_Sheets.SelectedIndexChanged += new System.EventHandler(this.listBox_Sheets_SelectedIndexChanged);
			// 
			// textBox_ClientSrc
			// 
			this.textBox_ClientSrc.Enabled = false;
			this.textBox_ClientSrc.Location = new System.Drawing.Point(10, 88);
			this.textBox_ClientSrc.Name = "textBox_ClientSrc";
			this.textBox_ClientSrc.Size = new System.Drawing.Size(590, 21);
			this.textBox_ClientSrc.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 12);
			this.label3.TabIndex = 13;
			this.label3.Text = "C#代码文件:";
			// 
			// textBox_ServerConfig
			// 
			this.textBox_ServerConfig.Enabled = false;
			this.textBox_ServerConfig.Location = new System.Drawing.Point(10, 87);
			this.textBox_ServerConfig.Name = "textBox_ServerConfig";
			this.textBox_ServerConfig.Size = new System.Drawing.Size(590, 21);
			this.textBox_ServerConfig.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(95, 12);
			this.label4.TabIndex = 15;
			this.label4.Text = "服务器配置文件:";
			// 
			// checkBox_ExportConfig
			// 
			this.checkBox_ExportConfig.AutoSize = true;
			this.checkBox_ExportConfig.Enabled = false;
			this.checkBox_ExportConfig.Location = new System.Drawing.Point(492, 449);
			this.checkBox_ExportConfig.Name = "checkBox_ExportConfig";
			this.checkBox_ExportConfig.Size = new System.Drawing.Size(96, 16);
			this.checkBox_ExportConfig.TabIndex = 9;
			this.checkBox_ExportConfig.Text = "导出配置文件";
			this.checkBox_ExportConfig.UseVisualStyleBackColor = true;
			this.checkBox_ExportConfig.CheckedChanged += new System.EventHandler(this.checkBox_ExportConfig_CheckedChanged);
			// 
			// checkBox_ExportSrc
			// 
			this.checkBox_ExportSrc.AutoSize = true;
			this.checkBox_ExportSrc.Enabled = false;
			this.checkBox_ExportSrc.Location = new System.Drawing.Point(605, 449);
			this.checkBox_ExportSrc.Name = "checkBox_ExportSrc";
			this.checkBox_ExportSrc.Size = new System.Drawing.Size(84, 16);
			this.checkBox_ExportSrc.TabIndex = 10;
			this.checkBox_ExportSrc.Text = "导出源代码";
			this.checkBox_ExportSrc.UseVisualStyleBackColor = true;
			this.checkBox_ExportSrc.CheckedChanged += new System.EventHandler(this.checkBox_ExportSrc_CheckedChanged);
			// 
			// button_CheckData
			// 
			this.button_CheckData.Enabled = false;
			this.button_CheckData.Location = new System.Drawing.Point(210, 443);
			this.button_CheckData.Name = "button_CheckData";
			this.button_CheckData.Size = new System.Drawing.Size(89, 27);
			this.button_CheckData.TabIndex = 8;
			this.button_CheckData.Text = "检验数据(&V)";
			this.button_CheckData.UseVisualStyleBackColor = true;
			this.button_CheckData.Click += new System.EventHandler(this.button_CheckData_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(151, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(35, 12);
			this.label5.TabIndex = 17;
			this.label5.Text = "类名:";
			// 
			// textBox_ClassNameCS
			// 
			this.textBox_ClassNameCS.Enabled = false;
			this.textBox_ClassNameCS.Location = new System.Drawing.Point(192, 35);
			this.textBox_ClassNameCS.Name = "textBox_ClassNameCS";
			this.textBox_ClassNameCS.Size = new System.Drawing.Size(137, 21);
			this.textBox_ClassNameCS.TabIndex = 5;
			// 
			// numericUpDown_StepSize
			// 
			this.numericUpDown_StepSize.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numericUpDown_StepSize.Location = new System.Drawing.Point(534, 20);
			this.numericUpDown_StepSize.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
			this.numericUpDown_StepSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_StepSize.Name = "numericUpDown_StepSize";
			this.numericUpDown_StepSize.Size = new System.Drawing.Size(66, 21);
			this.numericUpDown_StepSize.TabIndex = 18;
			this.numericUpDown_StepSize.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBox_ManagerCS);
			this.groupBox1.Controls.Add(this.textBox_ClientSrc);
			this.groupBox1.Controls.Add(this.textBox_ClassNameCS);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(200, 187);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(606, 118);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "C#代码";
			// 
			// comboBox_ManagerCS
			// 
			this.comboBox_ManagerCS.FormattingEnabled = true;
			this.comboBox_ManagerCS.Location = new System.Drawing.Point(10, 35);
			this.comboBox_ManagerCS.Name = "comboBox_ManagerCS";
			this.comboBox_ManagerCS.Size = new System.Drawing.Size(132, 20);
			this.comboBox_ManagerCS.TabIndex = 23;
			// 
			// numericUpDown_KeyCount
			// 
			this.numericUpDown_KeyCount.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numericUpDown_KeyCount.Location = new System.Drawing.Point(396, 47);
			this.numericUpDown_KeyCount.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this.numericUpDown_KeyCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_KeyCount.Name = "numericUpDown_KeyCount";
			this.numericUpDown_KeyCount.Size = new System.Drawing.Size(66, 21);
			this.numericUpDown_KeyCount.TabIndex = 26;
			this.numericUpDown_KeyCount.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			// 
			// numericUpDown_GroupSize
			// 
			this.numericUpDown_GroupSize.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numericUpDown_GroupSize.Location = new System.Drawing.Point(534, 47);
			this.numericUpDown_GroupSize.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
			this.numericUpDown_GroupSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_GroupSize.Name = "numericUpDown_GroupSize";
			this.numericUpDown_GroupSize.Size = new System.Drawing.Size(66, 21);
			this.numericUpDown_GroupSize.TabIndex = 24;
			this.numericUpDown_GroupSize.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
			// 
			// numericUpDown_Capacity
			// 
			this.numericUpDown_Capacity.Increment = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this.numericUpDown_Capacity.Location = new System.Drawing.Point(396, 20);
			this.numericUpDown_Capacity.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this.numericUpDown_Capacity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_Capacity.Name = "numericUpDown_Capacity";
			this.numericUpDown_Capacity.Size = new System.Drawing.Size(66, 21);
			this.numericUpDown_Capacity.TabIndex = 21;
			this.numericUpDown_Capacity.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label_ClientTarget);
			this.groupBox2.Controls.Add(this.textBox_ClientConfig);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.textBox_ServerConfig);
			this.groupBox2.Location = new System.Drawing.Point(200, 67);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(606, 114);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "配置";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 12);
			this.label1.TabIndex = 21;
			this.label1.Text = "选择工作表:";
			// 
			// comboBox_ExcelList
			// 
			this.comboBox_ExcelList.FormattingEnabled = true;
			this.comboBox_ExcelList.Location = new System.Drawing.Point(105, 12);
			this.comboBox_ExcelList.Name = "comboBox_ExcelList";
			this.comboBox_ExcelList.Size = new System.Drawing.Size(692, 20);
			this.comboBox_ExcelList.TabIndex = 22;
			this.comboBox_ExcelList.SelectedIndexChanged += new System.EventHandler(this.comboBox_ExcelList_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboBox_ManagerCPP);
			this.groupBox3.Controls.Add(this.textBox_ClassNameCPP);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.numericUpDown_KeyCount);
			this.groupBox3.Controls.Add(this.label_ServerTarget);
			this.groupBox3.Controls.Add(this.textBox_ServerSrc);
			this.groupBox3.Controls.Add(this.numericUpDown_StepSize);
			this.groupBox3.Controls.Add(this.numericUpDown_Capacity);
			this.groupBox3.Controls.Add(this.numericUpDown_GroupSize);
			this.groupBox3.Location = new System.Drawing.Point(200, 311);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(606, 118);
			this.groupBox3.TabIndex = 23;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "C++代码";
			// 
			// comboBox_ManagerCPP
			// 
			this.comboBox_ManagerCPP.FormattingEnabled = true;
			this.comboBox_ManagerCPP.Location = new System.Drawing.Point(10, 35);
			this.comboBox_ManagerCPP.Name = "comboBox_ManagerCPP";
			this.comboBox_ManagerCPP.Size = new System.Drawing.Size(132, 20);
			this.comboBox_ManagerCPP.TabIndex = 23;
			this.comboBox_ManagerCPP.SelectedIndexChanged += new System.EventHandler(this.comboBox_ManagerCPP_SelectedIndexChanged);
			// 
			// textBox_ClassNameCPP
			// 
			this.textBox_ClassNameCPP.Enabled = false;
			this.textBox_ClassNameCPP.Location = new System.Drawing.Point(192, 35);
			this.textBox_ClassNameCPP.Name = "textBox_ClassNameCPP";
			this.textBox_ClassNameCPP.Size = new System.Drawing.Size(137, 21);
			this.textBox_ClassNameCPP.TabIndex = 5;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(151, 38);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(35, 12);
			this.label14.TabIndex = 17;
			this.label14.Text = "类名:";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(809, 661);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.comboBox_ExcelList);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button_Export);
			this.Controls.Add(this.checkBox_ExportSrc);
			this.Controls.Add(this.checkBox_ExportConfig);
			this.Controls.Add(this.listBox_Sheets);
			this.Controls.Add(this.button_CheckData);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.button_OpenExcel);
			this.Controls.Add(this.groupBox_Log);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			//this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "配置导出工具";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.groupBox_Log.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_StepSize)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_KeyCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_GroupSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Capacity)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Log;
        private System.Windows.Forms.Button button_OpenExcel;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.Label label_ClientTarget;
        private System.Windows.Forms.Label label_ServerTarget;
        private System.Windows.Forms.TextBox textBox_ClientConfig;
        private System.Windows.Forms.TextBox textBox_ServerSrc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox_Sheets;
        private System.Windows.Forms.TextBox textBox_ClientSrc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ServerConfig;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richTextBox_Log;
        private System.Windows.Forms.CheckBox checkBox_ExportConfig;
        private System.Windows.Forms.CheckBox checkBox_ExportSrc;
        private System.Windows.Forms.Button button_CheckData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_ClassNameCS;
		private System.Windows.Forms.NumericUpDown numericUpDown_StepSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox_ExcelList;
        private System.Windows.Forms.NumericUpDown numericUpDown_Capacity;
		private System.Windows.Forms.ComboBox comboBox_ManagerCS;
		private System.Windows.Forms.NumericUpDown numericUpDown_KeyCount;
        private System.Windows.Forms.NumericUpDown numericUpDown_GroupSize;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox_ManagerCPP;
        private System.Windows.Forms.TextBox textBox_ClassNameCPP;
        private System.Windows.Forms.Label label14;
    }
}

