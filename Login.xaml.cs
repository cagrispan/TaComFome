using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Facebook;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaComFome
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public string AccessToken { get; private set; }
        public DateTime TokenExpiry { get; private set; }

        public Login()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void lg_btn_ClickAsync(object sender, RoutedEventArgs e)
        {
            await Autenticar();
            
        }

        private async Task Autenticar()
        {
            FBSession sess = FBSession.ActiveSession;
            sess.FBAppId = "1726759157552802";
            sess.WinAppId = "s-1-15-2-180908069-3780063668-2257116519-1202614353-3110149805-3504969281-2720131179";

            // Get active session
            sess = FBSession.ActiveSession;

            // Add permissions required by the app
            List<String> permissionList = new List<String>();
            permissionList.Add("public_profile");
            permissionList.Add("user_location");
            permissionList.Add("user_photos");
            permissionList.Add("publish_actions");
            FBPermissions permissions = new FBPermissions(permissionList);

            // Login to Facebook
            FBResult result = await sess.LoginAsync(permissions);

            if (result.Succeeded)
            {
                // Login successful, fetch user likes
                Frame.Navigate(typeof(Mapa));
            }
            else
            {
                // Login failed
                // Do work
            }
        }
    }
}
