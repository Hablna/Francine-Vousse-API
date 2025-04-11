using Vousse.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vousse.DAL.Modeles;
using System.Collections.Generic;
using System.Linq;

namespace Vousse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpectaclesController : ControllerBase
    {
        private readonly VousseContext _context;

        public SpectaclesController(VousseContext context)
        {
            _context = context;
        }

        // POST: api/spectacles
        [HttpPost]
        public IActionResult CreateSpectacle([FromBody] Spectacle_DTO spectacle)
        {
            if (spectacle == null)
            {
                return BadRequest("Le spectacle ne peut pas être nul.");
            }

            //TODO _context.Spectacles.Add(spectacle);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetSpectacleById), new { id = spectacle.Id }, spectacle);
        }

        // GET: api/spectacles
        [HttpGet]
        public ActionResult<IEnumerable<Spectacle>> GetSpectacles()
        {
            var spectacles = _context.Spectacles.ToList();
            return Ok(spectacles);
        }
       
        // GET: api/spectacles/{id}
        [HttpGet("{id}")]
        public ActionResult<Spectacle> GetSpectacleById(int id)
        {
            var spectacle = _context.Spectacles.Find(id);

            if (spectacle == null)
            {
                return NotFound();
            }

            return Ok(spectacle);
        }
    }
}