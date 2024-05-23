using ProyectoFinal.Models;

namespace ProyectoFinal.Vistas;

public partial class vMenu : ContentPage
{
	public vMenu()
	{
		InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        this.cargarData();
    }

    private void cargarData()
    {
        string userLogged = Preferences.Get("auth_user", string.Empty);
        string userRole = Preferences.Get("auth_rol", string.Empty);
        Title = $"Bienvenido {userLogged}";
        if (userRole.Equals(Common.RoleAdmin)) 
        {
            lbl_all_historial.IsVisible = true;
            frm_all_historial.IsVisible = true;
        }
    }

    private void btn_historial_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vReporteCliente(false));
    }

    private async void btn_reportar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vReporteSiniestro());
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        Preferences.Remove("auth_token");
        Preferences.Remove("auth_user");
        Preferences.Remove("auth_rol");
        Navigation.PushAsync(new VLogin());
    }

    private void btn_all_historial_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new vReporteCliente(true));
    }
}