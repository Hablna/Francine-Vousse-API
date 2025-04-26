using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vousse.WPF.spectacle
{
    public partial class Client
    {
        public Client()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("C:\\Users\\habib\\source\\repos\\Vousse\\Vousse.WPF\\appsettings.json", optional: false, reloadOnChange: false);
            var config = builder.Build();

            _httpClient = new HttpClient();
            //avec l'URLWebAPI que je vais chercher dans le fichier appsettings.json
            BaseUrl = config.GetValue<string>("URLWebAPI");

            //je fais juste un copier/coller de ce qui est dans le constructeur de la classe générée
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);

        }
    }
}
