﻿using DataAccessLayer.Model;
using System.Security.Principal;
using System.Data.SqlClient;
using System.Data;

namespace DataAccessLayer.SqlDbDataAccess
{
    public class OrderDAO : IOrderDataAccess
    {
        private string connectionstring;

        public OrderDAO(string connectionstring)
        { 
            this.connectionstring = connectionstring;
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            int id = -1;
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                string queryString = "INSERT INTO dbo.Order ('orderStatus', 'customer') VALUES (@status, @customer); SELECT CAST(scope_identity() AS int)";
                SqlCommand insertCommand = new SqlCommand(queryString, connection);
                insertCommand.Parameters.AddWithValue("@status", order.Status);
                insertCommand.Parameters.AddWithValue("@customer", order.User.Email);
                id = (int)insertCommand.ExecuteScalar();
                order.Id = id;
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("An error occured while inserting account information into the database: " + sqlex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while inserting account information into the database: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return id;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("DELETE FROM Order WHERE id = " + id, connection);
                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while deleting an account: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            List<Order> orders = new List<Order>();
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("Select * from Order");
                selectCommand.Connection = connection;
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    User? user = null; //UserDAO.GetByIdAsync(reader.GetString("customer"));
                    List<LineItem>? items = null; //OrderLineItemsDAO.GetByIdAsync(reader.GetInt32("id"));
                    orders.Add(new Order(reader.GetInt32("id"), reader.GetDateTime("date"), reader.GetDecimal("total"), (Status)reader.GetInt32("status"), reader.GetString("note"), user, items));
                }
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("An sql error occured while trying to retrieve all the accounts from the database: " + sqlex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unspecified error occured while trying to retrieve all the accounts from the database: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                string query = "SELECT * FROM Order WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                User user = null; //UserDAO.GetByIdAsync(reader.GetString("customer"));
                List<LineItem> items = null; //OrderLineItemsDAO.GetByIdAsync(reader.GetInt32("id"));
                return new Order(reader.GetInt32("id"), reader.GetDateTime("date"), reader.GetDecimal("total"), (Status)reader.GetInt32("status"), reader.GetString("note"), user, items);
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("An sql error occured while trying to retrieve all the accounts from the database: " + sqlex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unspecified error occured while trying to retrieve all the accounts from the database: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(User user)
        {
            List<Order> orders = new List<Order>();
            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                SqlCommand selectCommand = new SqlCommand("Select * from Order where customer = " + user.Email);
                selectCommand.Connection = connection;
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    List<LineItem> items = null; //OrderLineItemsDAO.GetByIdAsync(reader.GetInt32("id"));
                    orders.Add(new Order(reader.GetInt32("id"), reader.GetDateTime("date"), reader.GetDecimal("total"), (Status)reader.GetInt32("status"), reader.GetString("note"), user, items));
                }
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("A sql error occured while trying to retrieve all the accounts from the database: " + sqlex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unspecified error occured while trying to retrieve all the accounts from the database: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return orders;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            if (order == null)
            {
                return false;
                throw new NullReferenceException();
            }

            using SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("UPDATE Order SET date = @date, total = @total, orderStatus = @staus WHERE id = @id", connection);
                command.Parameters.AddWithValue("@date", order.Date);
                command.Parameters.AddWithValue("@total", order.TotalPrice);
                command.Parameters.AddWithValue("@status", (int)order.Status);
                command.Parameters.AddWithValue("@id", order.Id);
                int affected = command.ExecuteNonQuery();
                if (affected == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while updateing name of an account: " + ex);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }
    }
}