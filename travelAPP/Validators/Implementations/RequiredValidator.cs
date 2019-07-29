using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using travelAPP.Validators.Contracts;

namespace travelAPP.Validators.Implementations
{
    public class RequiredValidator : IValidator
    {
        public string Message { get; set; }

        public bool CheckAsync(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Message = "必填欄位";
                return false;
            }
            else
            {
                return true;
            }
            //return !string.IsNullOrWhiteSpace(value);
        }

        public Task<bool> DoCheckAsync(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
