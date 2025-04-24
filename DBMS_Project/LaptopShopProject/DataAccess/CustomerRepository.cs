using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LaptopShopProject.Models;

namespace LaptopShopProject.DataAccess
{
    internal class CustomerRepository
    {
        private readonly string connectionString = "Data Source=YNWA\\SQLEXPRESS;Initial Catalog=DBMS_2;Integrated Security=True";

        // Get all customers (Read)
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllCustomers", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = Convert.ToInt32(reader["customer_id"]),
                            Name = reader["name"].ToString(),
                            Gmail = reader["gmail"].ToString(),
                            Phone = reader["phone"].ToString()
                            // Address and ExportDate are not in Customer table, can be fetched if needed
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching customers: {ex.Message}", ex);
            }
            return customers;
        }

        // Insert a new customer (Create)
        public int InsertCustomer(string name, string gmail, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertCustomer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@gmail", gmail);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting customer: {ex.Message}", ex);
            }
        }

        // Update a customer (Update)
        public void UpdateCustomer(int customerId, string name, string gmail, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateCustomer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@gmail", gmail);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating customer: {ex.Message}", ex);
            }
        }

        // Delete a customer (Delete)
        public void DeleteCustomer(int customerId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteCustomer", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting customer: {ex.Message}", ex);
            }
        }
    }

}