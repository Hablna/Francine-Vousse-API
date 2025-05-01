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

        private void ImportSpectacleCsv(object sender, RoutedEventArgs e)
        {
            //definir le client API (service) pour la communication avec le serveur
            var client = new Vousse.WPF.spectacle.Client();

            bool isFirstLine = true;
            int count = 0;
            List<string> errorMessages = new List<string>();

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

                //lecture du fichier csv
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {

                    // Afficher la ProgressBar
                    progressBar.Visibility = Visibility.Visible;
                    progressText.Visibility = Visibility.Visible;

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var values = line.Split(';');

                        if (isFirstLine)
                        {
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

                        var spectacle = new Spectacle_DTO
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
                        };
                        try
                        {
                            //envoi du spectacle au serveur
                            var resultSpectacle = client.ApiSpectaclesCreateSpectacle(spectacle);
                            if (!resultSpectacle)
                            {
                                errorMessages.Add($"Erreur lors de la création du spectacle N°{spectacle.Id}");
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Erreur au niveau du spectacle {spectacle.Id} : {ex.Message}");
                        }
                        //lier le datatable au datagrid
                        CsvSpectacleDataGrid.ItemsSource = dt.DefaultView;

                    }

                    progressBar.Visibility = Visibility.Hidden;

                    //gestion d'erreur
                    if (errorMessages.Count > 0)
                    {
                        reponseSpectacle.Text = "Erreurs lors de la création des spectacles :\n" + string.Join("\n", errorMessages);
                    }
                    else
                    {
                        reponseSpectacle.Text = "Tous les spectacles ont été créés avec succès.";
                    }
                }
            }
        }

        private void ImportBilletterieCsv(object sender, RoutedEventArgs e)
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

                //lecture du fichier csv
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    //pour sauter la première ligne
                    List<string> errorMessages = new List<string>();
                    bool isFirstLine = true;
                    int count = 0;
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var values = line.Split(';');

                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }
                        count++;
                        //ajouter les valeurs dans le datatable jusqu'à 16
                        dt.Rows.Add(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8]);

                        var billet = new Billeterie_DTO
                        {
                            numero_billet = int.Parse(values[0]),
                            civilite = values[1],
                            nom = values[2],
                            prenom = values[3],
                            spectacle = values[4],
                            prix = decimal.Parse(values[7]),
                            typeTarif = values[8]
                        };
                        try
                        {
                            var resultBillet = client.ApiSpectaclesCreateBillet(billet);
                            if (!resultBillet)
                            {
                                errorMessages.Add($"Erreur lors de la création du billet N°{billet.numero_billet}");
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Erreur au niveau du billet {billet.numero_billet} : {ex.Message}");

                        }
                        //lier le datatable au datagrid
                        CsvBilletterieDataGrid.ItemsSource = dt.DefaultView;


                    }
                    //gestion d'erreur
                    if (errorMessages.Count > 0)
                    {
                        reponseBilleterie.Text = "Erreurs lors de la création des billets :\n" + string.Join("\n", errorMessages);
                    }
                    else
                    {
                        reponseBilleterie.Text = "Tous les billets ont été créés avec succès.";
                    }
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