namespace LaptopShopProject.Forms
{
    partial class UserManagementForm
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
            dgvUsers = new DataGridView();
            dgvPermisstionLogs = new DataGridView();
            txtUserName = new TextBox();
            txtPassword = new TextBox();
            cboRole = new ComboBox();
            btnAdd = new Button();
            btnDelete = new Button();
            btnUpdate = new Button();
            btnRefresh = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPermisstionLogs).BeginInit();
            SuspendLayout();
            // 
            // dgvUsers
            // 
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsers.Location = new Point(394, 31);
            dgvUsers.Margin = new Padding(3, 4, 3, 4);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.Size = new Size(583, 335);
            dgvUsers.TabIndex = 0;
            dgvUsers.CellContentClick += dgvUsers_CellContentClick;
            // 
            // dgvPermisstionLogs
            // 
            dgvPermisstionLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPermisstionLogs.Location = new Point(394, 405);
            dgvPermisstionLogs.Margin = new Padding(3, 4, 3, 4);
            dgvPermisstionLogs.Name = "dgvPermisstionLogs";
            dgvPermisstionLogs.RowHeadersWidth = 51;
            dgvPermisstionLogs.Size = new Size(583, 351);
            dgvPermisstionLogs.TabIndex = 1;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(40, 31);
            txtUserName.Margin = new Padding(3, 4, 3, 4);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(249, 27);
            txtUserName.TabIndex = 2;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(40, 131);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(249, 27);
            txtPassword.TabIndex = 3;
            // 
            // cboRole
            // 
            cboRole.FormattingEnabled = true;
            cboRole.Location = new Point(40, 241);
            cboRole.Margin = new Padding(3, 4, 3, 4);
            cboRole.Name = "cboRole";
            cboRole.Size = new Size(249, 28);
            cboRole.TabIndex = 4;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(394, 811);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 31);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(713, 811);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(554, 811);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(871, 811);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 8;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(cboRole);
            Controls.Add(txtPassword);
            Controls.Add(txtUserName);
            Controls.Add(dgvPermisstionLogs);
            Controls.Add(dgvUsers);
            Margin = new Padding(3, 4, 3, 4);
            Name = "UserManagementForm";
            Size = new Size(1030, 868);
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPermisstionLogs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvUsers;
        private DataGridView dgvPermisstionLogs;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private ComboBox cboRole;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnRefresh;
    }
}
