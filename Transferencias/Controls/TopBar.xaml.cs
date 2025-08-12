namespace Transferencias.Controls
{
    public partial class TopBar : ContentView
    {
        public TopBar()
        {
            InitializeComponent();
            Loaded += TopBar_Loaded;
        }

        private async void TopBar_Loaded(object? sender, EventArgs e)
        {
            if (ShowUserData)
            {
                var userData = await Config.GetLoggedUser();
                UsernameLabel.Text = userData?.Codigo ?? string.Empty;
                CompleteNameLabel.Text = userData?.NombreCompleto ?? string.Empty;
            }
        }

        public bool UseLogoutButton
        {
            get { return (bool)GetValue(UseLogoutButtonProperty); }
            set { SetValue(UseLogoutButtonProperty, value); }
        }

        public static readonly BindableProperty UseLogoutButtonProperty = BindableProperty.Create(nameof(UseLogoutButton), typeof(bool), typeof(TopBar), false);


        public bool ShowUserData
        {
            get { return (bool)GetValue(ShowUserDataProperty); }
            set { SetValue(ShowUserDataProperty, value); }
        }

        public static readonly BindableProperty ShowUserDataProperty = BindableProperty.Create(nameof(ShowUserData), typeof(bool), typeof(TopBar), false);
        
        private async void LogoutButton_OnPressed(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("////Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}