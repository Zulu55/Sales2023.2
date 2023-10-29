using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Enums;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories
{
    public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public OrdersRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<Response<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                return new Response<IEnumerable<Order>>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido",
                };
            }

            var queryable = _context.Orders
                .Include(s => s.User!)
                .Include(s => s.OrderDetails!)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == email);
            }

            return new Response<IEnumerable<Order>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderByDescending(x => x.Date)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<Response<int>> GetTotalPagesAsync(string email, PaginationDTO pagination)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                return new Response<int>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido",
                };
            }

            var queryable = _context.Orders.AsQueryable();

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == email);
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return new Response<int>
            {
                WasSuccess = true,
                Result = (int)totalPages
            };
        }

        public override async Task<Response<Order>> GetAsync(int id)
        {
            var order = await _context.Orders
                .Include(s => s.User!)
                .ThenInclude(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .Include(s => s.OrderDetails!)
                .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (order == null)
            {
                return new Response<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no existe"
                };
            }

            return new Response<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }

        public async Task<Response<Order>> UpdateFullAsync(string email, OrderDTO orderDTO)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                return new Response<Order>
                {
                    WasSuccess = false,
                    Message = "Usuario no existe"
                };
            }

            var isAdmin = await _userHelper.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin && orderDTO.OrderStatus != OrderStatus.Cancelled)
            {
                return new Response<Order>
                {
                    WasSuccess = false,
                    Message = "Solo permitido para administradores."
                };
            }

            var order = await _context.Orders
                .Include(s => s.OrderDetails)
                .FirstOrDefaultAsync(s => s.Id == orderDTO.Id);
            if (order == null)
            {
                return new Response<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no existe"
                };
            }

            if (orderDTO.OrderStatus == OrderStatus.Cancelled)
            {
                await ReturnStockAsync(order);
            }

            order.OrderStatus = orderDTO.OrderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return new Response<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }

        private async Task ReturnStockAsync(Order order)
        {
            foreach (var orderDetail in order.OrderDetails!)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetail.ProductId);
                if (product != null)
                {
                    product.Stock += orderDetail.Quantity;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}