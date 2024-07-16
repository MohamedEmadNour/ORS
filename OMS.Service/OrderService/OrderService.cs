using OMS.Data.Entites;
using OMS.Repositores.Interfaces;

namespace OMS.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void UpdateStock(OrderItem item, int quantity)
        {
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            item.Quantity -= quantity;
            if (item.Quantity < 0) throw new InvalidOperationException("Insufficient stock.");
        }

        public async Task<bool> ValidateStockAsync(Order order)
        {

            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.repositories<Product, int>().GetByIdAsync(item.ProductId);
                if (product.Stock < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        public decimal ApplyDiscount(Order order)
        {
            decimal discount = 0;
            if (order.TotalAmount > 200)
            {
                discount = 0.10m;
            }
            else if (order.TotalAmount > 100)
            {
                discount = 0.05m;
            }

            return order.TotalAmount * (1 - discount);
        }
    }

}
