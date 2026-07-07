using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace POS_Emart
{
    public partial class ProductFrm : Form
    {
        public ProductFrm()
        {
            InitializeComponent();
            LoadCategoryTitle();
        }

        private void productsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();


        }

        private void LoadCategoryTitle()
        {
            using (var conn = new SqlConnection(DbConfig.con_string))
            {
                var adapter = new SqlDataAdapter("Select id, title from tbl_categories", conn);
                var dt = new DataTable();
                // 메모 
                //속도를 높이기 위해 데이터를 복사하세요.
                // declare memory table and copy to it
                adapter.Fill(dt); 
                cmbCategory.DataSource = dt;
                cmbCategory.DisplayMember = "title";
                cmbCategory.ValueMember = "id";
                cmbCategory.SelectedIndex = -1;

            }
        }

        //old version


        //private void LoadCategories()
        //{
        //    using (SqlConnection con = new SqlConnection(DbConfig.con_string))
        //        try
        //        {
        //            string query = "SELECT id, title FROM tbl_categories WHERE status = 1 ORDER BY title";

        //            using (SqlCommand cmd = new SqlCommand(query, con))
        //            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //            {
        //                DataTable categories = new DataTable();
        //                con.Open();
        //                adapter.Fill(categories);

        //                cmbCategory.DataSource = categories;
        //                cmbCategory.DisplayMember = "title";
        //                cmbCategory.ValueMember = "id";
        //                //cmbCategory.SelectedIndex = -1;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //}

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

            if (!decimal.TryParse(txtPriceIn.Text.Trim(), out priceIn))
            {
                MessageBox.Show("Invalid Purchase Price.");
                return;
            }

            if (!decimal.TryParse(TxtPriceOut.Text.Trim(), out priceOut))
            {
                MessageBox.Show("Invalid Selling Price.");
                return;
            }

            using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                try
                {
                    string query = @"INSERT INTO tbl_products
                        (barcode, product_name, qty, cate_id,
                         price_in, price_out, status, created_at, user_id)
                        VALUES
                        (@barcode, @product_name, @qty, @cate_id,
                         @price_in, @price_out, @status, @created_at, @user_id)";

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


                        try
                        {
                            con.Open();
                            int isSuccess = cmd.ExecuteNonQuery();

                            if (isSuccess > 0)
                            {
                                MessageBox.Show("Product successfully saved!",
                                    "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                ClearFormFields();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Product Error: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
