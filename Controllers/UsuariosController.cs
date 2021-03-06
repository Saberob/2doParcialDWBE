using ExamenDWBE.Helper;
using ExamenDWBE.Models;
using ExamenDWBE.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenDWBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        public readonly RFIDContext context;

        public UsuariosController(RFIDContext _context)
        {
            context = _context;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<IActionResult> Post(Usuarios usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }
            
            if(await context.Usuarios.Where(x => x.userName == usuario.userName).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, "el usaurio {usuario.userName} ya existe."));
            }

            HashedPassword Password = HashHelper.Hash(usuario.password);
            usuario.password = Password.Password;
            usuario.sal = Password.Salt;
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            return Ok(new UsuarioVM()
            {
                usuarioId = usuario.usuarioId,
                userName = usuario.userName
            });
        }
    }
}
