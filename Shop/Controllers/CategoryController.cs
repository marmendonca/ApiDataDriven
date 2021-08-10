using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agente", Location = ResponseCacheLocation.Any, Duration = 30)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] -- desabilita o cache do método que você deseja
    public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")] //dessa forma estou adicionando uma restrição na rota, onde o parametro deve ser apenas inteiro.
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return Ok(category);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Post([FromBody]Category model, [FromServices]DataContext context)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível criar a categoria" });
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Put(int id, [FromBody]Category model, [FromServices]DataContext context)
    {
        try
        {
            if (id != model.Id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível atualizar a categoria" });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Delete(int id, [FromServices]DataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound("Categoria não encontrada");
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(new { message = "Categoria removida com sucesso." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível remover a categoria" });
        }
    }
}