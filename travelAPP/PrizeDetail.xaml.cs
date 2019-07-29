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
	public partial class PrizeDetail : ContentPage
	{
        CheckImage cImg = new CheckImage();
        AppValue app = new AppValue();
        string imgUrl = null;
        string prizeId = null;
        int prizePoint = 0;
        public PrizeDetail (PrizeDetailItem arg)
		{
			InitializeComponent ();

            if (NetworkCheck.IsInternet())
            {
                Show(app.url + "/AR_admin/UsergetPrizeDetailbyId/" + arg.id);
            }
            else
            {
                DisplayAlert("訊息",app.networkMessage,"ok");
            }
            
        }
        private async void Show(string url)
        {
            var userTotalPoint =await getPoint(Application.Current.Properties["account"].ToString());
            if (string.IsNullOrEmpty(userTotalPoint))
            {
                userTotalPoint = "0";
            }
            var jsonData = await GetJsonDataAsync(url);
            var posts = JsonConvert.DeserializeObject<List<PrizeDetailItem>>(jsonData);
            if (posts.Count > 0)
            {
                foreach (var postData in posts)
                {
                    prizeId = postData.id;
                    imgUrl = postData.image;
                    bool checkImage = cImg.GetImageSourceOrDefault(app.url + postData.image);
                    if (checkImage)
                    {
                        Uri uriImage = new Uri(app.url + postData.image);
                        PrizeImg.Source = ImageSource.FromUri(uriImage);
                    }
                    else
                    {
                        PrizeImg.Source = ImageSource.FromResource("travelAPP.Images.ic_notfound.png");
                    }
                    userPoint.Text = "您的總點數是:" + userTotalPoint + "點";
                    PrizeName.Text = postData.prizeName;
                    PrizeDescription.Text = postData.prizeDescription;
                    prizePoint = int.Parse(postData.point);
                    PrizePoint.Text = postData.point + "點";
                    PrizeCategoryName.Text = postData.categoryName;
                }
            }
        }

        public async Task<string> GetJsonDataAsync(string url)
        {
            string jsonContent = null;
            using (var client = new HttpClient())
            {
                var uri = url;
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    jsonContent = await response.Content.ReadAsStringAsync();
                }
            }
            return jsonContent;
        }

        async void btnExchange_Click(object sender, EventArgs args)
        {
            string userPoint =await getPoint(Application.Current.Properties["account"].ToString());
            if (string.IsNullOrEmpty(userPoint))
            {
                userPoint = "0";
            }
            
            int userTotalPoint = Convert.ToInt32(userPoint);
            if (userTotalPoint >= prizePoint)
            {                
                await Navigation.PushModalAsync(new PrizeExchange(new PrizeDetailItem()
                {
                    id = prizeId,
                    prizeName = PrizeName.Text,
                    point = prizePoint.ToString(),
                    image = imgUrl
                }));
            }
            else
            {
                await DisplayAlert("訊息", "您的點數不夠無法兌換該商品", "ok");
            }            
        }

        public async Task<string> getPoint(string userId)
        {
            string userPoint = null;
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    var uriPoint = app.url + "/AR_admin/UsergetTotalPoint/" + userId;
                    var resultPoint = await client.GetAsync(uriPoint);
                    if (resultPoint.IsSuccessStatusCode)
                    {
                        string contentPoint = await resultPoint.Content.ReadAsStringAsync();
                        //handling the answer  
                        var getPoint = JsonConvert.DeserializeObject<List<UserTotalPoint>>(contentPoint);
                        if (getPoint != null && getPoint.Count > 0)
                        {
                            foreach (var pointData in getPoint)
                            {
                                userPoint = pointData.totalPoint;
                            }
                        }
                        else
                        {
                            await DisplayAlert("訊息", "取得使用者點數資料失敗!", "ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("訊息", app.errorMessage, "ok");
                    }
                }
            }
            else
            {
                await DisplayAlert("訊息", app.networkMessage, "ok");
            }            
            return userPoint;    
        }

        private void btnBack(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PrizeListView());           
        }
    }
}