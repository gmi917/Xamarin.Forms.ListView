using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using travelAPP.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static travelAPP.DataService;

namespace travelAPP
{
    public partial class MainPage : ContentPage
    {        
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            if (Application.Current.Properties.ContainsKey("isLoggedIn") &&
                Convert.ToBoolean(Application.Current.Properties["isLoggedIn"].ToString())==true)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Navigation.PushModalAsync(new PrizeListView());
                }
                else
                {
                    var PrizeListView = new NavigationPage(new PrizeListView());
                    NavigationPage.SetHasNavigationBar(PrizeListView, false);
                    Navigation.PushAsync(PrizeListView);
                    Navigation.RemovePage(this);
                }
            }
            else
            {                
                Application.Current.Properties.Clear();
                Application.Current.SavePropertiesAsync();
            }                        
        }

        private async void btnLogin(object osender, EventArgs e)
        {
            AppValue app = new AppValue();
            if (!string.IsNullOrEmpty(account.Text) && !string.IsNullOrEmpty(password.Text))
            {
                if (NetworkCheck.IsInternet())
                {
                    using (var client = new HttpClient())
                    {
                        var postData = new User
                        {
                            userAccount = account.Text,
                            userPWD = password.Text
                        };
                        // create the request content and define Json  
                        var json = JsonConvert.SerializeObject(postData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        //  send a POST request                  
                        var uri = app.url + "/AR_admin/userlogin";
                        var result = await client.PostAsync(uri, content);

                        if (result.IsSuccessStatusCode)
                        {
                            var resultString = await result.Content.ReadAsStringAsync();
                            var post = JsonConvert.DeserializeObject<LoginResult>(resultString);
                            if (post != null && post.result != null && post.result != "" && post.result == "0")
                            {
                                if (Application.Current.Properties.ContainsKey("account") == false)
                                {
                                    Application.Current.Properties.Add("account", account.Text);                                    
                                }
                                else
                                {
                                    Application.Current.Properties["account"] = account.Text;
                                }
                                await Application.Current.SavePropertiesAsync();
                                var uriPoint = app.url + "/AR_admin/UsergetTotalPoint/" + account.Text;
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
                                            if (Application.Current.Properties.ContainsKey("userTotalPoint") == false)
                                            {
                                                Application.Current.Properties.Add("userTotalPoint", pointData.totalPoint);                                                
                                            }
                                            else
                                            {
                                                Application.Current.Properties["userTotalPoint"] = pointData.totalPoint;                                                
                                            }
                                            await Application.Current.SavePropertiesAsync();
                                        }
                                        if(Application.Current.Properties.ContainsKey("isLoggedIn") == false)
                                        {
                                            Application.Current.Properties.Add("isLoggedIn", true);                                            
                                        }
                                        else
                                        {
                                            Application.Current.Properties["isLoggedIn"] = true;                                            
                                        }
                                        await Application.Current.SavePropertiesAsync();
                                        var postLogData = new UserLog
                                        {
                                            userAccount = account.Text                                            
                                        };
                                        var jsonLog = JsonConvert.SerializeObject(postLogData);
                                        var contentLog = new StringContent(jsonLog, Encoding.UTF8, "application/json");

                                        //  send a POST request                  
                                        var uriLog = app.url + "/AR_admin/UseLoginLog";
                                        var resultLog = await client.PostAsync(uriLog, contentLog);

                                        //if (resultLog.IsSuccessStatusCode)
                                        //{

                                        //}

                                            if (Device.RuntimePlatform == Device.iOS)
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
                                        await DisplayAlert("訊息", "取得使用者點數資料失敗!", "ok");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("訊息", app.errorMessage, "ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("訊息", "登入失敗!", "ok");
                            }
                        }
                    }
                }
                else
                {
                    await DisplayAlert("訊息", app.networkMessage, "ok");
                }
            }
            else
            {
                await DisplayAlert("訊息", "請輸入帳號和密碼!", "ok");
            }
        }
        private void btnRegister(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                Navigation.PushModalAsync(new Register());
            }
            else
            {
                var Register = new NavigationPage(new Register());
                NavigationPage.SetHasNavigationBar(Register, false);
                Navigation.PushAsync(Register);
                Navigation.RemovePage(this);
            }                    
        }       
    }    
}
