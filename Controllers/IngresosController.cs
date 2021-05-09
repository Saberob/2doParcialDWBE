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
using ExamenDWBE.Models.ViewModels;

namespace ExamenDWBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IngresosController : Controller
    {
        private readonly RFIDContext context;

        public IngresosController(RFIDContext _context)
        {
            context = _context;
        }

        [HttpGet]
        // GET: Ingresos
        public async Task<IEnumerable<Ingresos>> Index()
        {
            return await context.Ingresos.ToListAsync();
        }

        [HttpGet("{id}")]
        // GET: Ingresos/Details/5
        public async Task<IActionResult> GetRegistroById(int id)
        {
            var ingreso = await context.Ingresos
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (ingreso == null)
            {
                return NotFound(ErrorHelper.Response(404, "No existe ese id"));
            }

            return Ok(ingreso);
        }

        [HttpGet("byEmp/{id}")]
        // GET: Ingresos/Details/5
        public async Task<IActionResult> GetRegistroByEmpleadoId(int id)
        {
            var empleado = await context.Empleado.FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound(ErrorHelper.Response(404, "No existe ese id de empleado"));
            }

            var request = await context.Ingresos.Where(x => x.EmpleadoId == empleado.Id).ToListAsync();
            var ingresos = new List<IngresoVM>();

            foreach(Ingresos item in request){
                ingresos.Add(new IngresoVM
                {
                    RegistroId = item.RegistroId,
                    Fecha = item.Fecha
                });
            }

            return Ok(new IngresoEmpleadoVM
            {
                EmpleadoId = id,
                Nombre = empleado.Nombre,
                ingresos = ingresos
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([Bind("EmpleadoId")] Ingresos ingreso)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }
            
            ingreso.Fecha = DateTime.Now;
            context.Add(ingreso);
            await context.SaveChangesAsync();
            return Created($"/ingresos/{ingreso.RegistroId}", ingreso);
            
        }
    }
}
