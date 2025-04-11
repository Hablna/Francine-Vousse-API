namespace Vousse.DTO
{
    public class Spectacle_DTO
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public List<string> Artistes { get; set; }
        public string TypeDeSpectacle { get; set; }
        public int Duree { get; set; }
        public decimal TarifPlein { get; set; }
        public decimal TarifReduit { get; set; }
        public decimal TarifEnfant { get; set; }
        public DateTime Horaire { get; set; }
        public string Lieu { get; set; }
        public string SpectacleEnfant1 { get; set; }
        public string SpectacleEnfant2 { get; set; }
        public string SpectacleEnfant3 { get; set; }
        public bool DeconseilleAuxEnfants { get; set; }

    }
}
