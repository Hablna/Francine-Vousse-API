using System.Data;
using System.IO;
using System.Text;
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

namespace Vosse.WPF
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

        private void importSpectacle(object sender, RoutedEventArgs e)
        {
            //definir le client API (service) pour la communication avec le serveur

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
                    //pour sauter la première ligne
                    bool isFirstLine = true;
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var values = line.Split(';');

                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }
                        //ajouter les valeurs dans le datatable jusqu'à 16
                        dt.Rows.Add(values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11], values[12], values[13], values[14], values[15], values[16]);
                    }
                    //lier le datatable au datagrid
                    CsvDataGrid.ItemsSource = dt.DefaultView;
                }
            }
        
        }
    }
}