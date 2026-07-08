using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class LoginFrm : Form
    {
        public LoginFrm()
        {
            InitializeComponent();
            this.FormClosing += LoginFrm_FormClosing;
        }

        private void LoginFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str_username = txtUsername.Text.Trim();
            string str_password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(str_username) || string.IsNullOrWhiteSpace(str_password))
            {
                MessageBox.Show("Please enter both Username and Password.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            bool isLogged = HelperClass.IsValidUser(str_username, str_password);

            if (isLogged)
            {
                MessageBox.Show("Login Successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                new MainDashboard().Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBoxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = checkBoxShowPass.Checked ? '\0' : '*';
        }
    }
}
