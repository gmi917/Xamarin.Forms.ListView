﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace travelAPP
{
    class AppValue
    {        
        public string url = "http://211.21.173.183:8387";        
        public int PageSize = 6;//每頁顯示的數量
        public string networkMessage = "請先開啟網路";
        public string errorMessage = "系統有誤請稍後再試!";
    }
}
