using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using travelAPP.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace travelAPP
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RatingStar : ContentPage
	{
        string prizeId = null;
        AppValue app = new AppValue();
        public RatingStar (PrizeDetailItem arg)
		{			
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            prizeId = arg.id;
            if (arg.image.Contains("/AR_admin"))
            {
                Uri uriImage = new Uri(app.url + arg.image);
                RatingPrizeImg.Source = ImageSource.FromUri(uriImage);
            }
            else
            {
                RatingPrizeImg.Source = ImageSource.FromResource(arg.image);
            }
        }
        async void btnRatingStar(object sender, EventArgs args)
        {
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new RatingStarPrize
                    {
                        userAccount = Application.Current.Properties["account"].ToString(),
                        prizeID = prizeId,
                        ratingStar = this.label.Text,
                        comment = this.editFeedback.Text
                    };
                    // create the request content and define Json  
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    //  send a POST request                  
                    var uri = app.url + "/AR_admin/UserRatingStarPrize";
                    var result = await client.PostAsync(uri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        await DisplayAlert("訊息", "感謝您寶貴的建議!", "ok");
                        if (Device.RuntimePlatform ==Device.iOS)
                        {
                            await Navigation.PushModalAsync(new PrizeListView());
                        }
                        else
                        {
                            var PrizeListView = new NavigationPage(new PrizeListView());
                            NavigationPage.SetHasNavigationBar(PrizeListView, false);
                            await Navigation.PushAsync(PrizeListView);
                            Navigation.RemovePage(this);
                        }
                    }
                    else
                    {
                        DependencyService.Get<Toast>().Show("填寫評論失敗");
                    }
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "ok");
            }            
        }
    }
}