using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class CustomerRepository
    {
        public List<Customer> GetAllCustomers(int currentUserId)
        {
            var customers = new List<Customer>();
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_GetAllCustomers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                ContactInfo = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return customers;
        }

        public void InsertCustomer(int currentUserId, Customer customer)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_InsertCustomer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@customer_name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@contact_info", customer.ContactInfo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCustomer(int currentUserId, Customer customer)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_UpdateCustomer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@customer_id", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@customer_name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@contact_info", customer.ContactInfo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCustomer(int currentUserId, int customerId)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_DeleteCustomer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
