using System;
using System.Collections.Generic;

namespace Vousse.DAL.Modeles;

public partial class SpectacleGrouped
{
    public int Id { get; set; }

    public int? NombreSpectacle { get; set; }

    public virtual SpectacleParent IdNavigation { get; set; } = null!;

    public virtual ICollection<Spectacle> IdSpectacles { get; set; } = new List<Spectacle>();
}
