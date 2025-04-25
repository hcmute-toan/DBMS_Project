namespace LaptopShopProject.Forms
{
    partial class ReportForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            dgvInventory = new TabPage();
            dgvImportReport = new TabPage();
            dgvExportReport = new TabPage();
            btnRefresh = new Button();
            btnExportToExcel = new Button();
            dgvChartReport = new TabPage();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(dgvInventory);
            tabControl1.Controls.Add(dgvImportReport);
            tabControl1.Controls.Add(dgvExportReport);
            tabControl1.Controls.Add(dgvChartReport);
            tabControl1.Location = new Point(3, 18);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(711, 303);
            tabControl1.TabIndex = 0;
            // 
            // dgvInventory
            // 
            dgvInventory.Location = new Point(4, 24);
            dgvInventory.Name = "dgvInventory";
            dgvInventory.Padding = new Padding(3);
            dgvInventory.Size = new Size(703, 275);
            dgvInventory.TabIndex = 0;
            dgvInventory.Text = "Tab Inventory";
            dgvInventory.UseVisualStyleBackColor = true;
            // 
            // dgvImportReport
            // 
            dgvImportReport.Location = new Point(4, 24);
            dgvImportReport.Name = "dgvImportReport";
            dgvImportReport.Padding = new Padding(3);
            dgvImportReport.Size = new Size(703, 275);
            dgvImportReport.TabIndex = 1;
            dgvImportReport.Text = "Tab Imports";
            dgvImportReport.UseVisualStyleBackColor = true;
            // 
            // dgvExportReport
            // 
            dgvExportReport.Location = new Point(4, 24);
            dgvExportReport.Name = "dgvExportReport";
            dgvExportReport.Padding = new Padding(3);
            dgvExportReport.Size = new Size(703, 275);
            dgvExportReport.TabIndex = 2;
            dgvExportReport.Text = "Tab Exports";
            dgvExportReport.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(382, 362);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(109, 27);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Location = new Point(545, 362);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new Size(165, 27);
            btnExportToExcel.TabIndex = 2;
            btnExportToExcel.Text = "Export to Excel";
            btnExportToExcel.UseVisualStyleBackColor = true;
            // 
            // dgvChartReport
            // 
            dgvChartReport.Location = new Point(4, 24);
            dgvChartReport.Name = "dgvChartReport";
            dgvChartReport.Padding = new Padding(3);
            dgvChartReport.Size = new Size(703, 275);
            dgvChartReport.TabIndex = 3;
            dgvChartReport.Text = "Tab Chart";
            dgvChartReport.UseVisualStyleBackColor = true;
            // 
            // ReportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(btnExportToExcel);
            Controls.Add(btnRefresh);
            Controls.Add(tabControl1);
            Name = "ReportForm";
            Size = new Size(730, 427);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage dgvInventory;
        private TabPage dgvImportReport;
        private TabPage dgvExportReport;
        private Button btnRefresh;
        private Button btnExportToExcel;
        private TabPage dgvChartReport;
    }
}
