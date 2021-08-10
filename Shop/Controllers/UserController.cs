using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post([FromBody]User model, [FromServices]DataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Users.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody]User model, [FromServices]DataContext context)
        {
            var user = await context.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            var token = TokenService.GenerateToken(user);

            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(int id, [FromBody]User model, [FromServices]DataContext context)
        {
            try
            {
                if (id != model.Id)
                    return NotFound(new { message = "Usuário não encontrado" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o usuário" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete(int id, [FromServices]DataContext context)
        {
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return NotFound("Usuário não encontrado");

                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Ok(new { message = "Categoria removida com sucesso." });
            }
            catch (Exception)
            {
                return BadRequest("Não foi possivel remover usuário");
            }
        }
    }
}
