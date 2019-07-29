using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using travelAPP.Validators.Contracts;

namespace travelAPP.Validators.Implementations
{
    public class FormatValidator : IValidator
    {
        public string Message { get; set; } = "格式錯誤";
        public string Format { get; set; }

        public bool CheckAsync(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Regex format = new Regex(Format);

                return format.IsMatch(value);
            }
            else
            {
                return false;
            }
        }

        public Task<bool> DoCheckAsync(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
