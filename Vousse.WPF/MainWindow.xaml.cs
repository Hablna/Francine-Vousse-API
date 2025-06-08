using System.Data;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using Vousse.DTO;

namespace Vousse.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ImportSpectacleCsv(object sender, RoutedEventArgs e)
        {
            //definir le client API (service) pour la communication avec le serveur
            var client = new Vousse.WPF.spectacle.Client();

            bool isFirstLine = true;
            int count = 0;

            // Load the CSV file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.",
                Title = "selectionner un fichier csv"
            };



            //si le fichier est selectionné
            if (openFileDialog.ShowDialog() == true)
            {
                //creation de datatable
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("titre", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("artiste1", typeof(string));
                dt.Columns.Add("artiste2", typeof(string));
                dt.Columns.Add("artiste3", typeof(string));
                dt.Columns.Add("typeDeSpectacle", typeof(string));
                dt.Columns.Add("duree", typeof(string));
                dt.Columns.Add("tarifPlein", typeof(string));
                dt.Columns.Add("tarifReduit", typeof(string));
                dt.Columns.Add("tarifEnfant", typeof(string));
                dt.Columns.Add("horaire", typeof(string));
                dt.Columns.Add("lieu", typeof(string));
                dt.Columns.Add("spectacleEnfant1", typeof(string));
                dt.Columns.Add("spectacleEnfant2", typeof(string));
                dt.Columns.Add("spectacleEnfant3", typeof(string));
                dt.Columns.Add("deconseilléAuxEnfants", typeof(string));

                List<Spectacle_DTO> spectacles = new List<Spectacle_DTO>();

                try
                {
                    //lecture du fichier csv
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var values = line.Split(';');

                            if (isFirstLine)
                            {
                                if (values.Length != 17)
                                {
                                    MessageBox.Show("Veuillez choisir un CSV valide");
                                    //arrêt du processus
                                    return;
                                }

                                isFirstLine = false;
                                continue;
                            }
                            count++;

                            //ajouter les valeurs dans le datatable jusqu'à 16
                            dt.Rows.Add(count, values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11], values[12], values[13], values[14], values[15], values[16]);

                            //si la valeur artiste est null
                            if (string.IsNullOrEmpty(values[3])) values[3] = "RAS";
                            if (string.IsNullOrEmpty(values[4])) values[4] = "RAS";
                            if (string.IsNullOrEmpty(values[5])) values[5] = "RAS";

                            spectacles.Add(new Spectacle_DTO
                            {
                                Id = count, //id du spectacle
                                Titre = values[1],
                                Description = values[2],
                                Artistes = new List<string>() { values[3], values[4], values[5] },
                                TypeDeSpectacle = values[6],
                                Duree = int.Parse(values[7]),
                                TarifPlein = decimal.Parse(values[8]),
                                TarifReduit = decimal.Parse(values[9]),
                                TarifEnfant = decimal.Parse(values[10]),
                                Horaire = DateTime.Parse(values[11]),
                                Lieu = values[12],
                                SpectacleEnfant1 = values[13],
                                SpectacleEnfant2 = values[14],
                                SpectacleEnfant3 = values[15],
                                DeconseilleAuxEnfants = int.Parse(values[16]) != 0
                            });
                        }
                        CsvSpectacleDataGrid.ItemsSource = dt.DefaultView;
                        // message de confirmation
                        reponseSpectacle.Text = $"Nombre de spectacles à créer: {spectacles.Count}. Confirmez pour enregistrer.";
                        var confirmation = MessageBox.Show($"Voulez- vous créer {spectacles.Count} spectacles ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (confirmation != MessageBoxResult.Yes) return;

                        progressBarSpectacle.Visibility = Visibility.Visible;
                        progressBarSpectacle.Minimum = 0;
                        progressBarSpectacle.Maximum = spectacles.Count;
                        progressBarSpectacle.Value = 0;
                        reponseSpectacle.Text = "Enregistrement des spectacles...";

                        List<string> errorMessages = new List<string>();
                        int successCount = 0;
                        //enregistrement des spectacles
                        await Task.Run(() =>
                        {
                            for (int i = 0; i < spectacles.Count; i++)
                            {
                                var spectacle = spectacles[i];
                                try
                                {
                                    var result = client.ApiSpectaclesCreateSpectacle(spectacle);
                                    if (result) successCount++;
                                    else errorMessages.Add($"Échec création spectacle {spectacle.Titre}");
                                }
                                catch (Exception ex)
                                {
                                    errorMessages.Add($"Erreur spectacle {spectacle.Titre} : {ex.Message}");
                                }
                                // Mise à jour de la barre dans le thread UI
                                Dispatcher.Invoke(() =>
                                {
                                    progressBarSpectacle.Value = i + 1;
                                    reponseSpectacle.Text = $"Enregistrement du spectacle {i + 1}/{spectacles.Count}";
                                });
                            }
                        });
                        progressBarSpectacle.Visibility = Visibility.Collapsed;
                        reponseSpectacle.Text = "";

                        //gestion des reponses
                        if (errorMessages.Count > 0)
                        {
                            reponseSpectacle.Text = "Erreurs lors de la création des spectacles :\n" + string.Join("\n", errorMessages);
                        }
                        else
                        {
                            reponseSpectacle.Text = $"{successCount} spectacles créés avec succès.";
                        }


                    }
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show("Impossible d’ouvrir le fichier. Il est peut-être déjà ouvert dans un autre programme (ex : Excel).\n\n" + ioEx.Message,
                                    "Fichier verrouillé", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur est survenue lors de la lecture du fichier :\n" + ex.Message,
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private async void ImportBilletterieCsv(object sender, RoutedEventArgs e)
        {
            var client = new Vousse.WPF.spectacle.Client();

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.",
                Title = "selectionner un fichier csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("civilite", typeof(string));
                dt.Columns.Add("nom", typeof(string));
                dt.Columns.Add("prenom", typeof(string));
                dt.Columns.Add("spectacle", typeof(string));
                dt.Columns.Add("horaire", typeof(string));
                dt.Columns.Add("lieu", typeof(string));
                dt.Columns.Add("prix", typeof(string));
                dt.Columns.Add("Type de tarif", typeof(string));

                List<Billeterie_DTO> billets = new List<Billeterie_DTO>();

                try
                {
                    //lecture du fichier csv
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                    {
                        //pour sauter la première ligne

                        bool isFirstLine = true;
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var values = line.Split(';');

                            if (isFirstLine)
                            {
                                if (values.Length != 9)
                                {
                                    MessageBox.Show("Veuillez choisir un CSV valide");
                                    //arrêt du processus
                                    return;
                                }

                                isFirstLine = false;
                                continue;
                            }

                            //ajouter les valeurs dans le datatable jusqu'à 16
                            dt.Rows.Add(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8]);

                            if (int.TryParse(values[0], out int numero_billet) && decimal.TryParse(values[7], out decimal prix))
                            {
                                billets.Add(new Billeterie_DTO
                                {
                                    numero_billet = numero_billet,
                                    civilite = values[1],
                                    nom = values[2],
                                    prenom = values[3],
                                    spectacle = values[4],
                                    prix = prix,
                                    typeTarif = values[8]
                                }
                                );
                            }
                        }
                        CsvBilletterieDataGrid.ItemsSource = dt.DefaultView;
                        // message de confirmation
                        reponseBilleterie.Text = $"Nombre de billets à créer: {billets.Count}. Confirmez pour enregistrer.";

                        var confirmation = MessageBox.Show($"Voulez- vous créer {billets.Count} billets ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (confirmation == MessageBoxResult.Yes)
                        {
                            progressBarBillet.Visibility = Visibility.Visible;
                            progressBarBillet.Minimum = 0;
                            progressBarBillet.Maximum = billets.Count;
                            progressBarBillet.Value = 0;
                            reponseBilleterie.Text = "Enregistrement des billets...";

                            List<string> errorMessages = new List<string>();
                            int successCount = 0;

                            await Task.Run(() =>
                            {
                                for (int i = 0; i < billets.Count; i++)
                                {
                                    var billet = billets[i];
                                    try
                                    {
                                        var result = client.ApiSpectaclesCreateBillet(billet);
                                        if (result) successCount++;
                                        else errorMessages.Add($"Échec création billet N°{billet.numero_billet}");
                                    }
                                    catch (Exception ex)
                                    {
                                        errorMessages.Add($"Erreur billet N°{billet.numero_billet} : {ex.Message}");
                                    }

                                    // Mise à jour de la barre dans le thread UI
                                    Dispatcher.Invoke(() =>
                                    {
                                        progressBarBillet.Value = i + 1;
                                        reponseBilleterie.Text = $"Enregistrement du billet {i + 1}/{billets.Count}";
                                    });
                                }
                            });

                            // Cacher le texte après traitement
                            progressBarBillet.Visibility = Visibility.Collapsed;
                            reponseBilleterie.Text = "";

                            //gestion des reponses
                            if (errorMessages.Count > 0)
                            {
                                reponseBilleterie.Text = "Erreurs lors de la création des billets :\n" + string.Join("\n", errorMessages);
                            }
                            else
                            {
                                reponseBilleterie.Text = $"{successCount} billets créés avec succès.";
                            }

                        }
                    }
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show("Impossible d’ouvrir le fichier. Il est peut-être déjà ouvert dans un autre programme (ex : Excel).\n\n" + ioEx.Message,
                                    "Fichier verrouillé", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur est survenue lors de la lecture du fichier :\n" + ex.Message,
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CheckChevauchements(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new Vousse.WPF.spectacle.Client();

                var chevauchements = client.ApiSpectaclesGetChevauchements();

                ChevauchementsDataGrid.ItemsSource = chevauchements;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des chevauchements : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

