using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class ProductUpdate : Form
    {
        private readonly int productId;

        public ProductUpdate(int productId)
        {
            InitializeComponent();
            this.productId = productId;
            LoadCategoryTitle();
            LoadProduct();
        }

        private void LoadCategoryTitle()
        {
            using (var conn = new SqlConnection(DbConfig.con_string))
            {
                var adapter = new SqlDataAdapter("SELECT id, title FROM tbl_categories WHERE status = 1", conn);
                var dt = new DataTable();
                adapter.Fill(dt);
                cmbCategory.DataSource = dt;
                cmbCategory.DisplayMember = "title";
                cmbCategory.ValueMember = "id";
                cmbCategory.SelectedIndex = -1;
            }
        }

        private void LoadProduct()
        {
            string query = "SELECT barcode, product_name, qty, cate_id, price_in, price_out, status " +
                "FROM tbl_products WHERE id = @id";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", productId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtBarcode.Text = reader["barcode"] == DBNull.Value ? "" : reader["barcode"].ToString();
                        txtProductName.Text = reader["product_name"] == DBNull.Value ? "" : reader["product_name"].ToString();
                        txtQty.Text = reader["qty"].ToString();
                        cmbCategory.SelectedValue = reader["cate_id"] == DBNull.Value ? (object)DBNull.Value : reader["cate_id"];
                        txtPriceIn.Text = reader["price_in"].ToString();
                        TxtPriceOut.Text = reader["price_out"].ToString();
                        checkBoxStatus.Checked = Convert.ToBoolean(reader["status"]);
                    }
                    else
                    {
                        MessageBox.Show("Product not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new ProductFrm());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            string productName = txtProductName.Text.Trim();
            int qty;
            decimal priceIn;
            decimal priceOut;
            bool status = checkBoxStatus.Checked;

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Please enter product name.");
                return;
            }

            if (!int.TryParse(txtQty.Text.Trim(), out qty))
            {
                MessageBox.Show("Invalid Quantity.");
                return;
            }

            if (cmbCategory.SelectedValue == null || !(cmbCategory.SelectedValue is int cateId))
            {
                MessageBox.Show("Please select a category.");
                return;
            }

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

            string query = @"UPDATE tbl_products SET
                barcode = @barcode, product_name = @product_name, qty = @qty, cate_id = @cate_id,
                price_in = @price_in, price_out = @price_out, status = @status
                WHERE id = @id";

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
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = productId;

                    con.Open();
                    int isSuccess = cmd.ExecuteNonQuery();

                    if (isSuccess > 0)
                    {
                        MessageBox.Show("Product successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Global.OpenForm(new ProductFrm());
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
