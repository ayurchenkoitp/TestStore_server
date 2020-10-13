using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestStore_server.Models;

namespace TestStore_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        [Authorize]
        [Route("getproducts")]
        [HttpGet]
        public IActionResult GetProducts()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return Ok(db.Products.ToList());
            }
        }

        [Authorize]
        [Route("getproductbyid")]
        [HttpGet]
        public IActionResult GetProductById([FromQuery] int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                //var products = db.Products.ToList();

                var myProduct = db.Products.Find(id);
                //Product product = products.FirstOrDefault(x => x.Id == id);

                return Ok(myProduct);
            }
        }

        [Authorize]
        [Route("getcategories")]
        [HttpGet]
        public IActionResult GetCategories()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var categories = (from p in db.Products
                                 group p by p.category into c
                                 select c.Key).ToList();

                return Ok(categories);
            }
        }

        [Authorize]
        [Route("updateproduct")]
        [HttpPost]
        public IActionResult UpdateProduct([FromBody] Product productToUpdate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Products.Update(productToUpdate);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok("Saved!");
        }
    }
}
