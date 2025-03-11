using System;
using System.Collections.Generic;

namespace Vousse.DAL.Modeles;

public partial class Spectacle
{
    public int Id { get; set; }

    public string? TypeDeSpectacle { get; set; }

    public virtual SpectacleParent IdNavigation { get; set; } = null!;

    public virtual ICollection<SpectacleGrouped> IdSpectacleGroupeds { get; set; } = new List<SpectacleGrouped>();
}
