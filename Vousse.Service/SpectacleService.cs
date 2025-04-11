using Vousse.DAL.Modeles;
using Vousse.DTO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Vousse.Service
{
    public class SpectacleService : ISpectacleService
    {
        private readonly VousseContext _context;

        public SpectacleService(VousseContext context)
        {
            _context = context;
        }


        public bool CreateSpectale(Spectacle_DTO spectacle)
        {
            // verifier si le spectacle existe déjà
            // si oui creer une nouvelle planification en prenant en compte les chevauchements
            // si non d'abord creer le spectacle parent puis la planification
            // puis tester si chaque ariste existe déjà si oui on ne le cree pas si non on le cree
            // puis recuperer l'id de chaque artiste et l'ajouter dans la table intermédiaire
            // on teste si c'est un spectacle groupé, si oui on lit spectacle enfant et spectacle grouped dans la table intermediaire

            try
            {
                // test l'existence du spectacle avec son nom
                var spectacleExist = _context.SpectacleParents
                    .FirstOrDefault(s => s.NomSpectacle.Trim() == spectacle.Titre);

                // création d'une nouvelle planification si le spectacle existe déjà
                if (spectacleExist != null)
                {
                    var planification = new Planification
                    {
                        DateSpectacle = spectacle.Horaire,
                        Lieu = spectacle.Lieu,
                        Duree = spectacle.Duree,
                        //création de la clé étrangère grâce à la clé primaire de la table spectacle_parent 
                        IdSpectacle = spectacleExist.Id
                    };
                    _context.Planifications.Add(planification);
                    _context.SaveChanges();
                    //Ici la gestion des chevauchements
                }

                //création d'un nouveau spectacle parent si le spectacle n'existe pas
                else
                {
                    var spectacleParent = new SpectacleParent
                    {
                        NomSpectacle = spectacle.Titre,
                        Descriptions = spectacle.Description,
                        TarifNormal = spectacle.TarifPlein,
                        TarifReduit = spectacle.TarifReduit,
                        TarifEnfant = spectacle.TarifEnfant,
                        DeconseilleAuxEnfants = spectacle.DeconseilleAuxEnfants,
                        TypeDeSpectacle = spectacle.TypeDeSpectacle
                    };
                    _context.SpectacleParents.Add(spectacleParent);
                    _context.SaveChanges(); // ce qui me retourne l'id du spectacle parent créé

                    // création de la planification
                    var planification = new Planification
                    {
                        DateSpectacle = spectacle.Horaire,
                        Lieu = spectacle.Lieu,
                        Duree = spectacle.Duree,
                        //création de la clé étrangère grâce à la clé primaire de la table spectacle_parent 
                        IdSpectacle = spectacleParent.Id,
                    };
                    _context.Planifications.Add(planification);

                    //enregistrement des artistes
                    foreach (var artisteName in spectacle.Artistes)// la valeur Artistes est sous forme de liste
                    {
                        var artisteExiste = _context.Artistes
                            .FirstOrDefault(a => a.Nom.Trim() == artisteName);

                        Artiste artiste;

                        if (artisteExiste == null)
                        {
                            artiste = new Artiste
                            {
                                Nom = artisteName
                            };
                            _context.Artistes.Add(artiste);
                            _context.SaveChanges(); // ce qui me retourne l'id de l'artiste créé
                        }

                        else
                        {
                            artiste = artisteExiste;
                        }
                        // j'insère dans la table intermédiaire
                        spectacleParent.IdArtistes.Add(artiste);
                        artiste.IdSpectacles.Add(spectacleParent);

                        _context.SaveChanges();

                    }

                    //gestion des spectacles simples et groupés
                    if (!string.IsNullOrEmpty(spectacle.SpectacleEnfant1) /**||
                        !string.IsNullOrEmpty(spectacle.SpectacleEnfant1) ||
                        !string.IsNullOrEmpty(spectacle.SpectacleEnfant1**/)
                    {
                        List<string> spectaclesEnfants = new List<string>() { spectacle.SpectacleEnfant1, spectacle.SpectacleEnfant2, spectacle.SpectacleEnfant3};

                        // on teste pour chaque spectacle enfant si il existe déjà
                        int nbrSpectacle= 0;
                        foreach (var spectacleEnfant in spectaclesEnfants)
                        {
                            var spectacleEnfantExist = _context.SpectacleParents
                                .FirstOrDefault(s => s.NomSpectacle == spectacleEnfant);
                            if (spectacleEnfantExist == null)
                                //on passe à la boucle suivante
                                Console.WriteLine($"le spectacle enfant '{spectacleEnfant}' n'existe pas, veuillez d'abord le creer");
                            else {
                                nbrSpectacle++;
                                

                                //spectacle enfant
                                var spectacleEnfan = new Spectacle
                                {
                                    Id = spectacleEnfantExist.Id

                                };

                                var spectacleGrouped = new SpectacleGrouped
                                {
                                    Id = spectacleEnfantExist.Id,
                                    NombreSpectacle = nbrSpectacle,
                                };
                                _context.SpectacleGroupeds.Add(spectacleGrouped);
                                
                                //Table intermédiaire                                    
                                spectacleGrouped.IdSpectacles.Add(spectacleEnfan);
                                spectacleEnfan.IdSpectacleGroupeds.Add(spectacleGrouped);

                            }
                        }
                    }
                    else // j'ajoute l'id du spectacle parent dans le spectacle simple
                        _context.Spectacles.Add(new Spectacle { Id = spectacleParent.Id, TypeDeSpectacle = spectacle.TypeDeSpectacle });
                    
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"message d'erreur : {ex}");
                return false;
            }
            
        }

        public IEnumerable<planification_DTO> GetHoraires(Spectacle_DTO spectacle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Spectacle_DTO> GetAllSpectacles()
        {
            throw new NotImplementedException();
        }
    }

}
