using Vousse.DTO;
using Microsoft.AspNetCore.Mvc;
using Vousse.DAL.Modeles;
using Vousse.Service;

namespace Vousse.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SpectaclesController : ControllerBase
    {
        private readonly ISpectacleService _spectacleService;

        public SpectaclesController(ISpectacleService spectacleService)
        {
            _spectacleService = spectacleService;
        }

        [HttpPost]
        public bool CreateSpectacle(Spectacle_DTO spectacle)
        {
            return _spectacleService.CreateSpectale(spectacle);
        }

        [HttpPost]
        public bool CreateBillet(Billeterie_DTO billeterie_DTO)
        {
            return _spectacleService.CreateBillet(billeterie_DTO);
        }

        [HttpPost]
        public bool CheckBillet(billetExistence_DTO billetExistence_DTO)
        {
            return _spectacleService.checkBillet(billetExistence_DTO);
        }

        [HttpGet]
        public IEnumerable<Spectacle_DTO> GetAllSpectacles()
        {
            return _spectacleService.GetAllSpectacles();
        }

        [HttpGet]
        public IEnumerable<planification_DTO> GetPlanifications(int id)
        {
            return _spectacleService.Getplanifications(id);
        }

        [HttpGet]
        public IEnumerable<statistiques_DTO> GetStatistiques(int debutSaison, int finSaison)
        {
            return _spectacleService.GetStatistiques(debutSaison, finSaison);
        }
    }
}