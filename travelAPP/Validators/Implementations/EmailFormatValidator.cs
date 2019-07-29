using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using travelAPP.Validators.Contracts;

namespace travelAPP.Validators.Implementations
{
    public class EmailFormatValidator : IValidator
    {
        public string Message { get; set; }
        //public string Format { get; set; }

        public bool CheckAsync(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (ValidateEmail(value))
                {
                    return true;
                }
                else
                {
                    Message = "email格式有誤";
                    return false;
                }

            }
            else
            {
                Message = "請輸入email";
                return false;
            }
        }

        private bool ValidateEmail(string email)
        {
            if (email != null && email != "")
            {
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                return isEmail;
            }
            else
            {
                return false;
            }
        }
    }
}
