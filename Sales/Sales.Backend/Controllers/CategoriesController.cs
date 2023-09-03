using Microsoft.AspNetCore.Mvc;
using Sales.Backend.Intertfaces;
using Sales.Shared.Entities;

namespace Sales.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : GenericController<Category>
    {
        public CategoriesController(IGenericUnitOfWork<Category> unitOfWork) : base(unitOfWork)
        {
        }
    }
}