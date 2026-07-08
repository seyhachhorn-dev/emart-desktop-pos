using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Emart
{
    internal class HelperClass
    {

        public static bool IsValidUser(string username, string password)
        {
            bool isValid = false;

            // using @parameter map to prevent SQL injection
            string selectQuery = "select user_id,username,role from tbl_users where username = @Username and password = @Password";

            try
            {
                using (SqlConnection con = new SqlConnection(DbConfig.con_string))
                using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username.Trim());
                    cmd.Parameters.AddWithValue("@Password", password.Trim());

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserSession.UserId = Convert.ToInt32(reader["user_id"]);
                            UserSession.Username = reader["username"].ToString();
                            UserSession.Role = reader["role"].ToString();
                            isValid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isValid;
        }

        public static object DbValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
