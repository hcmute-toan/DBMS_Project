using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace LaptopShopProject.DataAccess
{
    public class DatabaseConnection
    {
        private readonly string _connectionString = Properties.Settings.Default.ConnStr;

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
