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
    public partial class UserUpdateFrm : Form
    {
        private readonly int userId;
        private byte[] photoBytes = null;
        private bool photoChanged = false;

        public UserUpdateFrm(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new UserFrm());
        }

        private void UserUpdateFrm_Load(object sender, EventArgs e)
        {
            LoadGenderTitle();
            LoadUser();
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

        private void LoadUser()
        {
            string query = "SELECT nickname, username, phone, gender_id, role, disabled, photo " +
                "FROM tbl_users WHERE user_id=@user_id";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtNickName.Text = reader["nickname"] == DBNull.Value ? "" : reader["nickname"].ToString();
                        txtUsername.Text = reader["username"] == DBNull.Value ? "" : reader["username"].ToString();
                        txtPhone.Text = reader["phone"] == DBNull.Value ? "" : reader["phone"].ToString();
                        comboBoxGender.SelectedValue = reader["gender_id"] == DBNull.Value ? (object)DBNull.Value : reader["gender_id"];
                        comboBoxRole.SelectedItem = reader["role"] == DBNull.Value ? null : reader["role"].ToString();
                        checkBoxStatus.Checked = !Convert.ToBoolean(reader["disabled"]);

                        if (reader["photo"] != DBNull.Value)
                        {
                            photoBytes = (byte[])reader["photo"];
                            MemoryStream ms = new MemoryStream(photoBytes);
                            pictureBoxPhoto.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        MessageBox.Show("User not found.");
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
                        photoChanged = true;
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

            if (nickName == "" || username == "")
            {
                MessageBox.Show("Please fill in Nickname and Username.");
                return;
            }

            if (comboBoxGender.SelectedValue == null)
            {
                MessageBox.Show("Please select a gender.");
                return;
            }

            int genderId = (int)comboBoxGender.SelectedValue;

            string query = "UPDATE tbl_users SET nickname=@nickname, gender_id=@gender_id, username=@username, " +
                "phone=@phone, role=@role, disabled=@disabled";

            if (password != "")
                query += ", password=@password";

            if (photoChanged)
                query += ", photo=@photo";

            query += " WHERE user_id=@user_id";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nickname", nickName);
                    cmd.Parameters.AddWithValue("@gender_id", genderId);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@phone", phone == "" ? (object)DBNull.Value : phone);
                    cmd.Parameters.AddWithValue("@role", role == null ? (object)DBNull.Value : role);
                    cmd.Parameters.AddWithValue("@disabled", !isActive);
                    cmd.Parameters.AddWithValue("@user_id", userId);

                    if (password != "")
                        cmd.Parameters.AddWithValue("@password", password);

                    if (photoChanged)
                        cmd.Parameters.AddWithValue("@photo", photoBytes == null ? (object)DBNull.Value : photoBytes);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User successfully updated!");
                        Global.OpenForm(new UserFrm());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
