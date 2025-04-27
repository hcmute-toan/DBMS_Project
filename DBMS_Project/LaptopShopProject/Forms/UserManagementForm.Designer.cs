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
            dgvUsers.Location = new Point(345, 23);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.Size = new Size(510, 251);
            dgvUsers.TabIndex = 0;
            // 
            // dgvPermisstionLogs
            // 
            dgvPermisstionLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPermisstionLogs.Location = new Point(345, 304);
            dgvPermisstionLogs.Name = "dgvPermisstionLogs";
            dgvPermisstionLogs.Size = new Size(510, 263);
            dgvPermisstionLogs.TabIndex = 1;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(35, 23);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(218, 23);
            txtUserName.TabIndex = 2;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(35, 98);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(218, 23);
            txtPassword.TabIndex = 3;
            // 
            // cboRole
            // 
            cboRole.FormattingEnabled = true;
            cboRole.Location = new Point(35, 181);
            cboRole.Name = "cboRole";
            cboRole.Size = new Size(218, 23);
            cboRole.TabIndex = 4;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(345, 608);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(93, 23);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += new EventHandler(btnAdd_Click);
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(624, 608);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(89, 23);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += new EventHandler(btnDelete_Click);   
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(485, 608);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(93, 23);
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(762, 608);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(93, 23);
            btnRefresh.TabIndex = 8;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
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
            Name = "UserManagementForm";
            Size = new Size(901, 651);
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
