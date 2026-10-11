using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Database
{
    public static class DbHelper
    {
        // Thêm <connectionStrings name="quanlydiem"> vào App.config của GUI
        public static string ConnStr
        {
            get
            {
                var connSetting = ConfigurationManager.ConnectionStrings["quanlydiem"];
                if (connSetting != null && !string.IsNullOrWhiteSpace(connSetting.ConnectionString))
                {
                    return connSetting.ConnectionString;
                }
                return @"Data Source=localhost;Initial Catalog=quanlydiem;Integrated Security=True;TrustServerCertificate=True";
            }
        }

        public static DataTable ExecuteQuery(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
