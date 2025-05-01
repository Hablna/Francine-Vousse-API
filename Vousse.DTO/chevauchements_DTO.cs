using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vousse.DTO
{
    public class chevauchements_DTO
    {
        public int spectacle1_ID { get; set; }
        public int spectacle2_ID { get; set; }
        public DateTime Date_spectacle1{ get; set; }
        public DateTime Date_spectacle2 { get; set; }
        public DateTime Fin_spectacle1 { get; set; }
        public DateTime Fin_spectacle2 { get; set; }
        public string Lieu { get; set; }
    }
}
