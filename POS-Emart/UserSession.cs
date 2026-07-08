using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_Emart
{
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string Role { get; set; }
        public static bool IsLoggedIn => UserId > 0;
        public static bool IsAdmin =>
       string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);
        public static void Logout()    // call this on logout
        {
            UserId = 0;
            Username = null;
            Role = null;
        }

        // Admins can delete anything; everyone else may only delete rows they created.
        public static bool CanDelete(string tableName, int id)
        {
            if (string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            using (SqlConnection con = new SqlConnection(DbConfig.con_string))
            using (SqlCommand cmd = new SqlCommand($"SELECT user_id FROM {tableName} WHERE id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                object ownerId = cmd.ExecuteScalar();

                if (ownerId == null || ownerId == DBNull.Value)
                    return false;

                return Convert.ToInt32(ownerId) == UserId;
            }
        }
    }
}
