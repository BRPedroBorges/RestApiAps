using Microsoft.AspNetCore.Mvc;
using RestApiAps.Data;
using RestApiAps.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiAps.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostScore([FromBody] Score score)
        {
            if (score == null)
                return BadRequest("Objeto inválido.");

            _context.Scores.Add(score);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pontuação salva com sucesso!" });
        }

        [HttpGet]
        public IActionResult GetScores()
        {
            var scores = _context.Scores
                .OrderByDescending(s => s.Pontuacao)
                .ThenByDescending(s => s.DataRegistro)
                .Take(10)
                .ToList();

            return Ok(scores);
        }
    }
}
