using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using travelAPP.Model;
using Xamarin.Forms;

namespace travelAPP
{
    public class DataService
    {
        public ObservableCollection<MyItemListData> MyItemList;
        public int TotlaCount = 0;
        AppValue app = new AppValue();
        //public ObservableCollection<someDataClass> MyItemList { get; set; }
        public DataService()
        {
            if (NetworkCheck.IsInternet())
            {
                using (WebClient webClient = new WebClient())
                {
                    // 指定 WebClient 的編碼
                    webClient.Encoding = Encoding.UTF8;
                    // 指定 WebClient 的 Content-Type header
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    // 從網路 url 上取得資料
                    var response = webClient.DownloadString(app.url + "/AR_admin/UserfindallPrize");
                    if (response != null && !string.IsNullOrEmpty(response))
                    {
                        //handling the answer  
                        var posts = JsonConvert.DeserializeObject<List<PrizeContent>>(response);
                        TotlaCount = posts.Count;
                        if (posts.Count > 0)
                        {
                            //postsCount = posts.Count;
                            MyItemList = new ObservableCollection<MyItemListData>();
                            foreach (var postData in posts)
                            {
                                MyItemList.Add(new MyItemListData { id = postData.id, prizeName = postData.prizeName, point = "兌換點數:" + postData.point + "點", image = app.url + postData.image });
                            }
                        }
                    }
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "ok");
                });
            }
            
        }

        public async Task<List<MyItemListData>> GetItemsAsync(int pageIndex, int pageSize)
        {
            await Task.Delay(2000);

            return MyItemList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public class MyItemListData
        {
            public string id { get; set; }
            public string categoryID { get; set; }
            public string prizeName { get; set; }
            public string point { get; set; }
            public string image { get; set; }
        }

    }
}
