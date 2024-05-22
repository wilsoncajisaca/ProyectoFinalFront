using Newtonsoft.Json;
using ProyectoFinal.Models;
using System.Net.Http.Headers;

namespace ProyectoFinal.Vistas;

public partial class vReporteSiniestro : ContentPage
{

    private double? latitud;
    private double? longitud;
    private FileResult photo;

    public vReporteSiniestro()
	{
		InitializeComponent();
        CargarTiposAsync();

    }

   

    private async void btn_camra_Clicked(object sender, EventArgs e)
    {

        // Código para mostrar la foto
        photo = await MediaPicker.CapturePhotoAsync();
        if (photo != null)
        {

            // Muestra la foto en la interfaz
            var stream = await photo.OpenReadAsync();
            btn_camara.Source = ImageSource.FromStream(() => stream);
        }

       
    }
    
    private async void btnUbicacion_Clicked(object sender, EventArgs e)
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

                btnUbicacion.Text = loading + new string('.', i);
                await Task.Delay(500);
            }
        }
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
                        btnUbicacion.Text = placemark.Thoroughfare + ", " + placemark.Locality + ", " + placemark.CountryName;
                    }
                    else
                    {
                        btnUbicacion.Text = placemark.Locality + ", " + placemark.CountryName;
                    }
                }
                else
                {
                    btnUbicacion.Text = "Direccion no encontrada";
                }
            }
            else
            {
                btnUbicacion.Text = "Ubicacion no disponible";
            }
        }
        catch (Exception ex)
        {
            btnUbicacion.Text = "No se logro obtener la ubicacion";
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

    private async void btnGaleria_Clicked(object sender, EventArgs e)
    {
        try
        {
            photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Selecciona una foto"
            });

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                btn_camara.Source = ImageSource.FromStream(() => stream);

            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Error", "Esta característica no es soportada en este dispositivo.", "OK");
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Error", "Permiso denegado para acceder a la galería.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private async Task CargarTiposAsync()
    {
        try
        {
            // Obtener los datos desde el servicio web
            var tipos = await ObtenerTiposDesdeServicioWeb();

            // Asignar los datos obtenidos al Picker
            pkTipo.ItemsSource = tipos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudieron cargar los tipos: " + ex.Message, "OK");
        }
    }

    private async Task<List<TipoSiniestro>> ObtenerTiposDesdeServicioWeb()
    {
        using (var client = new HttpClient())
        {
            // URL del servicio web
            var url = "https://96a0-190-123-34-107.ngrok-free.app/appMovilesFinal/api/sinisterType/getAllSinisterType";
            string userToken = Preferences.Get("auth_token", string.Empty);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            // Realizar la solicitud GET al servicio web
            var response = await client.GetStringAsync(url);

            // Parsear la respuesta JSON a una lista de SinisterType
            var tipos = JsonConvert.DeserializeObject<List<TipoSiniestro>>(response);

            return tipos;
        }
    }
    private async void btnSiniestro_Clicked(object sender, EventArgs e)
    {
        if (photo != null && latitud.HasValue && longitud.HasValue)
        {
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            // Obtén la extensión del archivo
            string extension = Path.GetExtension(photo.FileName);


            // Convertir la foto a un Stream
            using (var stream = await photo.OpenReadAsync())
            {
                // Crear cliente HTTP
                using (var client = new HttpClient())
                {
                    // Configurar la dirección del servicio
                    client.BaseAddress = new Uri("https://96a0-190-123-34-107.ngrok-free.app");
                    string userToken = Preferences.Get("auth_token", string.Empty);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

                   
                    // Convertir la imagen a un formato adecuado para enviarla al servidor
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(stream), "file", $"photo{extension}");

                    var selectedSinisterType = (TipoSiniestro)pkTipo.SelectedItem;

                    if (string.IsNullOrWhiteSpace(txtObservacion.Text)) 
                        {
                        await DisplayAlert("Alerta", "Ingrese la observación", "OK");
                        return;
                        }

                    // Agregar otros parámetros si es necesario
                    content.Add(new StringContent(txtObservacion.Text), "descripcion");
                    content.Add(new StringContent($"{latitud},{longitud}"), "ubicacionSiniestro");
                    content.Add(new StringContent(selectedSinisterType.SinisterTypeId), "tipoSiniestro");

                    // Enviar la solicitud POST al servicio
                    var response = await client.PostAsync("/appMovilesFinal/api/sinister/createWithPhoto", content);

                    // Verificar si la solicitud fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        activityIndicator.IsVisible = false;
                        activityIndicator.IsRunning = false;
                        var responseContent = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Éxito", "Siniestro Registrado", "OK");
                    }
                    else
                    {
                        activityIndicator.IsVisible = false;
                        activityIndicator.IsRunning = false;
                        await DisplayAlert("Error", "No se pudo subir la imagen", "OK");
                    }
                }
            }
        }
        else
        {
            await DisplayAlert("Advertencia", "Primero debe obtener la ubicacion del dispositivo", "OK");
        }
    }

}