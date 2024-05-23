using Newtonsoft.Json;
using System.Net.Http.Headers;
using ProyectoFinal.Models;

namespace ProyectoFinal.Vistas;

public partial class vReporteCliente : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    private Siniestro siniestroSeleccionado;
    private Siniestro siniestro;
    private Boolean isAllData = false;

    public vReporteCliente(Boolean isAllData)
    {
        InitializeComponent();
        this.isAllData = isAllData;
        InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        var loadingTask = loadingLocation(_cts.Token);
        await LoadSiniestrosAsync();
        _cts.Cancel();
        await loadingTask;
        lblMessage.IsVisible = false;
    }

    private async Task LoadSiniestrosAsync()
    {
        try
        {
            string token = Preferences.Get("auth_token", string.Empty);

            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Advertencia", "Por favor vuelve a iniciar sesion", "OK");
                await Navigation.PushAsync(new VLogin());
            }
            string url = $"{Common.BaseUrl}/appMovilesFinal/api/sinister/getAllSinisterByPartner";
            if (isAllData)
            {
                url = $"{Common.BaseUrl}/appMovilesFinal/api/sinister/getAllSinister";
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Siniestro> siniestros = JsonConvert.DeserializeObject<List<Siniestro>>(responseBody);
                foreach (var siniestro in siniestros)
                {
                    siniestro.BackgroundColor = ConvertHexToColor(siniestro.ColorFondo);
                }

                if (siniestros == null || siniestros.Count == 0)
                {
                    lblMessage.Text = "No se encontraron siniestros.";
                }
                else
                {
                    lblMessage.Text = string.Empty;
                    collectionViewSiniestro.ItemsSource = siniestros;
                }
            }
            else
            {
                lblMessage.Text = $"No se pudo obtener la información: {response.StatusCode}";
            }
        }
        catch (HttpRequestException e)
        {
            lblMessage.Text = $"Se produjo un error en la solicitud HTTP: {e.Message}";
        }
        catch (Exception e)
        {
            lblMessage.Text = $"Se produjo un error: {e.Message}";
        }
    }

    private async Task loadingLocation(CancellationToken token)
    {
        string loading = "Cargando";

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < 4; i++)
            {
                if (token.IsCancellationRequested) break;

                lblMessage.Text = loading + new string('.', i);
                await Task.Delay(500);
            }
        }
    }

    private Color ConvertHexToColor(string hex)
    {
        if (!string.IsNullOrEmpty(hex))
        {
            return Color.FromArgb(hex);
        }
        return Colors.Transparent;
    }

    private async void collectionViewSiniestro_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedSiniestro = e.CurrentSelection.FirstOrDefault() as Siniestro;
        if (selectedSiniestro != null)
        {
            await Navigation.PushAsync(new vDetalleReporte(siniestro));
        }
    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var siniestro = frame.BindingContext as Siniestro;
        if (siniestro != null)
        {
            await Navigation.PushAsync(new vDetalleReporte(siniestro));
        }
    }
}