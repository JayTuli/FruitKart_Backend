using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Model;
using OrderService.Repository;
using OrderService.Model.DTO;

namespace OrderService.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _db;
 
        public OrderRepository(OrderDbContext db)
        {
            _db = db;
        }
 
        // GET ALL — filter by userId if provided
        public async Task<IEnumerable<OrderHeader>> GetAllOrdersAsync(string userId = "")
        {
            var query = _db.OrderHeader
                           .Include(u => u.OrderDetails)
                           .OrderByDescending(u => u.OrderHeaderId)
                           .AsQueryable();
 
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(u => u.ApplicationUserId == userId);
 
            return await query.ToListAsync();
        }
 
        // GET BY ID
        public async Task<OrderHeader?> GetOrderByIdAsync(int orderId)
        {
            return await _db.OrderHeader
                            .Include(u => u.OrderDetails)
                            .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
        }
 
        // CREATE — manual mapping, no AutoMapper needed
        public async Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO dto)
        {
            OrderHeader orderHeader = new()
            {
                PickUpName        = dto.PickUpName,
                PickUpPhoneNumber = dto.PickUpPhoneNumber,
                PickUpEmail       = dto.PickUpEmail,
                OrderDate         = DateTime.Now,
                OrderTotal        = dto.OrderTotal,
                Status            = "Confirmed",
                TotalItem         = dto.TotalItem,
                ApplicationUserId = dto.ApplicationUserId
            };
 
            _db.OrderHeader.Add(orderHeader);
            await _db.SaveChangesAsync();
 
            foreach (var detailDTO in dto.OrderDetailDTO)
            {
                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = orderHeader.OrderHeaderId,
                    MenuItemId    = detailDTO.MenuItemId,
                    Quantity      = detailDTO.Quantity,
                    ItemName      = detailDTO.ItemName,
                    Price         = detailDTO.Price
                };
                _db.OrderDetail.Add(orderDetail);
            }
 
            await _db.SaveChangesAsync();
 
            // Clear nav property before returning — matches original behaviour
            orderHeader.OrderDetails = [];
            return orderHeader;
        }
 
        // UPDATE ORDER HEADER — patch fields manually, same status logic as monolithic
        public async Task<OrderHeader?> UpdateOrderAsync(int orderId, OrderHeaderUpdateDTO dto)
        {
            var order = await _db.OrderHeader.FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
            if (order is null) return null;
 
            if (!string.IsNullOrEmpty(dto.PickUpName))
                order.PickUpName = dto.PickUpName;
 
            if (!string.IsNullOrEmpty(dto.PickUpPhoneNumber))
                order.PickUpPhoneNumber = dto.PickUpPhoneNumber;
 
            if (!string.IsNullOrEmpty(dto.PickUpEmail))
                order.PickUpEmail = dto.PickUpEmail;
 
            if (!string.IsNullOrEmpty(dto.Status))
            {
                // Confirmed → ReadyForPickup
                if (order.Status.Equals("Confirmed", StringComparison.InvariantCultureIgnoreCase)
                    && dto.Status.Equals("ReadyForPickup", StringComparison.InvariantCultureIgnoreCase))
                {
                    order.Status = "ReadyForPickup";
                }
 
                // ReadyForPickup → Completed
                if (order.Status.Equals("ReadyForPickup", StringComparison.InvariantCultureIgnoreCase)
                    && dto.Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    order.Status = "Completed";
                }
 
                // Any → Cancelled
                if (dto.Status.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase))
                {
                    order.Status = "Cancelled";
                }
            }
 
            await _db.SaveChangesAsync();
            return order;
        }
 
        // UPDATE RATING — just one field, no mapping needed
        public async Task<OrderDetail?> UpdateOrderDetailRatingAsync(int orderDetailId, OrderDetailUpdateDTO dto)
        {
            var detail = await _db.OrderDetail.FirstOrDefaultAsync(u => u.OrderDetailId == orderDetailId);
            if (detail is null) return null;
 
            detail.Rating = dto.Rating;
            await _db.SaveChangesAsync();
            return detail;
        }
    }
}
