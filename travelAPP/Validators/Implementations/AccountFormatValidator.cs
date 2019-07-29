using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using travelAPP.Model;
using travelAPP.Validators.Contracts;
using Xamarin.Forms;

namespace travelAPP.Validators.Implementations
{
   public class AccountFormatValidator : IValidator
    {
        public string Message { get; set; }
        AppValue app = new AppValue();
        //public string Format { get; set; }

        public bool CheckAsync(string value)
        {
            bool checkUser = ValidateUserAccount(value);
            if (!string.IsNullOrEmpty(value) && value.Length > 0)
            {
                if (IsNumericOrLetter(value))
                {
                    if (!checkUser)
                    {
                        return true;
                    }
                    else
                    {
                        Message = "這個帳號已經有人使用;請試試其他名稱";
                        return false;
                    }
                }
                else
                {
                    Message = "請輸入英數字組合的帳號";
                    return false;
                }
            }
            else
            {
                Message = "請輸入使用者帳號";
                return false;
            }
        }

        public static bool IsNumericOrLetter(string input)
        {
            return Regex.IsMatch(input, "^[A-Za-z0-9]+$");
        }

        private bool ValidateUserAccount(String userAccount)
        {
            bool isDuplicate = false;
            try
            {
                if (!string.IsNullOrEmpty(userAccount))
                {
                    //ServicePointManager.ServerCertificateValidationCallback +=
                    //        (sender, cert, chain, sslPolicyErrors) => true;
                    if (NetworkCheck.IsInternet())
                    {
                        var webClient = new WebClient();
                        Uri uri = new Uri(app.url + "/AR_admin/checkUser/" + userAccount);
                        var result = webClient.DownloadString(uri);
                        var user = JsonConvert.DeserializeObject<LoginResult>(result);
                        if (user.result == "1")
                        {
                            isDuplicate = true;
                        }
                        else
                        {
                            isDuplicate = false;
                        }
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert("訊息", app.networkMessage, "ok");
                        });
                        //showMessage show = new showMessage();
                        //show.DisplayAlert("訊息",app.networkMessage,"ok");
                    }                    
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(app.errorMessage);
                //Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
            }
            return isDuplicate;
        }
    }
}
