namespace LaptopShopProject.Forms
{
    partial class ImportManagementForm
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
            cboSupplier = new ComboBox();
            txtQuantity = new TextBox();
            txtProductName = new TextBox();
            dgvImportDetails = new DataGridView();
            dgvImports = new DataGridView();
            dtpImportDate = new DateTimePicker();
            txtUnitPrice = new TextBox();
            txtPrice = new TextBox();
            lblTotalAmount = new Label();
            lbProductName = new Label();
            lbQuantity = new Label();
            lbUnitPrice = new Label();
            lbPrice = new Label();
            lbSupplier = new Label();
            lbImportDate = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvImportDetails).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvImports).BeginInit();
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
            // cboSupplier
            // 
            cboSupplier.FormattingEnabled = true;
            cboSupplier.Location = new Point(212, 368);
            cboSupplier.Margin = new Padding(3, 4, 3, 4);
            cboSupplier.Name = "cboSupplier";
            cboSupplier.Size = new Size(249, 28);
            cboSupplier.TabIndex = 13;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(212, 105);
            txtQuantity.Margin = new Padding(3, 4, 3, 4);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(249, 27);
            txtQuantity.TabIndex = 12;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(212, 28);
            txtProductName.Margin = new Padding(3, 4, 3, 4);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(249, 27);
            txtProductName.TabIndex = 11;
            // 
            // dgvImportDetails
            // 
            dgvImportDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvImportDetails.Location = new Point(495, 403);
            dgvImportDetails.Margin = new Padding(3, 4, 3, 4);
            dgvImportDetails.Name = "dgvImportDetails";
            dgvImportDetails.RowHeadersWidth = 51;
            dgvImportDetails.Size = new Size(488, 264);
            dgvImportDetails.TabIndex = 10;
            // 
            // dgvImports
            // 
            dgvImports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvImports.Location = new Point(495, 28);
            dgvImports.Margin = new Padding(3, 4, 3, 4);
            dgvImports.Name = "dgvImports";
            dgvImports.RowHeadersWidth = 51;
            dgvImports.Size = new Size(488, 260);
            dgvImports.TabIndex = 9;
            // 
            // dtpImportDate
            // 
            dtpImportDate.Location = new Point(212, 464);
            dtpImportDate.Margin = new Padding(3, 4, 3, 4);
            dtpImportDate.Name = "dtpImportDate";
            dtpImportDate.Size = new Size(249, 27);
            dtpImportDate.TabIndex = 18;
            // 
            // txtUnitPrice
            // 
            txtUnitPrice.Location = new Point(212, 188);
            txtUnitPrice.Margin = new Padding(3, 4, 3, 4);
            txtUnitPrice.Name = "txtUnitPrice";
            txtUnitPrice.Size = new Size(249, 27);
            txtUnitPrice.TabIndex = 19;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(212, 277);
            txtPrice.Margin = new Padding(3, 4, 3, 4);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(249, 27);
            txtPrice.TabIndex = 20;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.Location = new Point(24, 587);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(135, 23);
            lblTotalAmount.TabIndex = 21;
            lblTotalAmount.Text = "Total Amount: ";
            // 
            // lbProductName
            // 
            lbProductName.AutoSize = true;
            lbProductName.Location = new Point(48, 35);
            lbProductName.Name = "lbProductName";
            lbProductName.Size = new Size(111, 20);
            lbProductName.TabIndex = 22;
            lbProductName.Text = "Product Name :";
            // 
            // lbQuantity
            // 
            lbQuantity.AutoSize = true;
            lbQuantity.Location = new Point(87, 108);
            lbQuantity.Name = "lbQuantity";
            lbQuantity.Size = new Size(72, 20);
            lbQuantity.TabIndex = 23;
            lbQuantity.Text = "Quantity :";
            // 
            // lbUnitPrice
            // 
            lbUnitPrice.AutoSize = true;
            lbUnitPrice.Location = new Point(80, 195);
            lbUnitPrice.Name = "lbUnitPrice";
            lbUnitPrice.Size = new Size(79, 20);
            lbUnitPrice.TabIndex = 24;
            lbUnitPrice.Text = "Unit Price :";
            // 
            // lbPrice
            // 
            lbPrice.AutoSize = true;
            lbPrice.Location = new Point(111, 280);
            lbPrice.Name = "lbPrice";
            lbPrice.Size = new Size(48, 20);
            lbPrice.TabIndex = 25;
            lbPrice.Text = "Price :";
            // 
            // lbSupplier
            // 
            lbSupplier.AutoSize = true;
            lbSupplier.Location = new Point(88, 371);
            lbSupplier.Name = "lbSupplier";
            lbSupplier.Size = new Size(71, 20);
            lbSupplier.TabIndex = 26;
            lbSupplier.Text = "Supplier :";
            // 
            // lbImportDate
            // 
            lbImportDate.AutoSize = true;
            lbImportDate.Location = new Point(62, 469);
            lbImportDate.Name = "lbImportDate";
            lbImportDate.Size = new Size(97, 20);
            lbImportDate.TabIndex = 27;
            lbImportDate.Text = "Import Date :";
            // 
            // ImportManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(lbImportDate);
            Controls.Add(lbSupplier);
            Controls.Add(lbPrice);
            Controls.Add(lbUnitPrice);
            Controls.Add(lbQuantity);
            Controls.Add(lbProductName);
            Controls.Add(lblTotalAmount);
            Controls.Add(txtPrice);
            Controls.Add(txtUnitPrice);
            Controls.Add(dtpImportDate);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(cboSupplier);
            Controls.Add(txtQuantity);
            Controls.Add(txtProductName);
            Controls.Add(dgvImportDetails);
            Controls.Add(dgvImports);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ImportManagementForm";
            Size = new Size(1030, 868);
            ((System.ComponentModel.ISupportInitialize)dgvImportDetails).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvImports).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRefresh;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnAdd;
        private ComboBox cboSupplier;
        private TextBox txtQuantity;
        private TextBox txtProductName;
        private DataGridView dgvImportDetails;
        private DataGridView dgvImports;
        private DateTimePicker dtpImportDate;
        private TextBox txtUnitPrice;
        private TextBox txtPrice;
        private Label lblTotalAmount;
        private Label lbProductName;
        private Label lbQuantity;
        private Label lbUnitPrice;
        private Label lbPrice;
        private Label lbSupplier;
        private Label lbImportDate;
    }
}
