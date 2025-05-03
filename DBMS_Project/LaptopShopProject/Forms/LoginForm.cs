using System;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using LaptopShopProject.Models;

namespace LaptopStoreApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly string connectionString = "Server=TonyNyan\\TONYNYAN;Database=LaptopStoreDBMS4;Trusted_Connection=True;";

        public LoginForm()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string userConnectionString = $"Server=TonyNyan\\TONYNYAN;Database=LaptopStoreDBMS4;User Id={username};Password={password};";

                using (SqlConnection connection = new SqlConnection(userConnectionString))
                {
                    connection.Open();

                    string roleQuery = @"
                        SELECT r.name AS RoleName
                        FROM sys.database_principals u
                        INNER JOIN sys.database_role_members rm ON u.principal_id = rm.member_principal_id
                        INNER JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
                        WHERE u.name = @username AND r.type = 'R'";

                    using (SqlCommand command = new SqlCommand(roleQuery, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        object result = command.ExecuteScalar();

                        string role = result?.ToString();
                        if (string.IsNullOrEmpty(role))
                        {
                            MessageBox.Show("User has no assigned role.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        MessageBox.Show($"Login successful! Welcome, {username} ({role}).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        MainForm mainForm = new MainForm(username, role, password);
                        mainForm.Show();
                        this.Hide();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 18456)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Laptop Store Management System", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}