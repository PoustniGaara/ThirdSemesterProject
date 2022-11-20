﻿using DataAccessLayer.Model;

namespace DataAccessLayer.Interfaces
{
    public interface ILineItemDataAccess
    {
        public Task<IEnumerable<LineItem>> GetOrderLineItems(int orderID);
        public Task CreateLineItemAsync(int orderID, LineItem item);
        public Task<bool> DeleteLineItemAsync(int orderID, LineItem item);
        public Task<bool> DeleteOrderLineItemsAsync(int orderID);
        public Task<bool> UpdateLineItemAsync(LineItem item);
    }
}