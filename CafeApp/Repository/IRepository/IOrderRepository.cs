using CafeApp.Data;

namespace CafeApp.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<OrderHeader> CreateAsync(OrderHeader orderHeader);
        Task<OrderHeader> GetAsync(int id);
        Task<OrderHeader> GetOrderBySessionIdAsync(string sessionId);
        Task<IEnumerable<OrderHeader>> GetAllAsync(string? userId = null);
        Task<OrderHeader> UpdateStatusAsync(int orderId, string status, string paymentIntentId);
    }
}
