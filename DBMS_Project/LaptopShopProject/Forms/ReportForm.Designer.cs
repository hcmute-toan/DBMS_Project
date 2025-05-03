namespace LaptopShopProject.Forms
{
    partial class ReportForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.tabImports = new System.Windows.Forms.TabPage();
            this.tabExports = new System.Windows.Forms.TabPage();
            this.tabRevenue = new System.Windows.Forms.TabPage();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();

            // tabControl1
            this.tabControl1.Controls.Add(this.tabInventory);
            this.tabControl1.Controls.Add(this.tabImports);
            this.tabControl1.Controls.Add(this.tabExports);
            this.tabControl1.Controls.Add(this.tabRevenue);
            this.tabControl1.Location = new System.Drawing.Point(10, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 400);
            this.tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;

            // tabInventory
            this.tabInventory.Location = new System.Drawing.Point(4, 22);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Text = "Inventory Report";
            this.tabInventory.UseVisualStyleBackColor = true;

            // tabImports
            this.tabImports.Location = new System.Drawing.Point(4, 22);
            this.tabImports.Name = "tabImports";
            this.tabImports.Text = "Import Report";
            this.tabImports.UseVisualStyleBackColor = true;

            // tabExports
            this.tabExports.Location = new System.Drawing.Point(4, 22);
            this.tabExports.Name = "tabExports";
            this.tabExports.Text = "Export Report";
            this.tabExports.UseVisualStyleBackColor = true;

            // tabRevenue
            this.tabRevenue.Location = new System.Drawing.Point(4, 22);
            this.tabRevenue.Name = "tabRevenue";
            this.tabRevenue.Text = "Revenue Report";
            this.tabRevenue.UseVisualStyleBackColor = true;

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(10, 420);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;

            // btnExportToExcel
            this.btnExportToExcel.Location = new System.Drawing.Point(120, 420);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(100, 30);
            this.btnExportToExcel.Text = "Export to Excel";
            this.btnExportToExcel.UseVisualStyleBackColor = true;

            // ReportForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnExportToExcel);
            this.Name = "ReportForm";
            this.Size = new System.Drawing.Size(780, 460);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInventory;
        private System.Windows.Forms.TabPage tabImports;
        private System.Windows.Forms.TabPage tabExports;
        private System.Windows.Forms.TabPage tabRevenue;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExportToExcel;
    }
}