using OrderService.Model;
using OrderService.Model.DTO;

namespace OrderService.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderHeader>> GetAllOrdersAsync(string userId = "");
        Task<OrderHeader?> GetOrderByIdAsync(int orderId);
        Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO dto);
        Task<OrderHeader?> UpdateOrderAsync(int orderId, OrderHeaderUpdateDTO dto);
        Task<OrderDetail?> UpdateOrderDetailRatingAsync(int orderDetailId, OrderDetailUpdateDTO dto);
    }
}
