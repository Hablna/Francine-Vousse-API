using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vousse.DTO
{
    public class Billeterie_DTO
    {
        public int numero_billet { get; set; }
        public string civilite { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        //contrainte check Enfant Réduit Plein
        public decimal prix { get; set; }
        public string typeTarif{ get; set; }
        public string spectacle{ get; set; }
    }
}
