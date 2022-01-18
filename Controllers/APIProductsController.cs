using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using projekat2.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace projekat2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class APIProductsController : ControllerBase
    {
        private readonly MyDbContext db;
        public APIProductsController(MyDbContext context)
        {
            db = context;
        }
        // GET: api/<APIProductsController>
        [HttpGet]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public IEnumerable<Product> Get()
        {
            var products = db.Products.AsNoTracking().ToList();
            return products;
        }

        // GET api/<APIProductsController>/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public Product Get(int id)
        {
            return db.Products.AsNoTracking().FirstOrDefault(x => x.ProductId == id);
        }

        // POST api/<APIProductsController>
        [HttpPost]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public ActionResult Post([FromBody] Product product)
        {
            try
            {
                db.Add(product);
                db.SaveChanges();
                return Ok(product);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT api/<APIProductsController>/5
        [HttpPut("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public ActionResult Put(int id, [FromBody] Product product)
        {
            try
            {
                db.Update(product);
                db.SaveChanges();
                return Ok(product);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE api/<APIProductsController>/5
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public ActionResult Delete(int id)
        {
            var product = db.Products.AsNoTracking().FirstOrDefault(m => m.ProductId == id);
            try
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return Ok(product);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
