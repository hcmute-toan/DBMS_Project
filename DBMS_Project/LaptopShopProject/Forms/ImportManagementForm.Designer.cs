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
            ((System.ComponentModel.ISupportInitialize)dgvImportDetails).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvImports).BeginInit();
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
            // cboSupplier
            // 
            cboSupplier.FormattingEnabled = true;
            cboSupplier.Location = new Point(40, 276);
            cboSupplier.Name = "cboSupplier";
            cboSupplier.Size = new Size(218, 23);
            cboSupplier.TabIndex = 13;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(40, 79);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(218, 23);
            txtQuantity.TabIndex = 12;
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(40, 21);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(218, 23);
            txtProductName.TabIndex = 11;
            // 
            // dgvImportDetails
            // 
            dgvImportDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvImportDetails.Location = new Point(350, 302);
            dgvImportDetails.Name = "dgvImportDetails";
            dgvImportDetails.Size = new Size(510, 263);
            dgvImportDetails.TabIndex = 10;
            // 
            // dgvImports
            // 
            dgvImports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvImports.Location = new Point(350, 21);
            dgvImports.Name = "dgvImports";
            dgvImports.Size = new Size(510, 251);
            dgvImports.TabIndex = 9;
            // 
            // dtpImportDate
            // 
            dtpImportDate.Location = new Point(40, 348);
            dtpImportDate.Name = "dtpImportDate";
            dtpImportDate.Size = new Size(218, 23);
            dtpImportDate.TabIndex = 18;
            // 
            // txtUnitPrice
            // 
            txtUnitPrice.Location = new Point(40, 141);
            txtUnitPrice.Name = "txtUnitPrice";
            txtUnitPrice.Size = new Size(218, 23);
            txtUnitPrice.TabIndex = 19;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(40, 208);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(218, 23);
            txtPrice.TabIndex = 20;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.Location = new Point(40, 422);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(107, 19);
            lblTotalAmount.TabIndex = 21;
            lblTotalAmount.Text = "Total Amount: ";
            // 
            // ImportManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
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
            Name = "ImportManagementForm";
            Size = new Size(901, 651);
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
    }
}
