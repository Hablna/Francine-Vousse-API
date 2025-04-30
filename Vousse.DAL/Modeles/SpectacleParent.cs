using System;
using System.Collections.Generic;

namespace Vousse.DAL.Modeles;

public partial class SpectacleParent
{
    public int Id { get; set; }

    public decimal? TarifNormal { get; set; }

    public decimal? TarifReduit { get; set; }

    public decimal? TarifEnfant { get; set; }

    public string? Descriptions { get; set; }

    public string? NomSpectacle { get; set; }

    public string? TypeDeSpectacle { get; set; }

    public bool? DeconseilleAuxEnfants { get; set; }

    public virtual ICollection<Billeterie> Billeteries { get; set; } = new List<Billeterie>();

    public virtual ICollection<Planification> Planifications { get; set; } = new List<Planification>();

    public virtual Spectacle? Spectacle { get; set; }

    public virtual SpectacleGrouped? SpectacleGrouped { get; set; }

    public virtual ICollection<Artiste> IdArtistes { get; set; } = new List<Artiste>();
}
