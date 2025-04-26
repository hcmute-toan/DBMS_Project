using LaptopShopProject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LaptopShopProject.DataAccess
{
    public class CustomerRepository
    {
        public async Task<List<Customer>> GetAllCustomersAsync(int currentUserId)
        {
            var customers = new List<Customer>();
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_GetAllCustomers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving customers: " + ex.Message, ex);
            }
            return customers;
        }

        public async Task<int> InsertCustomerAsync(int currentUserId, Customer customer)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_InsertCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@customer_name", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@contact_info", customer.ContactInfo);
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A customer with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can insert customers.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error inserting customer: " + ex.Message, ex);
            }
        }

        public async Task UpdateCustomerAsync(int currentUserId, Customer customer)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_UpdateCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@customer_id", customer.CustomerId);
                        cmd.Parameters.AddWithValue("@customer_name", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@contact_info", customer.ContactInfo);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("A customer with this name already exists.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can update customers.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Khách hàng không tồn tại"))
            {
                throw new KeyNotFoundException("Customer does not exist.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating customer: " + ex.Message, ex);
            }
        }

        public async Task DeleteCustomerAsync(int currentUserId, int customerId)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_DeleteCustomer", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                        cmd.Parameters.AddWithValue("@customer_id", customerId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex) when (ex.Message.Contains("Chỉ admin"))
            {
                throw new UnauthorizedAccessException("Only admins can delete customers.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("Khách hàng không tồn tại"))
            {
                throw new KeyNotFoundException("Customer does not exist.", ex);
            }
            catch (SqlException ex) when (ex.Message.Contains("phiếu xuất liên quan"))
            {
                throw new InvalidOperationException("Cannot delete customer due to related export records.", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception("Error deleting customer: " + ex.Message, ex);
            }
        }
    }
}