using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using ProyectoFinal.Models;


namespace ProyectoFinal.Vistas;

public partial class vDetalleReporte : ContentPage
{


    private Siniestro siniestro;

    public vDetalleReporte(Siniestro siniestro)
    {
        InitializeComponent();

        // Asignar el siniestro pasado como par�metro
        this.siniestro = siniestro;

        // Mostrar los datos del siniestro en los controles de la p�gina
        MostrarDetallesSiniestro();
    }

    private void MostrarDetallesSiniestro()
    {
        // Asignar los valores del siniestro a los controles en la p�gina

        img_foto.Source = siniestro.UrlFoto;
        lbl_nombre.Text = siniestro.NombreUsuario;
        lbl_Ubicacion.Text = siniestro.Ubicacion;
        lbl_Tipo.Text = siniestro.TipoSiniestro;
        lbl_Observacion.Text = siniestro.Observacion;

        // Puedes continuar asignando los dem�s valores del siniestro a los controles seg�n sea necesario
    }

 

}