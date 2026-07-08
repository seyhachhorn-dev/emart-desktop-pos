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
            LoadGenderTitle();
            LoadUsers();
        }

        private void LoadGenderTitle()
        {
            using (var conn = new SqlConnection(DbConfig.con_string))
            {
                var adapter = new SqlDataAdapter("Select gender_id, title_english from tbl_genders", conn);
                var dt = new DataTable();
                adapter.Fill(dt);
                comboBoxGender.DataSource = dt;
                comboBoxGender.DisplayMember = "title_english";
                comboBoxGender.ValueMember = "gender_id";
                comboBoxGender.SelectedIndex = -1;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchUsers(textBoxSearch.Text.Trim());
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchUsers(textBoxSearch.Text.Trim());
            }
        }

        private void LoadUsers()
        {
            SearchUsers("");
        }

        private void SearchUsers(string keyword)
        {
            string query = "SELECT user_id, nickname, username, " +
                "CASE gender_id WHEN 1 THEN 'Male' WHEN 2 THEN 'Female' ELSE '' END AS gender, " +
                "phone, role, " +
                "CASE WHEN disabled=1 THEN 'Inactive' ELSE 'Active' END AS status " +
                "FROM tbl_users ";

            if (keyword != "")
            {
                query += "WHERE CAST(user_id AS NVARCHAR(20)) LIKE @keyword OR nickname LIKE @keyword " +
                    "OR username LIKE @keyword OR phone LIKE @keyword OR role LIKE @keyword ";
            }

            query += "ORDER BY user_id";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (keyword != "")
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    userDataGridView.Rows.Clear();

                    while (reader.Read())
                    {
                        userDataGridView.Rows.Add(
                            reader["user_id"],
                            reader["nickname"],
                            reader["username"],
                            reader["gender"],
                            reader["phone"] == DBNull.Value ? "" : reader["phone"],
                            reader["role"] == DBNull.Value ? "" : reader["role"],
                            reader["status"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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

            if (nickName == "" || username == "" || password == "")
            {
                MessageBox.Show("Please fill in Nickname, Username and Password.");
                return;
            }

            if (comboBoxGender.SelectedValue == null)
            {
                MessageBox.Show("Please select a gender.");
                return;
            }

            int genderId = (int)comboBoxGender.SelectedValue;

            string query = "INSERT INTO tbl_users (nickname, gender_id, username, password, phone, photo, role, disabled) " +
                "VALUES (@nickname, @gender_id, @username, @password, @phone, @photo, @role, @disabled)";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nickname", nickName);
                    cmd.Parameters.AddWithValue("@gender_id", genderId);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@photo", HelperClass.DbValue(photoBytes));
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@disabled", !isActive);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User successfully saved!");
                        ClearFormFields();
                        LoadUsers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (userDataGridView.CurrentRow == null || userDataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row first to update.");
                return;
            }

            int selectedUserId = Convert.ToInt32(userDataGridView.CurrentRow.Cells["Id"].Value);
            Global.OpenForm(new UserUpdateFrm(selectedUserId));
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (userDataGridView.CurrentRow == null || userDataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row first to delete.");
                return;
            }

            int selectedUserId = Convert.ToInt32(userDataGridView.CurrentRow.Cells["id"].Value);
            string selectedUsername = userDataGridView.CurrentRow.Cells["username"].Value.ToString();

            if (selectedUserId == UserSession.UserId)
            {
                MessageBox.Show("You can't delete your own account while logged in.",
                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete user '{selectedUsername}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string query = "Delete from tbl_users where user_id = @id";

                using (var conn = new SqlConnection(DbConfig.con_string))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", selectedUserId);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User deleted successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsers();

                    }
                    
                    else
                        MessageBox.Show("User not found — maybe already deleted.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void lblSearch_Click(object sender, EventArgs e)
        {

        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
