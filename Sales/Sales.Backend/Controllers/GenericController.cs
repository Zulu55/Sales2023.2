using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.Backend.Data;
using Sales.Backend.Intertfaces;
using Sales.Shared.DTOs;
using Sales.Shared.Helpers;

namespace Sales.Backend.Controllers
{
    public class GenericController<T> : Controller where T : class
    {
        private readonly IGenericUnitOfWork<T> _unitOfWork;
        private readonly DataContext _context;
        private readonly DbSet<T> _entity;

        public GenericController(IGenericUnitOfWork<T> unitOfWork, DataContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _entity = _context.Set<T>();
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAsync([FromQuery]PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();
            return Ok(await queryable
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public virtual async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(int id)
        {
            var row = await _unitOfWork.GetAsync(id);
            if (row == null)
            {
                return NotFound();
            }
            return Ok(row);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync(T model)
        {
            var result = await _unitOfWork.AddAsync(model);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }

        [HttpPut]
        public virtual async Task<IActionResult> PutAsync(T model)
        {
            var result = await _unitOfWork.UpdateAsync(model);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(int id)
        {
            var row = await _unitOfWork.GetAsync(id);
            if (row == null)
            {
                return NotFound();
            }
            await _unitOfWork.DeleteAsync(id);
            return NoContent();
        }
    }
}