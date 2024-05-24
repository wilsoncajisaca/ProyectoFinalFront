using ProyectoFinal.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace ProyectoFinal.Vistas;

public partial class vDetalleReporte : ContentPage
{
    private double? latitud;
    private double? longitud;
    private Siniestro siniestro;
    private readonly HttpClient _httpClient = new HttpClient();

    public vDetalleReporte(Siniestro siniestro)
    {
        InitializeComponent();
        string userToken = Preferences.Get("auth_rol", string.Empty);
        if (userToken.Contains(Common.RoleAdmin))
        {
            btnAceptar.IsVisible = true;
            btnAnular.IsVisible = true;
        }

        this.siniestro = siniestro;
        MostrarDetallesSiniestroAsync();
    }

    private async Task MostrarDetallesSiniestroAsync()
    {
        img_foto.Source = siniestro.UrlFoto;
        lbl_nombre.Text = siniestro.NombreUsuario;
        lbl_Ubicacion.Text = siniestro.Ubicacion;
        lbl_Tipo.Text = siniestro.TipoSiniestro;
        lbl_Observacion.Text = siniestro.Observacion;
        lblStatus.Text = siniestro.EstadoSolicitud;
        await cargarDireccion();
    }

    private async Task cargarDireccion()
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
                lbl_Ubicacion.Text = loading + new string('.', i);
                await Task.Delay(500);
            }
        }
    }

    private async Task GetLocationAsync()
    {
        string[] partes = siniestro.Ubicacion.Split(',');
        if (partes.Length == 2)
        {
            if (double.TryParse(partes[0], out double latitud) && double.TryParse(partes[1], out double longitud))
            {
                this.latitud = latitud;
                this.longitud = longitud;
                var placemarks = await Geocoding.GetPlacemarksAsync(latitud, longitud);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    if (!string.IsNullOrWhiteSpace(placemark.Thoroughfare))
                    {
                        lbl_Ubicacion.Text = placemark.Thoroughfare + ", " + placemark.Locality + ", " + placemark.CountryName;
                    }
                    else
                    {
                        lbl_Ubicacion.Text = placemark.Locality + ", " + placemark.CountryName;
                    }
                }
                else
                {
                    lbl_Ubicacion.Text = siniestro.Ubicacion;
                }
            }
            else
            {
                lbl_Ubicacion.Text = siniestro.Ubicacion;
            }
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (latitud.HasValue && longitud.HasValue)
            {
                string url = $"https://www.google.com/maps/search/?api=1&query={latitud},{longitud}";

                await Launcher.OpenAsync(new Uri(url));
            }
            else
            {
                await DisplayAlert("Error", "Invalid coordinates", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void btnAceptar_Clicked(object sender, EventArgs e)
    {
        btnAceptar.IsVisible = false;
        loadAceptar.IsVisible = true;
        loadAceptar.IsRunning = true;
        await sendSolicitudReporte(Common.STATUS_ACTIVE);
        btnAceptar.IsVisible = true;
        loadAceptar.IsVisible = false;
        loadAceptar.IsRunning = false;
    }

    private async void btnAnular_Clicked(object sender, EventArgs e)
    {
        btnAnular.IsVisible = false;
        loadAnular.IsVisible = true;
        loadAnular.IsRunning = true;
        await sendSolicitudReporte(Common.STATUS_DECLINE);
        btnAnular.IsVisible = true;
        loadAnular.IsVisible = false;
        loadAnular.IsRunning = false;
    }

    private async Task sendSolicitudReporte(string statusRequest)
    {
        try
        {
            string token = Preferences.Get("auth_token", string.Empty);

            string url = $"{Common.BaseUrl}/appMovilesFinal/api/sinister/update";
            var updateData = new { siniestro_id = siniestro.SiniestroId, estado_solicitud = statusRequest };
            string jsonContent = JsonSerializer.Serialize(updateData);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", $"REPORTE {statusRequest}", "OK");
                await Navigation.PushAsync(new vMenu());
            }
            else
            {
                await DisplayAlert("Error", $"NO SE PUDO {statusRequest} EL REPORTE", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepción", ex.Message, "OK");
        }
    }
}