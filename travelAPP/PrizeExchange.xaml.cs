using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using travelAPP.Model;
using System.Net.Http;
using Newtonsoft.Json;

namespace travelAPP
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PrizeExchange : ContentPage
	{
        CheckImage cImg = new CheckImage();
        AppValue app = new AppValue();
        string prizeId = null;
        string imageSource = null;
        string exchangePoint = null;
        public PrizeExchange (PrizeDetailItem arg)
		{
			InitializeComponent ();
            this.prizeId = arg.id;
            this.PrizeName.Text = arg.prizeName;
            exchangePoint = arg.point;           
            this.PrizePoint.Text = arg.point+"點";
            if (NetworkCheck.IsInternet())
            {
                bool checkImage = cImg.GetImageSourceOrDefault(app.url + arg.image);
                if (checkImage)
                {
                    Uri uriImage = new Uri(app.url + arg.image);
                    PrizeImg.Source = ImageSource.FromUri(uriImage);
                    imageSource = arg.image;
                }
                else
                {
                    PrizeImg.Source = ImageSource.FromResource("travelAPP.Images.ic_notfound.png");
                    imageSource = "travelAPP.Images.ic_notfound.png";
                }
            }
            else
            {
                DisplayAlert("訊息", app.networkMessage, "ok");
            }            
        }

        void btnOK(object sender, EventArgs args)
        {
            EnterMemberNumber.Text = string.Empty;
            Popupoverlay.IsVisible = true;
            EnterMemberNumber.Focus();            
        }

        async void btnOKClicked(object sender, EventArgs args)
        {
            Popupoverlay.IsVisible = false;
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var postData = new MemberNumber
                    {
                        prizeId = prizeId,
                        memberNumber = EnterMemberNumber.Text                        
                    };
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var checkUri = app.url + "/AR_admin/checkMemberNumber";
                    var result = await client.PostAsync(checkUri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        var post = JsonConvert.DeserializeObject<ExchangeResult>(resultString);

                        if (post.result != null && post.result != "" && post.result == "0")
                        {
                            var postOrderData = new PrizeOrder
                            {
                                prizeId = prizeId,
                                username = Application.Current.Properties["account"].ToString(),
                                prizePoint = exchangePoint
                            };
                            var jsonOrder = JsonConvert.SerializeObject(postOrderData);
                            var contentOrder = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

                            //  send a POST request                  
                            var uri = app.url + "/AR_admin/UserExchangePrize";
                            var resultOrder = await client.PostAsync(uri, contentOrder);
                            if (resultOrder.IsSuccessStatusCode)
                            {
                                var resultOrderString = await result.Content.ReadAsStringAsync();
                                var postOrder = JsonConvert.DeserializeObject<ExchangeResult>(resultString);

                                if (postOrder.result != null && postOrder.result != "" && postOrder.result == "0")
                                {
                                    Application.Current.Properties["userTotalPoint"] = Convert.ToInt32(Application.Current.Properties["userTotalPoint"]) - int.Parse(exchangePoint);
                                    await Application.Current.SavePropertiesAsync();
                                    await DisplayAlert("訊息", "兌換成功", "Ok");
                                    var prizeItem = new PrizeDetailItem
                                    {
                                        id = prizeId,
                                        image = imageSource
                                    };

                                    var RatingStar = new NavigationPage(new RatingStar(prizeItem));
                                    NavigationPage.SetHasNavigationBar(RatingStar, false);
                                    await Navigation.PushModalAsync(RatingStar);
                                }
                                else
                                {
                                    await DisplayAlert("訊息", "兌換失敗!請稍候再試", "Ok");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("訊息", "輸入的廠商統編錯誤", "Ok");
                            EnterMemberNumber.Text = string.Empty;
                            Popupoverlay.IsVisible = true;
                            EnterMemberNumber.Focus();
                        }
                    }
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "ok");
            }            
        }

        void btnCancelClicked(object sender, EventArgs args)
        {
            Popupoverlay.IsVisible = false;
        }
    }
}