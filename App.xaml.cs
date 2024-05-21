using ProyectoFinal.Vistas;

namespace ProyectoFinal

{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            MainPage = new VLogin();
        }
    }
}
