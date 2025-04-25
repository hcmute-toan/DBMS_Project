namespace LaptopShopProject.Forms
{
    partial class ProductManagementForm
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
            btnUpdate = new Button();
            btnDelete = new Button();
            btnAdd = new Button();
            txtPrice = new TextBox();
            txtProductName = new TextBox();
            dgvProductLogs = new DataGridView();
            dgvProducts = new DataGridView();
            txtStockQuantity = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvProductLogs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(767, 606);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(93, 23);
            btnRefresh.TabIndex = 17;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(490, 606);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(93, 23);
            btnUpdate.TabIndex = 16;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(629, 606);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(89, 23);
            btnDelete.TabIndex = 15;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(350, 606);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(93, 23);
            btnAdd.TabIndex = 14;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(40, 96);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(218, 23);
            txtPrice.TabIndex = 12;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(40, 21);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(218, 23);
            txtProductName.TabIndex = 11;
            // 
            // dgvProductLogs
            // 
            dgvProductLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductLogs.Location = new Point(350, 302);
            dgvProductLogs.Name = "dgvProductLogs";
            dgvProductLogs.Size = new Size(510, 263);
            dgvProductLogs.TabIndex = 10;
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new Point(350, 21);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.Size = new Size(510, 251);
            dgvProducts.TabIndex = 9;
            // 
            // txtStockQuantity
            // 
            txtStockQuantity.Location = new Point(40, 183);
            txtStockQuantity.Name = "txtStockQuantity";
            txtStockQuantity.Size = new Size(218, 23);
            txtStockQuantity.TabIndex = 18;
            // 
            // ProductManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(txtStockQuantity);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(txtPrice);
            Controls.Add(txtProductName);
            Controls.Add(dgvProductLogs);
            Controls.Add(dgvProducts);
            Name = "ProductManagementForm";
            Size = new Size(901, 651);
            ((System.ComponentModel.ISupportInitialize)dgvProductLogs).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRefresh;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnAdd;
        private TextBox txtPrice;
        private TextBox txtProductName;
        private DataGridView dgvProductLogs;
        private DataGridView dgvProducts;
        private TextBox txtStockQuantity;
    }
}
