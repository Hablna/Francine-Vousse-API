using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vousse.DTO;

namespace Vousse.Service
{
    public interface ISpectacleService
    {
        IEnumerable<Spectacle_DTO> GetAllSpectacles();
        IEnumerable<statistiques_DTO> GetStatistiques(int debutSaison, int finSaison);

        //creer un spectacle
        bool CreateSpectale(Spectacle_DTO spectacle);

        //recupération de la planification d'un spectacle
        IEnumerable<planification_DTO> Getplanifications(int Id);
        bool CreateBillet(Billeterie_DTO billeterie_DTO);
        bool checkBillet(billetExistence_DTO billetExistence_DTO);
        

    }
}
