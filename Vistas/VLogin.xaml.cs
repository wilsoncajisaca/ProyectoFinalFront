using Newtonsoft.Json;
using ProyectoFinal.Objetos;
using System.Text;

namespace ProyectoFinal.Vistas;

public partial class VLogin : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    private double? latitud;
    private double? longitud;
    public VLogin()
    {
        InitializeComponent();
    }

    private async Task GetLocationAsync()
    {
        try
        {
            // Verificar y solicitar permisos
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "No se permitio el acceso a la ubicacion, para continuar, permite el acceso desde las configuraciones del dispositivo.", "Cerrar");
                return;
            }

            // Intentar obtener la ubicación actual si no hay una conocida
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(30)
            });

            if (location != null)
            {
                latitud = location.Latitude;
                longitud = location.Longitude;
                // Obtener la dirección correspondiente a las coordenadas
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {

                    if (!string.IsNullOrWhiteSpace(placemark.Thoroughfare))
                    {
                        ubicacion.Text = placemark.Thoroughfare + ", " + placemark.Locality + ", " + placemark.CountryName;
                    }
                    else
                    {
                        ubicacion.Text = placemark.Locality + ", " + placemark.CountryName;
                    }
                }
                else
                {
                    ubicacion.Text = "Direccion no encontrada";
                }
            }
            else
            {
                ubicacion.Text = "Ubicacion no disponible";
            }
        }
        catch (Exception ex)
        {
            ubicacion.Text = "No se logro obtener la ubicacion";
        }
    }
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        btnLogin.IsVisible = false;
        activityIndicator.IsVisible = true;
        activityIndicator.IsRunning = true;
        var username = txtUser.Text;
        var password = txtPass.Text;

        var result = await LoginAsync(username, password);
        activityIndicator.IsVisible = false;
        activityIndicator.IsRunning = false;
        btnLogin.IsVisible = true;
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

            // Navegar a la nueva página HomePage
            await Navigation.PushAsync(new vMenu());;
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

    private void btnRegister_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vRegistro());
    }

    private void btnInfo_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Alerta", "Lamentamos que tengas problemas para iniciar sección, para poder ayudarte ponte en contacto con el administrador \nadmin@proyectofinal.com", "Cerrar");
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        var loadingTask = loadingLocation(_cts.Token);
        await GetLocationAsync();
        _cts.Cancel();
        await loadingTask;
    }

    private async Task loadingLocation(CancellationToken token)
    {
        string loading = "Cargando";

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < 4; i++)
            {
                if (token.IsCancellationRequested) break;

                ubicacion.Text = loading + new string('.', i);
                await Task.Delay(500);
            }
        }
    }

    private async Task OpenMapsAsync()
    {
        try
        {
            if (latitud.HasValue && longitud.HasValue)
            {

                // Construir la URL de Google Maps con las coordenadas
                string url = $"https://www.google.com/maps/search/?api=1&query={latitud},{longitud}";

                // Abrir Google Maps
                await Launcher.OpenAsync(new Uri(url));
            }
            else
            {
                // Coordenadas inválidas
                await DisplayAlert("Error", "Invalid coordinates", "OK");
            }
        }
        catch (Exception ex)
        {
            // Manejar errores
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await OpenMapsAsync();
    }
}