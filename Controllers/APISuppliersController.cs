using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projekat2.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;

namespace projekat2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class APISuppliersController : ControllerBase
    {
        private readonly MyDbContext db;
        public APISuppliersController(MyDbContext context)
        {
            db = context;
        }
        // GET: api/<APISuppliersController>
        [HttpGet]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public IEnumerable<Supplier> Get()
        {
            var suppliers = db.Suppliers.AsNoTracking().ToList();
            return suppliers;
        }

        // GET api/<APISuppliersController>/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public Supplier Get(int id)
        {
            return db.Suppliers.AsNoTracking().FirstOrDefault(x => x.SupplierId == id);
        }
    }
}
