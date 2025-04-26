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
            txtBranch = new TextBox();
            lbProductName = new Label();
            lbPrice = new Label();
            lbStockQuantity = new Label();
            lbBranch = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvProductLogs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(877, 808);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 17;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(560, 808);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 16;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(719, 808);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 15;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(400, 808);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 31);
            btnAdd.TabIndex = 14;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(169, 149);
            txtPrice.Margin = new Padding(3, 4, 3, 4);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(249, 27);
            txtPrice.TabIndex = 12;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(169, 49);
            txtProductName.Margin = new Padding(3, 4, 3, 4);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(249, 27);
            txtProductName.TabIndex = 11;
            // 
            // dgvProductLogs
            // 
            dgvProductLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductLogs.Location = new Point(457, 403);
            dgvProductLogs.Margin = new Padding(3, 4, 3, 4);
            dgvProductLogs.Name = "dgvProductLogs";
            dgvProductLogs.RowHeadersWidth = 51;
            dgvProductLogs.Size = new Size(526, 292);
            dgvProductLogs.TabIndex = 10;
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Location = new Point(457, 48);
            dgvProducts.Margin = new Padding(3, 4, 3, 4);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 51;
            dgvProducts.Size = new Size(526, 280);
            dgvProducts.TabIndex = 9;
            // 
            // txtStockQuantity
            // 
            txtStockQuantity.Location = new Point(169, 265);
            txtStockQuantity.Margin = new Padding(3, 4, 3, 4);
            txtStockQuantity.Name = "txtStockQuantity";
            txtStockQuantity.Size = new Size(249, 27);
            txtStockQuantity.TabIndex = 18;
            // 
            // txtBranch
            // 
            txtBranch.Location = new Point(169, 373);
            txtBranch.Margin = new Padding(3, 4, 3, 4);
            txtBranch.Name = "txtBranch";
            txtBranch.Size = new Size(249, 27);
            txtBranch.TabIndex = 19;
            // 
            // lbProductName
            // 
            lbProductName.AutoSize = true;
            lbProductName.Location = new Point(18, 52);
            lbProductName.Name = "lbProductName";
            lbProductName.Size = new Size(108, 20);
            lbProductName.TabIndex = 20;
            lbProductName.Text = "Product name :";
            // 
            // lbPrice
            // 
            lbPrice.AutoSize = true;
            lbPrice.Location = new Point(76, 152);
            lbPrice.Name = "lbPrice";
            lbPrice.Size = new Size(48, 20);
            lbPrice.TabIndex = 21;
            lbPrice.Text = "Price :";
            // 
            // lbStockQuantity
            // 
            lbStockQuantity.AutoSize = true;
            lbStockQuantity.Location = new Point(14, 268);
            lbStockQuantity.Name = "lbStockQuantity";
            lbStockQuantity.Size = new Size(112, 20);
            lbStockQuantity.TabIndex = 22;
            lbStockQuantity.Text = "Stock Quantity :";
            // 
            // lbBranch
            // 
            lbBranch.AutoSize = true;
            lbBranch.Location = new Point(63, 380);
            lbBranch.Name = "lbBranch";
            lbBranch.Size = new Size(61, 20);
            lbBranch.TabIndex = 23;
            lbBranch.Text = "Branch :";
            // 
            // ProductManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(lbBranch);
            Controls.Add(lbStockQuantity);
            Controls.Add(lbPrice);
            Controls.Add(lbProductName);
            Controls.Add(txtBranch);
            Controls.Add(txtStockQuantity);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(txtPrice);
            Controls.Add(txtProductName);
            Controls.Add(dgvProductLogs);
            Controls.Add(dgvProducts);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ProductManagementForm";
            Size = new Size(1030, 868);
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
        private TextBox txtBranch;
        private Label lbProductName;
        private Label lbPrice;
        private Label lbStockQuantity;
        private Label lbBranch;
    }
}
