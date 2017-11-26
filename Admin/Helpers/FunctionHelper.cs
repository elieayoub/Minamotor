using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Admin.Models;
using System.IO;

namespace Admin.Helpers
{
    public class FunctionHelper
    {
        #region Methods
        public void InsertErrorLog(Exception ex, string action, string controller)
        {
            Entities _ctx = new Entities();
            ErrorLog _errorLog = new ErrorLog();
            _errorLog.ErrorLogID = Guid.NewGuid();
            _errorLog.Message = ex.Message;
            if (ex.InnerException != null)
                _errorLog.InnerException = ex.InnerException.ToString();
            _errorLog.Action = action;
            _errorLog.Controller = controller;
            _errorLog.IsActive = true;
            _errorLog.IsDeleted = false;
            _errorLog.CreatedOn = DateTime.Now;
            _ctx.ErrorLogs.Add(_errorLog);
            _ctx.SaveChanges();
        }

        public User AuthenticateUser(string username, string password)
        {
            Entities _ctx = new Entities();
            User _user = new User();
            _user = (from USR in _ctx.Users
                     where USR.Username.ToLower() == username.ToLower()
                     && USR.Password == password
                     && USR.IsActive == true
                     && USR.IsDeleted == false
                     select USR).FirstOrDefault();
            if (_user != null)
                return _user;
            else
                return null;
        }

        public string GetFromConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public UploadView UploadFile(HttpPostedFileBase file, string uploadToPath, string prefix = null)
        {
            bool _success = false;
            string _uploadedFilePath = "";
            string _errorMessage = "";
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    _uploadedFilePath = Path.Combine(uploadToPath, String.Concat((prefix != null ? prefix : ""), Guid.NewGuid(),
                        fileName.Substring(fileName.IndexOf("."))));
                    file.SaveAs(HttpContext.Current.Server.MapPath(_uploadedFilePath));
                    _success = true;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                InsertErrorLog(ex, "UploadFile", "FunctionHelper");
            }
            return new UploadView { Success = _success, UploadedFilePath = _uploadedFilePath, ErrorMessage = _errorMessage };
        }
        #endregion
    }
}