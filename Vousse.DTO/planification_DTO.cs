using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vousse.DTO
{
    public class planification_DTO
    {
        public int Id { get; set; }
        public DateTime? dateSpectacle { get; set; }
        public string? lieu { get; set; }
        public int? duree { get; set; }
        public int? idSpectacle { get; set; }
    }
}
