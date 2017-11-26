using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Helpers;
using Admin.Filters;

namespace Admin.Controllers
{
    [AuthenticationFilter]
    public class AdminMainController : Controller
    {
        #region Members
        private DataHelper _dataHelper;
        private FunctionHelper _functionHelper;

        public DataHelper dataHelper
        {
            get
            {
                if (_dataHelper == null)
                {
                    _dataHelper = new DataHelper();
                    Session["Global_DataHelper"] = _dataHelper;
                }
                return Session["Global_DataHelper"] as DataHelper;
            }
        }

        public FunctionHelper functionHelper
        {
            get
            {
                if (_functionHelper == null)
                {
                    _functionHelper = new FunctionHelper();
                    Session["Global_FunctionHelper"] = _functionHelper;
                }
                return Session["Global_FunctionHelper"] as FunctionHelper;
            }
        }
        #endregion
    }
}