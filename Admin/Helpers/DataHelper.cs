using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Helpers
{
    public class DataHelper
    {
        #region Enums
        public enum CarStatus
        {
            Active = 1,
            Sold = 2,
            Deleted = 3
        }

        public enum CarTransmission
        {
            Automatic = 1,
            Manual = 2
        }

        public enum CarCondition
        {
            New = 1,
            Used = 2
        }
        #endregion
        #region User
        public string UserID
        {
            get
            {
                if (HttpContext.Current.Session["Global_UserID"] != null)
                    return HttpContext.Current.Session["Global_UserID"].ToString();
                else
                    return null;
            }
            set { HttpContext.Current.Session["Global_UserID"] = value; }
        }

        public Boolean User_IsLoggedIn
        {
            get
            {
                if (HttpContext.Current.Session["Global_UserIsLoggedIn"] != null)
                    return Convert.ToBoolean(HttpContext.Current.Session["Global_UserIsLoggedIn"]);
                else
                    return false;
            }
            set { HttpContext.Current.Session["Global_UserIsLoggedIn"] = value; }
        }

        public string UserFullName
        {
            get
            {
                if (HttpContext.Current.Session["Global_UserFullName"] != null)
                    return HttpContext.Current.Session["Global_UserFullName"].ToString();
                else
                    return "";
            }
            set { HttpContext.Current.Session["Global_UserFullName"] = value; }
        }
        #endregion
        #region Currency
        public List<Currency> CurrencyList
        {
            get
            {
                Entities _ctx = new Entities();
                if (HttpContext.Current.Session["Global_CurrencyList"] == null)
                    HttpContext.Current.Session["Global_CurrencyList"] = (from cur in _ctx.Currencies
                                                                          where cur.IsActive == true
                                                                          && cur.IsDeleted == false
                                                                          select cur).ToList();
                return HttpContext.Current.Session["Global_CurrencyList"] as List<Currency>;
            }
            set { HttpContext.Current.Session["Global_CurrencyList"] = value; }
        }
        #endregion
        #region Car
        public string Car_SmallImage
        {
            get
            {
                if (HttpContext.Current.Session["Global_CarSmallImage"] != null)
                    return HttpContext.Current.Session["Global_CarSmallImage"].ToString();
                else
                    return null;
            }
            set { HttpContext.Current.Session["Global_CarSmallImage"] = value; }
        }
        public string Car_LargeImage
        {
            get
            {
                if (HttpContext.Current.Session["Global_CarLargeImage"] != null)
                    return HttpContext.Current.Session["Global_CarLargeImage"].ToString();
                else
                    return null;
            }
            set { HttpContext.Current.Session["Global_CarLargeImage"] = value; }
        }
        #endregion
    }
}