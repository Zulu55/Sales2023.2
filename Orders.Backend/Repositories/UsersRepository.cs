using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;

        public UsersRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<User>> GetAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return new Response<User>
                {
                    WasSuccess = false,
                    Message = "Usuario no encontrado"
                };
            }

            return new Response<User>
            {
                WasSuccess = true,
                Result = user
            };
        }

        public async Task<Response<User>> GetAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(x => x.Id == userId.ToString());

            if (user == null)
            {
                return new Response<User>
                {
                    WasSuccess = false,
                    Message = "Usuario no encontrado"
                };
            }

            return new Response<User>
            {
                WasSuccess = true,
                Result = user
            };
        }

        public async Task<Response<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users
                .Include(u => u.City)
                .ThenInclude(c => c!.State)
                .ThenInclude(s => s!.Country)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                    x.LastName.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new Response<IEnumerable<User>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<Response<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                    x.LastName.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return new Response<int>
            {
                WasSuccess = true,
                Result = (int)totalPages
            };
        }
    }
}