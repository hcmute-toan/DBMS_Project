namespace LaptopShopProject.Forms
{
    partial class ImportManagementForm
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
            txtCategory = new TextBox();
            txtCategoryDescription = new TextBox();
            lblTotalAmount = new Label();
            lbProductName = new Label();
            lbQuantity = new Label();
            lbUnitPrice = new Label();
            lbPrice = new Label();
            lbCategory = new Label();
            lbCategoryDescription = new Label();
            lbSupplier = new Label();
            lbImportDate = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)dgvImportDetails).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvImports).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();

            // btnRefresh
            btnRefresh.Location = new Point(877, 888);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 17;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;

            // btnUpdate
            btnUpdate.Location = new Point(560, 888);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 16;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;

            // btnDelete
            btnDelete.Location = new Point(719, 888);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 15;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;

            // btnAdd
            btnAdd.Location = new Point(400, 888);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 31);
            btnAdd.TabIndex = 14;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;

            // cboSupplier
            cboSupplier.FormattingEnabled = true;
            cboSupplier.Location = new Point(212, 448);
            cboSupplier.Margin = new Padding(3, 4, 3, 4);
            cboSupplier.Name = "cboSupplier";
            cboSupplier.Size = new Size(249, 28);
            cboSupplier.TabIndex = 13;

            // txtQuantity
            txtQuantity.Location = new Point(212, 105);
            txtQuantity.Margin = new Padding(3, 4, 3, 4);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(249, 27);
            txtQuantity.TabIndex = 12;

            // txtProductName
            txtProductName.Location = new Point(212, 28);
            txtProductName.Margin = new Padding(3, 4, 3, 4);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(249, 27);
            txtProductName.TabIndex = 11;

            // dgvImportDetails
            dgvImportDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvImportDetails.Location = new Point(495, 483);
            dgvImportDetails.Margin = new Padding(3, 4, 3, 4);
            dgvImportDetails.Name = "dgvImportDetails";
            dgvImportDetails.RowHeadersWidth = 51;
            dgvImportDetails.Size = new Size(488, 264);
            dgvImportDetails.TabIndex = 10;

            // dgvImports
            dgvImports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvImports.Location = new Point(495, 28);
            dgvImports.Margin = new Padding(3, 4, 3, 4);
            dgvImports.Name = "dgvImports";
            dgvImports.RowHeadersWidth = 51;
            dgvImports.Size = new Size(488, 260);
            dgvImports.TabIndex = 9;

            // dtpImportDate
            dtpImportDate.Location = new Point(212, 544);
            dtpImportDate.Margin = new Padding(3, 4, 3, 4);
            dtpImportDate.Name = "dtpImportDate";
            dtpImportDate.Size = new Size(249, 27);
            dtpImportDate.TabIndex = 18;

            // txtUnitPrice
            txtUnitPrice.Location = new Point(212, 188);
            txtUnitPrice.Margin = new Padding(3, 4, 3, 4);
            txtUnitPrice.Name = "txtUnitPrice";
            txtUnitPrice.Size = new Size(249, 27);
            txtUnitPrice.TabIndex = 19;

            // txtPrice
            txtPrice.Location = new Point(212, 277);
            txtPrice.Margin = new Padding(3, 4, 3, 4);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(249, 27);
            txtPrice.TabIndex = 20;

            // txtCategory
            txtCategory.Location = new Point(212, 360);
            txtCategory.Margin = new Padding(3, 4, 3, 4);
            txtCategory.Name = "txtCategory";
            txtCategory.Size = new Size(249, 27);
            txtCategory.TabIndex = 21;

            // txtCategoryDescription
            txtCategoryDescription.Location = new Point(212, 404);
            txtCategoryDescription.Margin = new Padding(3, 4, 3, 4);
            txtCategoryDescription.Name = "txtCategoryDescription";
            txtCategoryDescription.Size = new Size(249, 27);
            txtCategoryDescription.TabIndex = 22;

            // lblTotalAmount
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.Location = new Point(24, 667);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(135, 23);
            lblTotalAmount.TabIndex = 23;
            lblTotalAmount.Text = "Total Amount: ";

            // lbProductName
            lbProductName.AutoSize = true;
            lbProductName.Location = new Point(48, 35);
            lbProductName.Name = "lbProductName";
            lbProductName.Size = new Size(111, 20);
            lbProductName.TabIndex = 24;
            lbProductName.Text = "Product Name :";

            // lbQuantity
            lbQuantity.AutoSize = true;
            lbQuantity.Location = new Point(87, 108);
            lbQuantity.Name = "lbQuantity";
            lbQuantity.Size = new Size(72, 20);
            lbQuantity.TabIndex = 25;
            lbQuantity.Text = "Quantity :";

            // lbUnitPrice
            lbUnitPrice.AutoSize = true;
            lbUnitPrice.Location = new Point(80, 195);
            lbUnitPrice.Name = "lbUnitPrice";
            lbUnitPrice.Size = new Size(79, 20);
            lbUnitPrice.TabIndex = 26;
            lbUnitPrice.Text = "Unit Price :";

            // lbPrice
            lbPrice.AutoSize = true;
            lbPrice.Location = new Point(111, 280);
            lbPrice.Name = "lbPrice";
            lbPrice.Size = new Size(48, 20);
            lbPrice.TabIndex = 27;
            lbPrice.Text = "Price :";

            // lbCategory
            lbCategory.AutoSize = true;
            lbCategory.Location = new Point(92, 363);
            lbCategory.Name = "lbCategory";
            lbCategory.Size = new Size(67, 20);
            lbCategory.TabIndex = 28;
            lbCategory.Text = "Category :";

            // lbCategoryDescription
            lbCategoryDescription.AutoSize = true;
            lbCategoryDescription.Location = new Point(24, 407);
            lbCategoryDescription.Name = "lbCategoryDescription";
            lbCategoryDescription.Size = new Size(135, 20);
            lbCategoryDescription.TabIndex = 29;
            lbCategoryDescription.Text = "Category Description :";

            // lbSupplier
            lbSupplier.AutoSize = true;
            lbSupplier.Location = new Point(88, 451);
            lbSupplier.Name = "lbSupplier";
            lbSupplier.Size = new Size(71, 20);
            lbSupplier.TabIndex = 30;
            lbSupplier.Text = "Supplier :";

            // lbImportDate
            lbImportDate.AutoSize = true;
            lbImportDate.Location = new Point(62, 547);
            lbImportDate.Name = "lbImportDate";
            lbImportDate.Size = new Size(97, 20);
            lbImportDate.TabIndex = 31;
            lbImportDate.Text = "Import Date :";

            // pictureBox1
            pictureBox1.Image = Properties.Resources.ImportExport1;
            pictureBox1.Location = new Point(39, 795);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(120, 124);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 32;
            pictureBox1.TabStop = false;

            // ImportManagementForm
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(pictureBox1);
            Controls.Add(lbImportDate);
            Controls.Add(lbSupplier);
            Controls.Add(lbCategoryDescription);
            Controls.Add(lbCategory);
            Controls.Add(lbPrice);
            Controls.Add(lbUnitPrice);
            Controls.Add(lbQuantity);
            Controls.Add(lbProductName);
            Controls.Add(lblTotalAmount);
            Controls.Add(txtCategoryDescription);
            Controls.Add(txtCategory);
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
            Size = new Size(1030, 948);
            ((System.ComponentModel.ISupportInitialize)dgvImportDetails).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvImports).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

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
        private TextBox txtCategory;
        private TextBox txtCategoryDescription;
        private Label lblTotalAmount;
        private Label lbProductName;
        private Label lbQuantity;
        private Label lbUnitPrice;
        private Label lbPrice;
        private Label lbCategory;
        private Label lbCategoryDescription;
        private Label lbSupplier;
        private Label lbImportDate;
        private PictureBox pictureBox1;
    }
}