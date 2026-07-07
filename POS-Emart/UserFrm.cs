using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class UserFrm : Form
    {
        private byte[] photoBytes = null;

        public UserFrm()
        {
            InitializeComponent();
        }

        private void User_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(textBoxSearch.Text.Trim());
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadUsers(textBoxSearch.Text.Trim());
            }
        }

        private void LoadUsers(string searchTerm = "")
        {
            using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                try
                {
                    string query = @"SELECT user_id, nickname, username,
                            CASE gender_id WHEN 1 THEN 'Male' WHEN 2 THEN 'Female' ELSE '' END AS gender,
                            phone, role,
                            CASE WHEN disabled = 1 THEN 'Inactive' ELSE 'Active' END AS status
                        FROM tbl_users
                        WHERE (@search = '' OR
                            CAST(user_id AS NVARCHAR(20)) LIKE @likeSearch OR
                            nickname LIKE @likeSearch OR
                            username LIKE @likeSearch OR
                            phone LIKE @likeSearch OR
                            role LIKE @likeSearch)
                        ORDER BY user_id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@search", SqlDbType.NVarChar).Value = searchTerm ?? string.Empty;
                        cmd.Parameters.Add("@likeSearch", SqlDbType.NVarChar).Value = "%" + (searchTerm ?? string.Empty) + "%";

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dataGridView1.Rows.Clear();

                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(
                                    reader["user_id"],
                                    reader["nickname"],
                                    reader["username"],
                                    reader["gender"],
                                    reader["phone"] == DBNull.Value ? string.Empty : reader["phone"],
                                    reader["role"] == DBNull.Value ? string.Empty : reader["role"],
                                    reader["status"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void pictureBoxPhoto_Click(object sender, EventArgs e)
        {
            BrowsePhoto();
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            BrowsePhoto();
        }

        private void BrowsePhoto()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        photoBytes = File.ReadAllBytes(dialog.FileName);
                        pictureBoxPhoto.Image = Image.FromFile(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string nickName = txtNickName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string role = comboBoxRole.SelectedItem as string;
            bool isActive = checkBoxStatus.Checked;

            if (string.IsNullOrWhiteSpace(nickName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in Nickname, Username and Password.");
                return;
            }

            object genderId = comboBoxGender.SelectedIndex >= 0
                ? (object)(comboBoxGender.SelectedIndex + 1)
                : DBNull.Value;

            using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                try
                {
                    string query = @"INSERT INTO tbl_users
                        (nickname, gender_id, username, password, phone, photo, role, disabled)
                        VALUES
                        (@nickname, @gender_id, @username, @password, @phone, @photo, @role, @disabled)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@nickname", SqlDbType.NVarChar).Value = nickName;
                        cmd.Parameters.Add("@gender_id", SqlDbType.Int).Value = genderId;
                        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                        cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone;
                        cmd.Parameters.Add("@photo", SqlDbType.Image).Value = photoBytes != null ? (object)photoBytes : DBNull.Value;
                        cmd.Parameters.Add("@role", SqlDbType.NVarChar).Value = role != null ? (object)role : DBNull.Value;
                        cmd.Parameters.Add("@disabled", SqlDbType.Bit).Value = !isActive;

                        try
                        {
                            con.Open();
                            int isSuccess = cmd.ExecuteNonQuery();

                            if (isSuccess > 0)
                            {
                                MessageBox.Show("User successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearFormFields();
                                LoadUsers();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("User Error: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new UserUpdateFrm());
        }

        private void ClearFormFields()
        {
            txtNickName.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtPhone.Clear();
            comboBoxGender.SelectedIndex = -1;
            comboBoxRole.SelectedIndex = -1;
            checkBoxStatus.Checked = true;
            pictureBoxPhoto.Image = null;
            photoBytes = null;
        }
    }
}
