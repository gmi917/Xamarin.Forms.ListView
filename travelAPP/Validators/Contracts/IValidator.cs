using System;
using System.Collections.Generic;
using System.Text;

namespace travelAPP.Validators.Contracts
{
    public interface IValidator
    {
        string Message { get; set; }
        bool CheckAsync(string value);
    }
}
