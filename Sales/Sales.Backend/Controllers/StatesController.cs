using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Backend.Data;
using Sales.Backend.Intertfaces;
using Sales.Shared.DTOs;
using Sales.Shared.Entities;
using Sales.Shared.Helpers;

namespace Sales.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatesController : GenericController<State>
    {
        private readonly DataContext _context;

        public StatesController(IGenericUnitOfWork<State> unitOfWork, DataContext context) : base(unitOfWork, context)
        {
            _context = context;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.States
                .Include(x => x.Cities)
                .Where(x => x.Country!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public override async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.States
                .Where(x => x.Country!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }


        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var state = await _context.States
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }
            return Ok(state);
        }
    }
}
