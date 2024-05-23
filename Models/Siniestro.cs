using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Models
{
    public class Siniestro
    {
         [JsonProperty("siniestroId")] 
         public string SiniestroId { get; set; }

         [JsonProperty("ubicacion")]
         public string Ubicacion { get; set; }

         [JsonProperty("observacion")]
         public string Observacion { get; set; }

         [JsonProperty("tipoSiniestro")]
         public string TipoSiniestro { get; set; }

         [JsonProperty("fechaSiniestro")]
         public string FechaSiniestro { get; set; }

        [JsonProperty("cedulaUsuario")]
        public string CedulaUsuario { get; set; }

        [JsonProperty("nombreUsuario")]
        public string NombreUsuario { get; set; }

        [JsonProperty("colorFondo")]
        public string ColorFondo { get; set; }

        [JsonProperty("urlFoto")]
        public string UrlFoto { get; set; }

        public Color BackgroundColor { get; set; }

    }
}
