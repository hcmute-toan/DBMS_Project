
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
            ((System.ComponentModel.ISupportInitialize)dgvExportDetails).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvExports).BeginInit();
            SuspendLayout();
            // 
            // dtpExportDate
            // 
            dtpExportDate.Location = new Point(46, 332);
            dtpExportDate.Margin = new Padding(3, 4, 3, 4);
            dtpExportDate.Name = "dtpExportDate";
            dtpExportDate.Size = new Size(249, 27);
            dtpExportDate.TabIndex = 30;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(877, 808);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 29;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(560, 808);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 28;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(719, 808);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 27;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(400, 808);
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
            cboCustomer.Location = new Point(46, 229);
            cboCustomer.Margin = new Padding(3, 4, 3, 4);
            cboCustomer.Name = "cboCustomer";
            cboCustomer.Size = new Size(249, 28);
            cboCustomer.TabIndex = 25;
            cboCustomer.SelectedIndexChanged += cboCustomer_SelectedIndexChanged;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(46, 28);
            txtQuantity.Margin = new Padding(3, 4, 3, 4);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(249, 27);
            txtQuantity.TabIndex = 24;
            // 
            // txtUnitPrice
            // 
            txtUnitPrice.Location = new Point(46, 124);
            txtUnitPrice.Margin = new Padding(3, 4, 3, 4);
            txtUnitPrice.Name = "txtUnitPrice";
            txtUnitPrice.Size = new Size(249, 27);
            txtUnitPrice.TabIndex = 23;
            // 
            // dgvExportDetails
            // 
            dgvExportDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExportDetails.Location = new Point(400, 403);
            dgvExportDetails.Margin = new Padding(3, 4, 3, 4);
            dgvExportDetails.Name = "dgvExportDetails";
            dgvExportDetails.RowHeadersWidth = 51;
            dgvExportDetails.Size = new Size(583, 351);
            dgvExportDetails.TabIndex = 22;
            // 
            // dgvExports
            // 
            dgvExports.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExports.Location = new Point(400, 28);
            dgvExports.Margin = new Padding(3, 4, 3, 4);
            dgvExports.Name = "dgvExports";
            dgvExports.RowHeadersWidth = 51;
            dgvExports.Size = new Size(583, 335);
            dgvExports.TabIndex = 21;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.Location = new Point(46, 427);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(135, 23);
            lblTotalAmount.TabIndex = 31;
            lblTotalAmount.Text = "Total Amount: ";
            // 
            // ExportManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            ClientSize = new Size(1012, 821);
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
            ((System.ComponentModel.ISupportInitialize)dgvExportDetails).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvExports).EndInit();
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
    }
}
