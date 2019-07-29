using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using travelAPP.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace travelAPP
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Register : ContentPage
	{
        AppValue app = new AppValue();
        public Register()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            UserRegister model = new UserRegister();
            this.BindingContext = model;                 
        }

        private async void btnOK(object osender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(accountText.Text) && !string.IsNullOrEmpty(password.Text) && !string.IsNullOrEmpty(confirmPassword.Text)
                && !string.IsNullOrEmpty(nameText.Text) && !string.IsNullOrEmpty(emailText.Text))
            {
                if (password.Text.Equals(confirmPassword.Text))
                {
                    if (NetworkCheck.IsInternet())
                    {
                        using (var client = new HttpClient())
                        {
                            //ServicePointManager.ServerCertificateValidationCallback +=
                            //(sender, cert, chain, sslPolicyErrors) => true;
                            var postData = new UserRegister
                            {
                                userAccount = this.accountText.Text,
                                userPWD = this.password.Text,
                                userName = this.nameText.Text,
                                email = this.emailText.Text,
                                tel = this.mobileText.Text
                            };
                            // create the request content and define Json  
                            var json = JsonConvert.SerializeObject(postData);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            //  send a POST request                  
                            var uri = app.url + "/AR_admin/addUser";
                            var result = await client.PostAsync(uri, content);
                            if (result.IsSuccessStatusCode)
                            {
                                var resultString = await result.Content.ReadAsStringAsync();
                                var post = JsonConvert.DeserializeObject<LoginResult>(resultString);
                                if (post != null && post.result != null && post.result != "" && post.result == "0")
                                {                                    
                                    if (Application.Current.Properties.ContainsKey("account") == false)
                                    {
                                        Application.Current.Properties.Add("account", accountText.Text);
                                        await Application.Current.SavePropertiesAsync();
                                    }
                                    if (Application.Current.Properties.ContainsKey("userTotalPoint") == false)
                                    {
                                        Application.Current.Properties.Add("userTotalPoint", "0");
                                        await Application.Current.SavePropertiesAsync();
                                    }
                                    await DisplayAlert("訊息", "註冊成功!", "OK");
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
                                    await DisplayAlert("訊息", "註冊失敗!請稍候再試", "OK");
                                }
                            }
                            else
                            {
                                DependencyService.Get<Toast>().Show(app.errorMessage);
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("訊息", app.networkMessage, "OK");
                    }                    
                }
                else
                {
                    await DisplayAlert("訊息", "密碼不一致請重新輸入", "OK");
                }
            }
            else
            {
                await DisplayAlert("訊息", "有必填欄位未輸入請重新輸入!", "OK");
            }                        
        }

        private void btnCancel(object sender, EventArgs e)
        {
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
    }
}