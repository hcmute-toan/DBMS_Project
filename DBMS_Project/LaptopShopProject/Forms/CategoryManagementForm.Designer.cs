namespace LaptopShopProject.Forms
{
    partial class CategoryManagementForm
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
            txtDescription = new TextBox();
            txtCategoryName = new TextBox();
            dgvCategories = new DataGridView();
            lbCategoryName = new Label();
            lbDescription = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();
            SuspendLayout();
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(899, 404);
            btnRefresh.Margin = new Padding(3, 4, 3, 4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(106, 31);
            btnRefresh.TabIndex = 17;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(583, 404);
            btnUpdate.Margin = new Padding(3, 4, 3, 4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(106, 31);
            btnUpdate.TabIndex = 16;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(740, 404);
            btnDelete.Margin = new Padding(3, 4, 3, 4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(102, 31);
            btnDelete.TabIndex = 15;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(422, 404);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(106, 31);
            btnAdd.TabIndex = 14;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(160, 129);
            txtDescription.Margin = new Padding(3, 4, 3, 4);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(249, 27);
            txtDescription.TabIndex = 12;
            // 
            // txtCategoryName
            // 
            txtCategoryName.Location = new Point(160, 29);
            txtCategoryName.Margin = new Padding(3, 4, 3, 4);
            txtCategoryName.Name = "txtCategoryName";
            txtCategoryName.Size = new Size(249, 27);
            txtCategoryName.TabIndex = 11;
            // 
            // dgvCategories
            // 
            dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategories.Location = new Point(440, 28);
            dgvCategories.Margin = new Padding(3, 4, 3, 4);
            dgvCategories.Name = "dgvCategories";
            dgvCategories.RowHeadersWidth = 51;
            dgvCategories.Size = new Size(543, 286);
            dgvCategories.TabIndex = 9;
            // 
            // lbCategoryName
            // 
            lbCategoryName.AutoSize = true;
            lbCategoryName.Location = new Point(13, 32);
            lbCategoryName.Name = "lbCategoryName";
            lbCategoryName.Size = new Size(120, 20);
            lbCategoryName.TabIndex = 18;
            lbCategoryName.Text = "Category Name :";
            // 
            // lbDescription
            // 
            lbDescription.AutoSize = true;
            lbDescription.Location = new Point(13, 132);
            lbDescription.Name = "lbDescription";
            lbDescription.Size = new Size(92, 20);
            lbDescription.TabIndex = 19;
            lbDescription.Text = "Description :";
            // 
            // CategoryManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            Controls.Add(lbDescription);
            Controls.Add(lbCategoryName);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(btnAdd);
            Controls.Add(txtDescription);
            Controls.Add(txtCategoryName);
            Controls.Add(dgvCategories);
            Margin = new Padding(3, 4, 3, 4);
            Name = "CategoryManagementForm";
            Size = new Size(1030, 496);
            ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnRefresh;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnAdd;
        private TextBox txtDescription;
        private TextBox txtCategoryName;
        private DataGridView dgvCategories;
        private Label lbCategoryName;
        private Label lbDescription;
    }
}
