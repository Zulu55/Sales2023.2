using Microsoft.AspNetCore.Mvc;
using Sales.Backend.Intertfaces;
using Sales.Shared.Entities;

namespace Sales.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : GenericController<City>
    {
        public CitiesController(IGenericUnitOfWork<City> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
