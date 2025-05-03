
namespace LaptopShopProject.Forms
{
    partial class ExportManagementForm
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
            dtpExportDate = new DateTimePicker();
            btnRefresh = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnAdd = new Button();
            cboCustomer = new ComboBox();
            txtQuantity = new TextBox();
            txtUnitPrice = new TextBox();
            dgvExportDetails = new DataGridView();
            dgvExports = new DataGridView();
            lblTotalAmount = new Label();
            lbQuantity = new Label();
            lbUnitPrice = new Label();
            lbCustomer = new Label();
            lbExportDate = new Label();
            lbProductName = new Label();
            txtProductName = new TextBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)dgvExportDetails).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvExports).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // dtpExportDate
            // 
            dtpExportDate.Location = new Point(190, 425);
            dtpExportDate.Margin = new Padding(3, 4, 3, 4);
            dtpExportDate.Name = "dtpExportDate";
            dtpExportDate.Size = new Size(249, 27);
            dtpExportDate.TabIndex = 30;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(898, 748);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 29;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(581, 748);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 28;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(740, 748);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 27;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(421, 748);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 31);
            btnAdd.TabIndex = 26;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // cboCustomer
            // 
            cboCustomer.FormattingEnabled = true;
            cboCustomer.Location = new Point(190, 322);
            cboCustomer.Margin = new Padding(3, 4, 3, 4);
            cboCustomer.Name = "cboCustomer";
            cboCustomer.Size = new Size(249, 28);
            cboCustomer.TabIndex = 25;
            cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(190, 121);
            txtQuantity.Margin = new Padding(3, 4, 3, 4);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(249, 27);
            txtQuantity.TabIndex = 24;
            // 
            // txtUnitPrice
            // 
            txtUnitPrice.Location = new Point(190, 217);
            txtUnitPrice.Margin = new Padding(3, 4, 3, 4);
            txtUnitPrice.Name = "txtUnitPrice";
            txtUnitPrice.Size = new Size(249, 27);
            txtUnitPrice.TabIndex = 23;
            // 
            // dgvExportDetails
            // 
            dgvExportDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExportDetails.Location = new Point(465, 403);
            dgvExportDetails.Margin = new Padding(3, 4, 3, 4);
            dgvExportDetails.Name = "dgvExportDetails";
            dgvExportDetails.RowHeadersWidth = 51;
            dgvExportDetails.Size = new Size(518, 291);
            dgvExportDetails.TabIndex = 22;
            // 
            // dgvExports
            // 
            dgvExports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExports.Location = new Point(465, 28);
            dgvExports.Margin = new Padding(3, 4, 3, 4);
            dgvExports.Name = "dgvExports";
            dgvExports.RowHeadersWidth = 51;
            dgvExports.Size = new Size(518, 282);
            dgvExports.TabIndex = 21;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.Location = new Point(69, 569);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(135, 23);
            lblTotalAmount.TabIndex = 31;
            lblTotalAmount.Text = "Total Amount: ";
            // 
            // lbQuantity
            // 
            lbQuantity.AutoSize = true;
            lbQuantity.Location = new Point(82, 124);
            lbQuantity.Name = "lbQuantity";
            lbQuantity.Size = new Size(72, 20);
            lbQuantity.TabIndex = 32;
            lbQuantity.Text = "Quantity :";
            // 
            // lbUnitPrice
            // 
            lbUnitPrice.AutoSize = true;
            lbUnitPrice.Location = new Point(75, 220);
            lbUnitPrice.Name = "lbUnitPrice";
            lbUnitPrice.Size = new Size(79, 20);
            lbUnitPrice.TabIndex = 33;
            lbUnitPrice.Text = "Unit Price :";
            // 
            // lbCustomer
            // 
            lbCustomer.AutoSize = true;
            lbCustomer.Location = new Point(75, 325);
            lbCustomer.Name = "lbCustomer";
            lbCustomer.Size = new Size(79, 20);
            lbCustomer.TabIndex = 34;
            lbCustomer.Text = "Customer :";
            // 
            // lbExportDate
            // 
            lbExportDate.AutoSize = true;
            lbExportDate.Location = new Point(59, 430);
            lbExportDate.Name = "lbExportDate";
            lbExportDate.Size = new Size(95, 20);
            lbExportDate.TabIndex = 35;
            lbExportDate.Text = "Export Date :";
            // 
            // lbProductName
            // 
            lbProductName.AutoSize = true;
            lbProductName.Location = new Point(43, 31);
            lbProductName.Name = "lbProductName";
            lbProductName.Size = new Size(111, 20);
            lbProductName.TabIndex = 37;
            lbProductName.Text = "Product Name :";
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(190, 28);
            txtProductName.Margin = new Padding(3, 4, 3, 4);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(249, 27);
            txtProductName.TabIndex = 36;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.ImportExport1;
            pictureBox1.Location = new Point(34, 655);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(120, 124);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 38;
            pictureBox1.TabStop = false;
            // 
            // ExportManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(pictureBox1);
            Controls.Add(lbProductName);
            Controls.Add(txtProductName);
            Controls.Add(lbExportDate);
            Controls.Add(lbCustomer);
            Controls.Add(lbUnitPrice);
            Controls.Add(lbQuantity);
            Controls.Add(lblTotalAmount);
            Controls.Add(dtpExportDate);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(cboCustomer);
            Controls.Add(txtQuantity);
            Controls.Add(txtUnitPrice);
            Controls.Add(dgvExportDetails);
            Controls.Add(dgvExports);
            Margin = new Padding(3, 4, 3, 4);
            Name = "ExportManagementForm";
            Size = new Size(1012, 870);
            ((System.ComponentModel.ISupportInitialize)dgvExportDetails).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvExports).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private DateTimePicker dtpExportDate;
        private Button btnRefresh;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnAdd;
        private ComboBox cboCustomer;
        private TextBox txtQuantity;
        private TextBox txtUnitPrice;
        private DataGridView dgvExportDetails;
        private DataGridView dgvExports;
        private Label lblTotalAmount;
        private Label lbQuantity;
        private Label lbUnitPrice;
        private Label lbCustomer;
        private Label lbExportDate;
        private Label lbProductName;
        private TextBox txtProductName;
        private PictureBox pictureBox1;
    }
}
