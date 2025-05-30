﻿namespace LaptopShopProject.Forms
{
    partial class CustomerManagementForm
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
            txtCustomerName = new TextBox();
            dgvCustomers = new DataGridView();
            lbCustomerName = new Label();
            lbContactInfo = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)dgvCustomers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(903, 417);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 24;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(587, 417);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 23;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(744, 417);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 22;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(426, 417);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 31);
            btnAdd.TabIndex = 21;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // txtContactInfo
            // 
            txtContactInfo.Location = new Point(161, 139);
            txtContactInfo.Margin = new Padding(3, 4, 3, 4);
            txtContactInfo.Name = "txtContactInfo";
            txtContactInfo.Size = new Size(249, 27);
            txtContactInfo.TabIndex = 20;
            // 
            // txtCustomerName
            // 
            txtCustomerName.Location = new Point(161, 39);
            txtCustomerName.Margin = new Padding(3, 4, 3, 4);
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new Size(249, 27);
            txtCustomerName.TabIndex = 19;
            // 
            // dgvCustomers
            // 
            dgvCustomers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCustomers.Location = new Point(447, 39);
            dgvCustomers.Margin = new Padding(3, 4, 3, 4);
            dgvCustomers.Name = "dgvCustomers";
            dgvCustomers.RowHeadersWidth = 51;
            dgvCustomers.Size = new Size(536, 288);
            dgvCustomers.TabIndex = 18;
            dgvCustomers.SelectionChanged += dgvCustomers_SelectionChanged;
            // 
            // lbCustomerName
            // 
            lbCustomerName.AutoSize = true;
            lbCustomerName.Location = new Point(3, 42);
            lbCustomerName.Name = "lbCustomerName";
            lbCustomerName.Size = new Size(123, 20);
            lbCustomerName.TabIndex = 25;
            lbCustomerName.Text = "Customer Name :";
            // 
            // lbContactInfo
            // 
            lbContactInfo.AutoSize = true;
            lbContactInfo.Location = new Point(29, 142);
            lbContactInfo.Name = "lbContactInfo";
            lbContactInfo.Size = new Size(97, 20);
            lbContactInfo.TabIndex = 26;
            lbContactInfo.Text = "Contact Info :";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.supplier_customer;
            pictureBox1.Location = new Point(6, 324);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(120, 124);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 27;
            pictureBox1.TabStop = false;
            // 
            // CustomerManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(pictureBox1);
            Controls.Add(lbContactInfo);
            Controls.Add(lbCustomerName);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(txtContactInfo);
            Controls.Add(txtCustomerName);
            Controls.Add(dgvCustomers);
            Margin = new Padding(3, 4, 3, 4);
            Name = "CustomerManagementForm";
            Size = new Size(1030, 496);
            ((System.ComponentModel.ISupportInitialize)dgvCustomers).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRefresh;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnAdd;
        private TextBox txtContactInfo;
        private TextBox txtCustomerName;
        private DataGridView dgvCustomers;
        private Label lbCustomerName;
        private Label lbContactInfo;
        private PictureBox pictureBox1;
    }
}