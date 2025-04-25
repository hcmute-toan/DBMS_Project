namespace LaptopShopProject.Forms
{
    partial class SupplierManagementForm
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
            txtContactInfo = new TextBox();
            txtSupplierName = new TextBox();
            dgvSuppliers = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvSuppliers).BeginInit();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(767, 321);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(93, 23);
            btnRefresh.TabIndex = 24;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(491, 321);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(93, 23);
            btnUpdate.TabIndex = 23;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(628, 321);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(89, 23);
            btnDelete.TabIndex = 22;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(350, 321);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(93, 23);
            btnAdd.TabIndex = 21;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // txtContactInfo
            // 
            txtContactInfo.Location = new Point(40, 103);
            txtContactInfo.Name = "txtContactInfo";
            txtContactInfo.Size = new Size(218, 23);
            txtContactInfo.TabIndex = 20;
            // 
            // txtSupplierName
            // 
            txtSupplierName.Location = new Point(40, 28);
            txtSupplierName.Name = "txtSupplierName";
            txtSupplierName.Size = new Size(218, 23);
            txtSupplierName.TabIndex = 19;
            // 
            // dgvSuppliers
            // 
            dgvSuppliers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSuppliers.Location = new Point(350, 28);
            dgvSuppliers.Name = "dgvSuppliers";
            dgvSuppliers.Size = new Size(510, 251);
            dgvSuppliers.TabIndex = 18;
            // 
            // SupplierManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(txtContactInfo);
            Controls.Add(txtSupplierName);
            Controls.Add(dgvSuppliers);
            Name = "SupplierManagementForm";
            Size = new Size(901, 372);
            ((System.ComponentModel.ISupportInitialize)dgvSuppliers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRefresh;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnAdd;
        private TextBox txtContactInfo;
        private TextBox txtSupplierName;
        private DataGridView dgvSuppliers;
    }
}
