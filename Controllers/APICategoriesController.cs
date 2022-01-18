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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace projekat2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class APICategoriesController : ControllerBase
    {
        private readonly MyDbContext db;
        public APICategoriesController(MyDbContext context)
        {
            db = context;
        }
        // GET: api/<APICategoriesController>
        [HttpGet]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public IEnumerable<Category> Get()
        {
            var categories = db.Categories.AsNoTracking().ToList();
            return categories;
        }

        // GET api/<APICategoriesController>/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public Category Get(int id)
        {
            return db.Categories.AsNoTracking().FirstOrDefault(x=>x.CategoryId==id);
        }

        // POST api/<APICategoriesController>
        [HttpPost]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public ActionResult Post([FromBody] Category category)
        {
            try
            {
                db.Add(category);
                db.SaveChanges();
                return Ok(category);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT api/<APICategoriesController>/5
        [HttpPut("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public ActionResult Put(int id, [FromBody] Category category)
        {
            try
            {
                db.Update(category);
                db.SaveChanges();
                return Ok(category);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE api/<APICategoriesController>/5
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [EnableCors("CorsPolicy")]
        public ActionResult Delete(int id)
        {
            var category = db.Categories.AsNoTracking().FirstOrDefault(x => x.CategoryId == id);
            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                return Ok(category);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
