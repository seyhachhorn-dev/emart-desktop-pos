using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class ProductFrm : Form
    {
        public ProductFrm()
        {
            InitializeComponent();
            LoadCategoryTitle();
            LoadProducts();
        }

        private void productsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
        }

        private void LoadCategoryTitle()
        {
            using (var conn = new SqlConnection(DbConfig.con_string))
            {
                var adapter = new SqlDataAdapter("SELECT id, title FROM tbl_categories", conn);
                var dt = new DataTable();
                adapter.Fill(dt);
                cmbCategory.DataSource = dt;
                cmbCategory.DisplayMember = "title";
                cmbCategory.ValueMember = "id";
                cmbCategory.SelectedIndex = -1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearFormFields();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string productName = txtProductName.Text.Trim();
            int qty;
            decimal priceIn;
            decimal priceOut;
            bool status = checkBoxStatus.Checked;
            int userId = UserSession.UserId;

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Please enter product name.");
                return;
            }

            if (!int.TryParse(txtQty.Text.Trim(), out qty) || qty <0)
            {
                MessageBox.Show("Invalid Quantity.");
                return;
            }


            //if (cmbCategory.SelectedValue == null || !(cmbCategory.SelectedValue is int cateId))
            //{
            //    MessageBox.Show("Please select a category.");
            //    return;
            //}
            if (cmbCategory.SelectedIndex == -1)  
            {
                MessageBox.Show("Please select a category.");
                return;
            }
            int cateId = Convert.ToInt32(cmbCategory.SelectedValue);


            if (!decimal.TryParse(txtPriceIn.Text.Trim(), out priceIn) || priceIn < 0)
            {
                MessageBox.Show("Invalid Purchase Price.");
                return;
            }

            if (!decimal.TryParse(TxtPriceOut.Text.Trim(), out priceOut) || priceOut < 0)
            {
                MessageBox.Show("Invalid Selling Price.");
                return;
            }

            string query = @"INSERT INTO tbl_products
                (barcode, product_name, qty, cate_id,
                 price_in, price_out, status, created_at, user_id)
                VALUES
                (@barcode, @product_name, @qty, @cate_id,
                 @price_in, @price_out, @status, @created_at, @user_id)";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@barcode", SqlDbType.NVarChar).Value = barcode;
                    cmd.Parameters.Add("@product_name", SqlDbType.NVarChar).Value = productName;
                    cmd.Parameters.Add("@qty", SqlDbType.Int).Value = qty;
                    cmd.Parameters.Add("@cate_id", SqlDbType.Int).Value = cateId;
                    cmd.Parameters.Add("@price_in", SqlDbType.Decimal).Value = priceIn;
                    cmd.Parameters.Add("@price_out", SqlDbType.Decimal).Value = priceOut;
                    cmd.Parameters.Add("@status", SqlDbType.Bit).Value = status;
                    cmd.Parameters.Add("@created_at", SqlDbType.DateTime2).Value = DateTime.Now;
                    cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = userId;

                    con.Open();
                    int isSuccess = cmd.ExecuteNonQuery();

                    if (isSuccess > 0)
                    {
                        MessageBox.Show("Product successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFormFields();
                        LoadProducts();
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
            txtBarcode.Clear();
            txtProductName.Clear();
            txtQty.Clear();
            cmbCategory.SelectedIndex = -1;
            txtPriceIn.Clear();
            TxtPriceOut.Clear();
            checkBoxStatus.Checked = false;
        }

        private void LoadProducts()
        {
            SearchProducts("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchProducts(textBoxSearch.Text.Trim());
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SearchProducts(textBoxSearch.Text.Trim());
            }
        }

        private void SearchProducts(string keyword)
        {
            string query = "SELECT p.id, p.barcode, p.product_name, p.qty, c.title AS category, p.price_in, p.price_out, " +
                "CASE WHEN p.status = 1 THEN 'Active' ELSE 'Inactive' END AS status " +
                "FROM tbl_products p LEFT JOIN tbl_categories c ON p.cate_id = c.id ";

            if (keyword != "")
            {
                query += "WHERE p.product_name LIKE @keyword OR p.barcode LIKE @keyword OR c.title LIKE @keyword ";
            }

            query += "ORDER BY p.id";

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
                            reader["barcode"] == DBNull.Value ? "" : reader["barcode"],
                            reader["product_name"],
                            reader["qty"],
                            reader["category"] == DBNull.Value ? "" : reader["category"],
                            reader["price_in"],
                            reader["price_out"],
                            reader["status"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void userDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (userDataGridView.CurrentRow == null || userDataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row first to update.");
                return;
            }

            int selectedProductId = Convert.ToInt32(userDataGridView.CurrentRow.Cells["Id"].Value);
            Global.OpenForm(new ProductUpdate(selectedProductId));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (userDataGridView.CurrentRow == null || userDataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row first to delete.");
                return;
            }

            int selectedProductId = Convert.ToInt32(userDataGridView.CurrentRow.Cells["Id"].Value);
            string selectedProductName = userDataGridView.CurrentRow.Cells["ProductName"].Value.ToString();

            if (!UserSession.CanDelete("tbl_products", selectedProductId))
            {
                MessageBox.Show("Only an Admin can delete it.",
                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete product '{selectedProductName}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string query = "DELETE FROM tbl_products WHERE id = @id";

                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", selectedProductId);
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts();
                    }
                    else
                    {
                        MessageBox.Show("Product not found — maybe already deleted.");
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
