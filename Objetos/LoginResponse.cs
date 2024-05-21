using Newtonsoft.Json;

namespace ProyectoFinal.Objetos
{
    internal class LoginResponse
    {
        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("rol")]
        public string Rol { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
