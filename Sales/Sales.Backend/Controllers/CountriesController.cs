﻿using Microsoft.AspNetCore.Mvc;
using Sales.Backend.Intertfaces;
using Sales.Shared.Entities;

namespace Sales.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : GenericController<Country>
    {
        public CountriesController(IGenericUnitOfWork<Country> unitOfWork) : base(unitOfWork)
        {
        }
    }
}