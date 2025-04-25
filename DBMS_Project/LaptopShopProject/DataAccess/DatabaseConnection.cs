using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaptopShopProject.Properties;
using Microsoft.Data.SqlClient;


namespace LaptopShopProject.DataAccess
{
    public static class DatabaseConnection
    {
        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(Settings.Default.ConnStr);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create database connection. Ensure the connection string is correct.", ex);
            }
        }
    }
}
