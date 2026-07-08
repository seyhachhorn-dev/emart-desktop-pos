using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class CategoryFrm : Form
    {
        public CategoryFrm()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void label1_Click(object sender, EventArgs e)
        {

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

            string query = "INSERT INTO tbl_categories (title, Description, status, user_id) VALUES (@title, @des, @status, @user_id)";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = txtTitleBox;
                    cmd.Parameters.Add("@des", SqlDbType.NVarChar).Value = txtDesBox;
                    cmd.Parameters.Add("@status", SqlDbType.Bit).Value = statusCheckBox;
                    cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = UserSession.UserId;

                    con.Open();
                    int isSuccess = cmd.ExecuteNonQuery();

                    if (isSuccess > 0)
                    {
                        MessageBox.Show("Category successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFormFields();
                        LoadCategories();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFormFields()
        {
            titleTextBox.Clear();
            desTextBox.Clear();
            checkBoxStatus.Checked = false;
        }

        private void LoadCategories()
        {
            SearchCategories("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchCategories(textBoxSearch.Text.Trim());
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchCategories(textBoxSearch.Text.Trim());
            }
        }

        private void SearchCategories(string keyword)
        {
            string query = "SELECT id, title, Description, " +
                "CASE WHEN status = 1 THEN 'Active' ELSE 'Inactive' END AS status " +
                "FROM tbl_categories ";

            if (keyword != "")
            {
                query += "WHERE title LIKE @keyword OR Description LIKE @keyword ";
            }

            query += "ORDER BY id";

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
                            reader["id"],
                            reader["title"],
                            reader["Description"] == DBNull.Value ? "" : reader["Description"],
                            reader["status"]);
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
            if (userDataGridView.CurrentRow == null || userDataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row first to update.");
                return;
            }

            int selectedCategoryId = Convert.ToInt32(userDataGridView.CurrentRow.Cells["Id"].Value);
            Global.OpenForm(new CategoryUpdateFrm(selectedCategoryId));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (userDataGridView.CurrentRow == null || userDataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row first to delete.");
                return;
            }

            int selectedCategoryId = Convert.ToInt32(userDataGridView.CurrentRow.Cells["Id"].Value);
            string selectedTitle = userDataGridView.CurrentRow.Cells["title"].Value.ToString();

            if (!UserSession.CanDelete("tbl_categories", selectedCategoryId))
            {
                MessageBox.Show("Only an Admin can delete it.",
                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete category '{selectedTitle}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string query = "DELETE FROM tbl_categories WHERE id = @id";

                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", selectedCategoryId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Category not found — maybe already deleted.");
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
