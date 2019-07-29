using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace travelAPP
{
    class CheckImage
    {
        public bool GetImageSourceOrDefault(string orgUrl)
        {
            var req = (HttpWebRequest)WebRequest.Create(orgUrl);
            req.Method = "HEAD";
            try
            {
                using (var resp = req.GetResponse())
                {
                    bool res = resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                        .StartsWith("image/");
                    if (res)
                        return true;
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }

        }
    }
}
