using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vousse.DTO
{
    public class statistiques_DTO
    {
        public string NomSpectacle { get; set; }
        public int NombreBilletsPlein { get; set; }
        public int NombreBilletsReduit { get; set; }
        public int NombreBilletsEnfant { get; set; }
        public int NombreTotalBillets { get; set; }
    }
}
