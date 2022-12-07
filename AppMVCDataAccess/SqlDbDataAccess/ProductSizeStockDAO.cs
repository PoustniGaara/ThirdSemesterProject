﻿using DataAccessLayer.Exceptions;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer.SqlDbDataAccess
{
    public class ProductSizeStockDAO : IProductSizeStockDataAccess
    {

        #region Properties + Constructor
        public string connectionString;

        public ProductSizeStockDAO(string connectionstring)
        {
            this.connectionString = connectionstring;
        }
        #endregion

        public async Task CreateSizeStocksFromProductListAsync(SqlCommand command, int product_id, IEnumerable<ProductSizeStock> productSizeStocks)
        {
            foreach(ProductSizeStock productSizeStock in productSizeStocks)
            {
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO dbo.ProductStock (product_id, size_id, stock) VALUES (@product_id, @size_id, @stock)";
                command.Parameters.AddWithValue("@product_id", product_id);
                command.Parameters.AddWithValue("@size_id", productSizeStock.Id);
                command.Parameters.AddWithValue("@stock", productSizeStock.Stock);

                command.ExecuteNonQuery();
            }
        }

        public async Task UpdateProductSizeStock(SqlCommand command, int product_id, IEnumerable<ProductSizeStock> productSizeStocks)
        {
            foreach (ProductSizeStock productSizeStock in productSizeStocks)
            {
                command.Parameters.Clear();
                command.CommandText = "UPDATE dbo.ProductStock SET stock = @stock WHERE product_id = @product_id AND size_id = @size_id";
                command.Parameters.AddWithValue("@product_id", product_id);
                command.Parameters.AddWithValue("@size_id", productSizeStock.Id);
                command.Parameters.AddWithValue("@stock", productSizeStock.Stock);
                command.ExecuteNonQuery();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM dbo.ProductStock WHERE product_id = @product_id";
            command.Parameters.AddWithValue("@product_id", id);
            command.ExecuteNonQuery();
        }

        public Task<IEnumerable<ProductSizeStock>> GetAByIdAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductSizeStock>> GetByProductIdAsync(int id)
        {
            List<ProductSizeStock> productSizeStocks = new List<ProductSizeStock>();
            using SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                string query = "SELECT ProductStock.size_id, ProductStock.stock, Sizes.size FROM ProductStock FULL OUTER JOIN Sizes ON ProductStock.size_id = Sizes.id WHERE product_id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    productSizeStocks.Add(new ProductSizeStock(reader.GetInt32("size_id"), reader.GetString("size"), reader.GetInt32("stock")));
                }
                return productSizeStocks;
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw new Exception("An unspecified error occured while trying to retrieve all the product stock and sizes from the database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public Task<ProductSizeStock> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DecreaseStockWithCheck(SqlCommand command, int productId, int sizeId, int amountToDecrease)
        {
            try
            {
                //Get the stock
                command.Parameters.Clear();
                command.CommandText = "SELECT stock FROM ProductStock WHERE product_id = @product_id AND size_id = @size_id";
                command.Parameters.AddWithValue("@product_id", productId);
                command.Parameters.AddWithValue("@size_id", sizeId);
                SqlDataReader reader = command.ExecuteReader();

                //Check the stock 
                reader.Read(); 
                int stockAmount = reader.GetInt32("stock");
                if(stockAmount < amountToDecrease) 
                    throw new ProductOutOfStockException();
                reader.Close();

                //Decrease the stock
                command.Parameters.Clear();
                command.CommandText = "UPDATE ProductStock SET stock = stock - @amountToDecrease WHERE product_id = @product_id AND size_id = @size_id";
                command.Parameters.AddWithValue("@amountToDecrease", amountToDecrease);
                command.Parameters.AddWithValue("@product_id", productId);
                command.Parameters.AddWithValue("@size_id", sizeId);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("An error occured while updateing name of an account: " + ex);
            }
        }
    }
}
