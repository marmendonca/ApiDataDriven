using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices]DataContext context)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices]DataContext context, int id)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.Category.Id == id).ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices]DataContext context)
        {
            var product = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(product);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post([FromBody]Product model, [FromServices]DataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Products.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o produto" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> Put(int id, Product model, [FromServices] DataContext context)
        {
            try
            {
                if (id != model.Id)
                    return NotFound(new { message = "Produto não encontrada" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o produto" });
            }
        }
    }
}
