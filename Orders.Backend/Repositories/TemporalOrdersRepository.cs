using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories
{
    public class TemporalOrdersRepository : GenericRepository<TemporalOrder>, ITemporalOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public TemporalOrdersRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<Response<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalOrderDTO.ProductId);
            if (product == null)
            {
                return new Response<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message = "Producto no existe"
                };
            }

            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                return new Response<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message = "Usuario no existe"
                };
            }

            var temporalOrder = new TemporalOrder
            {
                Product = product,
                Quantity = temporalOrderDTO.Quantity,
                Remarks = temporalOrderDTO.Remarks,
                User = user
            };

            try
            {
                _context.Add(temporalOrder);
                await _context.SaveChangesAsync();
                return new Response<TemporalOrderDTO>
                {
                    WasSuccess = true,
                    Result = temporalOrderDTO
                };
            }
            catch (Exception ex)
            {
                return new Response<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<IEnumerable<TemporalOrder>>> GetAsync(string email)
        {
            var temporalOrders = await _context.TemporalOrders
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .Where(x => x.User!.Email == email)
                .ToListAsync();

            return new Response<IEnumerable<TemporalOrder>>
            {
                WasSuccess = true,
                Result = temporalOrders
            };
        }

        public async Task<Response<int>> GetCountAsync(string email)
        {
            var count = await _context.TemporalOrders
                .Where(x => x.User!.Email == email)
                .SumAsync(x => x.Quantity);

            return new Response<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<Response<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO)
        {
            var currentTemporalOrder = await _context.TemporalOrders.FirstOrDefaultAsync(x => x.Id == temporalOrderDTO.Id);
            if (currentTemporalOrder == null)
            {
                return new Response<TemporalOrder>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            currentTemporalOrder!.Remarks = temporalOrderDTO.Remarks;
            currentTemporalOrder.Quantity = temporalOrderDTO.Quantity;

            _context.Update(currentTemporalOrder);
            await _context.SaveChangesAsync();
            return new Response<TemporalOrder>
            {
                WasSuccess = true,
                Result = currentTemporalOrder
            };
        }

        public override async Task<Response<TemporalOrder>> GetAsync(int id)
        {
            var temporalOrder = await _context.TemporalOrders
                .Include(ts => ts.User!)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductCategories!)
                .ThenInclude(pc => pc.Category)
                .Include(ts => ts.Product!)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (temporalOrder == null)
            {
                return new Response<TemporalOrder>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            return new Response<TemporalOrder>
            {
                WasSuccess = true,
                Result = temporalOrder
            };
        }
    }
}