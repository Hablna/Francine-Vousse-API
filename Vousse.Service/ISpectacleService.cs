using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vousse.DTO;

namespace Vousse.Service
{
    internal interface ISpectacleService
    {
        IEnumerable<Spectacle_DTO> GetAllSpectacles();

        //creer un spectacle
        bool CreateSpectale(Spectacle_DTO spectacle);

        //recupération de la planification d'un spectacle
        IEnumerable<planification_DTO> Getplanifications(int Id);
        bool CreateBillet(Billeterie_DTO billeterie_DTO);

    }
}
