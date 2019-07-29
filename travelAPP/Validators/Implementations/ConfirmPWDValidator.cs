using System;
using System.Collections.Generic;
using System.Text;
using travelAPP.Behaviors;
using travelAPP.Validators.Contracts;

namespace travelAPP.Validators.Implementations
{
    class ConfirmPWDValidator : IValidator
    {
        ConfirmPasswordBehavior model = new ConfirmPasswordBehavior();
        public string Message { get; set; }
        public bool CheckAsync(string value)
        {
            var oldText = model.oldpassword;
            if (!string.IsNullOrEmpty(value))
            {
                //if (model.IsValid)
                //{
                //    return true;
                //}
                //else
                //{
                //    Message = "密碼不一致";
                //    return false;
                //}
                return true;
            }
            else
            {
                Message = "請輸入密碼";
                return false;
            }
        }
    }
}
