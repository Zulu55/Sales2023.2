using Microsoft.AspNetCore.Mvc;
using Sales.Backend.Intertfaces;

namespace Sales.Backend.Controllers
{
    public class GenericController<T> : Controller where T : class
    {
        private readonly IGenericUnitOfWork<T> _unitOfWork;

        public GenericController(IGenericUnitOfWork<T> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _unitOfWork.GetAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var row = await _unitOfWork.GetAsync(id);
            if (row == null)
            {
                return NotFound();
            }
            return Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(T model)
        {
            var result = await _unitOfWork.AddAsync(model);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(T model)
        {
            var result = await _unitOfWork.UpdateAsync(model);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
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