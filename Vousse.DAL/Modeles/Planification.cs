using System;
using System.Collections.Generic;

namespace Vousse.DAL.Modeles;

public partial class Planification
{
    public int Id { get; set; }

    public DateTime? DateSpectacle { get; set; }

    public string? Lieu { get; set; }

    public int? Duree { get; set; }

    public int? IdSpectacle { get; set; }

    public virtual SpectacleParent? IdSpectacleNavigation { get; set; }
}
