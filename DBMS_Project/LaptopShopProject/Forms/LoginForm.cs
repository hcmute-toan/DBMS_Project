using LaptopShopProject.DataAccess;
using LaptopShopProject.Models;
using System;
using System.Windows.Forms;

namespace LaptopStoreApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UserRepository _userRepository;

        public LoginForm()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            // Enable password masking for txtPassword
            txtPassword.UseSystemPasswordChar = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Attempt to log in using UserRepository
                User user = _userRepository.Login(username, password);

                if (user != null)
                {
                    // Login successful
                    MessageBox.Show($"Login successful! Welcome, {user.Username} ({user.Role}).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Navigate to MainForm, passing the user object
                    MainForm mainForm = new MainForm(user);
                    mainForm.Show();

                    // Hide the login form
                    this.Hide();
                }
                else
                {
                    // Login failed
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Optional: Handle picture box click (e.g., show app info)
            MessageBox.Show("Laptop Store Management System", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Handle form closing to exit application
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}