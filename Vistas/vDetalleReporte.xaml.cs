using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Devices.Sensors;
using ProyectoFinal.Models;


namespace ProyectoFinal.Vistas;

public partial class vDetalleReporte : ContentPage
{
    private Siniestro siniestro;

    public vDetalleReporte(Siniestro siniestro)
    {
        InitializeComponent();
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

}