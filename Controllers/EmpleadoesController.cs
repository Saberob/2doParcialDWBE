using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamenDWBE.Models;
using ExamenDWBE.Helper;
using Microsoft.AspNetCore.Authorization;

namespace ExamenDWBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmpleadoesController : Controller
    {
        private readonly RFIDContext context;

        public EmpleadoesController(RFIDContext _context)
        {
            context = _context;
        }

        // GET: Empleadoes
        [HttpGet]
        public async Task<IEnumerable<Empleado>> Index()
        {
            return await context.Empleado.ToListAsync();
        }

        // GET: Empleadoes/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmpleadobyId(int id)
        {
            var empleado = await context.Empleado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound(ErrorHelper.Response(404,"No existe ese id"));
            }

            return Ok(empleado);
        }
        
        // POST: api/empleadoes
        [HttpPost]
        public async Task<IActionResult> Post([Bind("Nombre,Rfid")] Empleado empleado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));            
            }

            if(await context.Empleado.Where(x => x.Rfid == empleado.Rfid).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, "el rfid ya existe."));
            }
            context.Add(empleado);
            await context.SaveChangesAsync();
            return Created($"/empleadoes/{empleado.Id}", empleado);
        }
    }
}
