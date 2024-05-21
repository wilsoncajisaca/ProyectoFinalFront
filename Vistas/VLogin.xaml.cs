using Newtonsoft.Json;
using ProyectoFinal.Objetos;
using System.Text;

namespace ProyectoFinal.Vistas;

public partial class VLogin : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    public VLogin()
    {
        InitializeComponent();
    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        var username = txtUser.Text;
        var password = txtPass.Text;

        var result = await LoginAsync(username, password);

        if ((bool)result["IsSuccess"])
        {
            // Manejar la respuesta exitosa aquí
            var loginData = (LoginResponse)result["Data"];

            // Guardar el token en las preferencias
            Preferences.Set("auth_token", loginData.Token);
            Preferences.Set("auth_user", loginData.Usuario);

            // Recuperar el token de las preferencias
            string userLogged = Preferences.Get("auth_user", string.Empty);
            await DisplayAlert("Alerta", $"Usuario logeado: {userLogged}", "Cerrar");
        }
        else
        {
            // Manejar la respuesta fallida aquí
            var errorMessage = (string)result["ErrorMessage"];
            Console.WriteLine($"Login failed: {errorMessage}");
            await DisplayAlert("Error", errorMessage, "OK");
        }
    }

    private async Task<Dictionary<string, object>> LoginAsync(string username, string password)
    {
        var url = "https://96a0-190-123-34-107.ngrok-free.app/appMovilesFinal/api/auth/login";
        var loginData = new { username = username, password = password };
        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var result = new Dictionary<string, object>();

        try
        {
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                result["IsSuccess"] = true;
                result["Data"] = loginResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                result["IsSuccess"] = false;
                result["ErrorMessage"] = errorResponse.Message;
            }
        }
        catch (HttpRequestException e)
        {
            // Manejo de errores en la solicitud
            Console.WriteLine($"Request exception: {e.Message}");
            result["IsSuccess"] = false;
            result["ErrorMessage"] = "Ocurrió un error al intentar conectarse al servicio.";
        }

        return result;
    }
}