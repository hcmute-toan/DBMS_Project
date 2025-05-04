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
            tabControl1 = new TabControl();
            tabInventory = new TabPage();
            tabImports = new TabPage();
            tabExports = new TabPage();
            tabRevenue = new TabPage();
            btnRefresh = new Button();
            btnExportToExcel = new Button();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabInventory);
            tabControl1.Controls.Add(tabImports);
            tabControl1.Controls.Add(tabExports);
            tabControl1.Controls.Add(tabRevenue);
            tabControl1.Location = new Point(13, 15);
            tabControl1.Margin = new Padding(4, 5, 4, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1013, 453);
            tabControl1.TabIndex = 0;
            // 
            // tabInventory
            // 
            tabInventory.Location = new Point(4, 29);
            tabInventory.Margin = new Padding(4, 5, 4, 5);
            tabInventory.Name = "tabInventory";
            tabInventory.Size = new Size(1005, 420);
            tabInventory.TabIndex = 0;
            tabInventory.Text = "Inventory Report";
            tabInventory.UseVisualStyleBackColor = true;
            tabInventory.Click += tabInventory_Click;
            // 
            // tabImports
            // 
            tabImports.Location = new Point(4, 29);
            tabImports.Margin = new Padding(4, 5, 4, 5);
            tabImports.Name = "tabImports";
            tabImports.Size = new Size(1005, 479);
            tabImports.TabIndex = 1;
            tabImports.Text = "Import Report";
            tabImports.UseVisualStyleBackColor = true;
            // 
            // tabExports
            // 
            tabExports.Location = new Point(4, 29);
            tabExports.Margin = new Padding(4, 5, 4, 5);
            tabExports.Name = "tabExports";
            tabExports.Size = new Size(1005, 479);
            tabExports.TabIndex = 2;
            tabExports.Text = "Export Report";
            tabExports.UseVisualStyleBackColor = true;
            // 
            // tabRevenue
            // 
            tabRevenue.Location = new Point(4, 29);
            tabRevenue.Margin = new Padding(4, 5, 4, 5);
            tabRevenue.Name = "tabRevenue";
            tabRevenue.Size = new Size(1005, 479);
            tabRevenue.TabIndex = 3;
            tabRevenue.Text = "Revenue Report";
            tabRevenue.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(13, 646);
            btnRefresh.Margin = new Padding(4, 5, 4, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(133, 46);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Location = new Point(160, 646);
            btnExportToExcel.Margin = new Padding(4, 5, 4, 5);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new Size(133, 46);
            btnExportToExcel.TabIndex = 2;
            btnExportToExcel.Text = "Export to Excel";
            btnExportToExcel.UseVisualStyleBackColor = true;
            // 
            // ReportForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Controls.Add(btnRefresh);
            Controls.Add(btnExportToExcel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "ReportForm";
            Size = new Size(1040, 708);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
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