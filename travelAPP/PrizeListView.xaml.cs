using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using travelAPP.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static travelAPP.DataService;

namespace travelAPP
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PrizeListView : ContentPage
	{
		public PrizeListView ()
		{
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            if (Application.Current.Properties.ContainsKey("account") != false
                && Application.Current.Properties.ContainsKey("userTotalPoint") != false)
            {                
                account.IsVisible = true;
                account.Text = Application.Current.Properties["account"].ToString();
            }
        }       

        void btnLogout(object osender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("account"))
            {
                Application.Current.Properties.Remove("account");
                Application.Current.SavePropertiesAsync();                
            }
            if (Application.Current.Properties.ContainsKey("userTotalPoint"))
            {
                Application.Current.Properties.Remove("userTotalPoint");
                Application.Current.SavePropertiesAsync();                
            }
            
            Application.Current.Properties["isLoggedIn"]= false;
            Application.Current.SavePropertiesAsync();
            if (Device.RuntimePlatform == Device.iOS)
            {
                Navigation.PushModalAsync(new MainPage());
            }
            else
            {
                var MainPage = new NavigationPage(new MainPage());
                NavigationPage.SetHasNavigationBar(MainPage, false);
                Navigation.PushAsync(MainPage);
                Navigation.RemovePage(this);
            }
            
        }

        void btnAR(object osender, EventArgs e)
        {
            //Navigation.PushModalAsync(new ARPage());
        }

        public void MainListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Selected = e.Item as MyItemListData;
            //var myListView = (ListView)sender;
            //var myItem = myListView.SelectedItem;
            Navigation.PushModalAsync(new PrizeDetail(new PrizeDetailItem()
            {
                id = Selected.id
            }));            
        }        
    }
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;

            return ImageSource.FromResource(Source);
        }
    }
}