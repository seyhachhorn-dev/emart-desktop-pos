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

namespace POS_Emart
{
    public partial class CategoryFrm : Form
    {
        public CategoryFrm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string txtTitleBox = titleTextBox.Text.Trim();
            string txtDesBox = desTextBox.Text.Trim();
            bool statusCheckBox = checkBoxStatus.Checked;

            if(string.IsNullOrWhiteSpace(txtTitleBox) || string.IsNullOrWhiteSpace(txtDesBox))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            using (SqlConnection con = new SqlConnection(DbConfig.con_string))
               try{

                    string query = "Insert into Categories(title,Description,status) VALUES(@title,@des,@status)";


                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = txtTitleBox;
                        cmd.Parameters.Add("@des", SqlDbType.NVarChar).Value = txtDesBox;
                        cmd.Parameters.Add("@status", SqlDbType.Bit).Value = statusCheckBox;

                        try
                        {
                            con.Open();
                            int isSuccess = cmd.ExecuteNonQuery();

                            if(isSuccess > 0)
                            {
                                MessageBox.Show("Category successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearFormFields();
                            }


                        }catch(Exception ex)
                        {
                            MessageBox.Show($"Category Error: {ex.Message}");
                        }

                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
        }

        private  void ClearFormFields()
        {
            titleTextBox.Clear();
            desTextBox.Clear();
            checkBoxStatus.Checked = false;
        }
    }
}
