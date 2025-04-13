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
                                _context.SaveChanges();

                            }
                        }
                    }
                    else // j'ajoute l'id du spectacle parent dans le spectacle simple
                        _context.Spectacles.Add(new Spectacle { Id = spectacleParent.Id, TypeDeSpectacle = spectacle.TypeDeSpectacle });
                        _context.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"message d'erreur : {ex}");
                return false;
            }
            
        }

        public IEnumerable<planification_DTO> Getplanifications(int Id)
        {
            try {
                // Récupérer les planifications du spectacle par son ID
                var planifications = _context.Planifications
                    .Where(p => p.IdSpectacle == Id);
                // Mapper les planifications vers le DTO
                if (planifications != null)
                {
                    return null;
                }
                else {
                    var planificationsDTO = planifications.Select(p => new planification_DTO
                    {
                        Id = p.Id,
                        dateSpectacle = p.DateSpectacle,
                        lieu = p.Lieu,
                        duree = p.Duree
                    }).ToList();
                    return planificationsDTO;
                }
                    
            }catch(Exception ex)
            {
                Console.WriteLine($"message d'erreur : {ex}");
                return null;
            }
        }

        public IEnumerable<Spectacle_DTO> GetAllSpectacles()
        {
            //return les spectacles
            var spectacles = _context.SpectacleParents
                .Select(s => new Spectacle_DTO
                {
                    Id = s.Id,
                    Titre = s.NomSpectacle,
                    Description = s.Descriptions,
                    TypeDeSpectacle = s.TypeDeSpectacle

                }).ToList();
            return spectacles;
        }

        public bool CreateBillet(Billeterie_DTO billeterie_DTO)
        {
            try
            {
                // Vérification de l'existence du spectacle
                var spectacle = _context.SpectacleParents
                    .FirstOrDefault(s => s.NomSpectacle == billeterie_DTO.spectacle);

                if (spectacle == null)
                {
                    Console.WriteLine($"Le spectacle '{billeterie_DTO.spectacle}' n'existe pas. Veuillez d'abord le créer.");
                    return false; // Spectacle non trouvé, retour en erreur
                }

                // Vérifier si le billet existe déjà
                var billetExist = _context.Billeteries
                    .FirstOrDefault(b => b.NumeroBillet == billeterie_DTO.numero_billet);

                if (billetExist != null)
                {
                    // Si le billet existe déjà, mettre à jour ses informations
                    billetExist.Civilite = billeterie_DTO.civilite;
                    billetExist.Nom = billeterie_DTO.nom;
                    billetExist.Prenom = billeterie_DTO.prenom;
                    billetExist.TypeTarif = billeterie_DTO.typeTarif;
                    billetExist.IdSpectacle = spectacle.Id;

                    _context.Billeteries.Update(billetExist);
                }
                else
                {
                    // Si le billet n'existe pas, le créer
                    var billet = new Billeterie
                    {
                        //problème d'incrémentation de la clé primaire
                        //NumeroBillet = billeterie_DTO.numero_billet,
                        Civilite = billeterie_DTO.civilite,
                        Nom = billeterie_DTO.nom,
                        Prenom = billeterie_DTO.prenom,
                        TypeTarif = billeterie_DTO.typeTarif,
                        IdSpectacle = spectacle.Id
                    };

                    _context.Billeteries.Add(billet);   
                }

                _context.SaveChanges();

                return true; // Billet ajouté ou mis à jour avec succès
            }
            catch (Exception ex)
            {
                // Afficher l'exception interne pour des détails plus spécifiques
                Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
                Console.WriteLine($"Détails internes : {ex.InnerException?.Message}");

                // Vous pouvez également afficher l'exception interne complète pour plus de détails
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException}");
                }

                return false; // Retourner false en cas d'erreur
            }
        }

    }
}
