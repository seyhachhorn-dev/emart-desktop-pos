using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class CategoryUpdateFrm : Form
    {
        private readonly int categoryId;

        public CategoryUpdateFrm(int categoryId)
        {
            InitializeComponent();
            this.categoryId = categoryId;
            LoadCategory();
        }

        private void LoadCategory()
        {
            string query = "SELECT title, Description, status FROM tbl_categories WHERE id = @id";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", categoryId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        titleTextBox.Text = reader["title"] == DBNull.Value ? "" : reader["title"].ToString();
                        desTextBox.Text = reader["Description"] == DBNull.Value ? "" : reader["Description"].ToString();
                        checkBoxStatus.Checked = Convert.ToBoolean(reader["status"]);
                    }
                    else
                    {
                        MessageBox.Show("Category not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new CategoryFrm());
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string txtTitleBox = titleTextBox.Text.Trim();
            string txtDesBox = desTextBox.Text.Trim();
            bool statusCheckBox = checkBoxStatus.Checked;

            if (string.IsNullOrWhiteSpace(txtTitleBox) || string.IsNullOrWhiteSpace(txtDesBox))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            string query = "UPDATE tbl_categories SET title = @title, Description = @des, status = @status WHERE id = @id";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = txtTitleBox;
                    cmd.Parameters.Add("@des", SqlDbType.NVarChar).Value = txtDesBox;
                    cmd.Parameters.Add("@status", SqlDbType.Bit).Value = statusCheckBox;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = categoryId;

                    con.Open();
                    int isSuccess = cmd.ExecuteNonQuery();

                    if (isSuccess > 0)
                    {
                        MessageBox.Show("Category successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Global.OpenForm(new CategoryFrm());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
