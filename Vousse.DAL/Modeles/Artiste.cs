using System;
using System.Collections.Generic;

namespace Vousse.DAL.Modeles;

public partial class Artiste
{
    public int IdArtiste { get; set; }

    public string Nom { get; set; } = null!;

    public virtual ICollection<SpectacleParent> IdSpectacles { get; set; } = new List<SpectacleParent>();
}
