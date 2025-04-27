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
            btnRefresh = new Button();
            btnExportToExcel = new Button();
            dgvExportReport = new TabPage();
            dgvImportReport = new TabPage();
            dgvInventory = new TabPage();
            tabControl1 = new TabControl();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(437, 483);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(125, 36);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Location = new Point(623, 483);
            btnExportToExcel.Margin = new Padding(3, 4, 3, 4);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new Size(189, 36);
            btnExportToExcel.TabIndex = 2;
            btnExportToExcel.Text = "Export to Excel";
            btnExportToExcel.UseVisualStyleBackColor = true;
            // 
            // dgvExportReport
            // 
            dgvExportReport.Location = new Point(4, 29);
            dgvExportReport.Margin = new Padding(3, 4, 3, 4);
            dgvExportReport.Name = "dgvExportReport";
            dgvExportReport.Padding = new Padding(3, 4, 3, 4);
            dgvExportReport.Size = new Size(805, 371);
            dgvExportReport.TabIndex = 2;
            dgvExportReport.Text = "Tab Exports";
            dgvExportReport.UseVisualStyleBackColor = true;
            // 
            // dgvImportReport
            // 
            dgvImportReport.Location = new Point(4, 29);
            dgvImportReport.Margin = new Padding(3, 4, 3, 4);
            dgvImportReport.Name = "dgvImportReport";
            dgvImportReport.Padding = new Padding(3, 4, 3, 4);
            dgvImportReport.Size = new Size(805, 371);
            dgvImportReport.TabIndex = 1;
            dgvImportReport.Text = "Tab Imports";
            dgvImportReport.UseVisualStyleBackColor = true;
            // 
            // dgvInventory
            // 
            dgvInventory.Location = new Point(4, 29);
            dgvInventory.Margin = new Padding(3, 4, 3, 4);
            dgvInventory.Name = "dgvInventory";
            dgvInventory.Padding = new Padding(3, 4, 3, 4);
            dgvInventory.Size = new Size(805, 371);
            dgvInventory.TabIndex = 0;
            dgvInventory.Text = "Tab Inventory";
            dgvInventory.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(dgvInventory);
            tabControl1.Controls.Add(dgvImportReport);
            tabControl1.Controls.Add(dgvExportReport);
            tabControl1.Location = new Point(3, 24);
            tabControl1.Margin = new Padding(3, 4, 3, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(813, 404);
            tabControl1.TabIndex = 0;
            // 
            // ReportForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(btnExportToExcel);
            Controls.Add(btnRefresh);
            Controls.Add(tabControl1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ReportForm";
            Size = new Size(834, 569);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnRefresh;
        private Button btnExportToExcel;
        private TabPage dgvExportReport;
        private TabPage dgvImportReport;
        private TabPage dgvInventory;
        private TabControl tabControl1;
    }
}
