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

        [JsonProperty("FechaSiniestro")]
        public string fechaSiniestro { get; set; }

    }
}
