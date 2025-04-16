using System;
using System.Collections.Generic;

namespace Vousse.DAL.Modeles;

public partial class BilleterieNew
{
    public int Id { get; set; }

    public string? Civilite { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? TypeTarif { get; set; }

    public int? IdSpectacle { get; set; }

    public virtual SpectacleParent? IdSpectacleNavigation { get; set; }
}
