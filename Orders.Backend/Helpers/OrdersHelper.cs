using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Shared.Entites;
using Orders.Shared.Enums;
using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly DataContext _context;

        public OrdersHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> ProcessOrderAsync(string email, string remarks)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return new Shared.Responses.Response<bool>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido"
                };
            }

            var temporalOrders = await _context.TemporalOrders
                .Include(x => x.Product)
                .Where(x => x.User!.Email == email)
                .ToListAsync();
            var response = await CheckInventoryAsync(temporalOrders);
            if (!response.WasSuccess)
            {
                return response;
            }

            Order sale = new()
            {
                Date = DateTime.UtcNow,
                User = user,
                Remarks = remarks,
                OrderDetails = new List<OrderDetail>(),
                OrderStatus = OrderStatus.New
            };

            foreach (var temporalOrder in temporalOrders)
            {
                sale.OrderDetails.Add(new OrderDetail
                {
                    Product = temporalOrder.Product,
                    Quantity = temporalOrder.Quantity,
                    Remarks = temporalOrder.Remarks,
                });

                Product? product = await _context.Products.FindAsync(temporalOrder.Product!.Id);
                if (product != null)
                {
                    product.Stock -= temporalOrder.Quantity;
                    _context.Products.Update(product);
                }

                _context.TemporalOrders.Remove(temporalOrder);
            }

            _context.Orders.Add(sale);
            await _context.SaveChangesAsync();
            return response;
        }

        private async Task<Response<bool>> CheckInventoryAsync(List<TemporalOrder> temporalOrders)
        {
            var response = new Response<bool>() { WasSuccess = true };
            foreach (var temporalOrder in temporalOrders)
            {
                Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalOrder.Product!.Id);
                if (product == null)
                {
                    response.WasSuccess = false;
                    response.Message = $"El producto {temporalOrder.Product!.Name}, ya no está disponible";
                    return response;
                }
                if (product.Stock < temporalOrder.Quantity)
                {
                    response.WasSuccess = false;
                    response.Message = $"Lo sentimos no tenemos existencias suficientes del producto {temporalOrder.Product!.Name}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.";
                    return response;
                }
            }
            return response;
        }
    }
}