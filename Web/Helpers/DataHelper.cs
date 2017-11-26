using Models;
using Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Helpers
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

        #region Make
        public List<Make> MakeList
        {
            get
            {
                Entities _ctx = new Entities();
                if (HttpContext.Current.Session["Global_MakeList"] == null)
                    HttpContext.Current.Session["Global_MakeList"] = (from mke in _ctx.Makes
                                                                          where mke.IsActive == true
                                                                          && mke.IsDeleted == false
                                                                          select mke).ToList();
                return HttpContext.Current.Session["Global_MakeList"] as List<Make>;
            }
            set { HttpContext.Current.Session["Global_MakeList"] = value; }
        }
        #endregion

        #region Model
        public List<Model> ModelList
        {
            get
            {
                Entities _ctx = new Entities();
                if (HttpContext.Current.Session["Global_ModelList"] == null)
                    HttpContext.Current.Session["Global_ModelList"] = (from mdl in _ctx.Models
                                                                      where mdl.IsActive == true
                                                                      && mdl.IsDeleted == false
                                                                      select mdl).ToList();
                return HttpContext.Current.Session["Global_ModelList"] as List<Model>;
            }
            set { HttpContext.Current.Session["Global_ModelList"] = value; }
        }
        #endregion

        #region VehicleTrim
        public List<VehicleTrim> VehicleTrimList
        {
            get
            {
                Entities _ctx = new Entities();
                if (HttpContext.Current.Session["Global_VehicleTrimList"] == null)
                    HttpContext.Current.Session["Global_VehicleTrimList"] = (from vtm in _ctx.VehicleTrims
                                                                       where vtm.IsActive == true
                                                                       && vtm.IsDeleted == false
                                                                       select vtm).ToList();
                return HttpContext.Current.Session["Global_VehicleTrimList"] as List<VehicleTrim>;
            }
            set { HttpContext.Current.Session["Global_VehicleTrimList"] = value; }
        }
        #endregion

        #region Car
        public List<CarView> CarViewList
        {
            get
            {
                Entities _ctx = new Entities();
                if (HttpContext.Current.Session["Global_CarViewList"] == null)
                    HttpContext.Current.Session["Global_CarViewList"] = (from itm in _ctx.Cars
                                                                         join mke in _ctx.Makes on itm.MakeID equals mke.MakeID
                                                                         join mdl in _ctx.Models on itm.ModelID equals mdl.ModelID
                                                                         join cur in _ctx.Currencies on itm.CurrencyID equals cur.CurrencyID
                                                                         join vtm in _ctx.VehicleTrims on itm.VehicleTrimID equals vtm.VehicleTrimID
                                                                         where itm.Status == (int)CarStatus.Active 
                                                                         select new CarView
                                                                         {
                                                                             car = itm,
                                                                             makeName = mke.Name,
                                                                             modelName = mdl.Name,
                                                                             currencySymbol = cur.Symbol,
                                                                             vehicleTrimName = vtm.Name
                                                                         })
                                                                        .AsEnumerable()
                                                                        .Select(obj => new CarView
                                                                        {
                                                                            car = obj.car,
                                                                            makeName = obj.makeName,
                                                                            modelName = obj.modelName,
                                                                            vehicleTrimName = obj.vehicleTrimName,
                                                                            currencySymbol = obj.currencySymbol,
                                                                            statusName = Enum.GetName(typeof(CarStatus), obj.car.Status),
                                                                            transmissionName = Enum.GetName(typeof(CarTransmission), obj.car.Transmission),
                                                                            conditionName = Enum.GetName(typeof(CarCondition), obj.car.Conditon)
                                                                        })
                                                                        .OrderBy(obj => obj.makeName).ToList();
                return HttpContext.Current.Session["Global_CarViewList"] as List<CarView>;
            }
            set { HttpContext.Current.Session["Global_CarViewList"] = value; }
        }

        public List<WebCarListView> WebCarListViewList
        {
            get
            {
                Entities _ctx = new Entities();
                if (HttpContext.Current.Session["Global_WebCarListView"] == null)
                { 
                    List<WebCarListView> _lst = (from itm in _ctx.Cars
                                                join mke in _ctx.Makes on itm.MakeID equals mke.MakeID
                                                join mdl in _ctx.Models on itm.ModelID equals mdl.ModelID
                                                join cur in _ctx.Currencies on itm.CurrencyID equals cur.CurrencyID
                                                join clst in _ctx.CarLists on itm.CarID equals clst.CarID
                                                join lst in _ctx.Lists on clst.ListID equals lst.ListID
                                                join vtm in _ctx.VehicleTrims on itm.VehicleTrimID equals vtm.VehicleTrimID
                                                where itm.Status == (int)CarStatus.Active
                                                || itm.Status == (int)CarStatus.Sold
                                                select new WebCarListView
                                                {
                                                    carID = itm.CarID,
                                                    smallImage = itm.SmallImage,
                                                    largeImage = itm.LargeImage,
                                                    makeName = mke.Name,
                                                    modelName = mdl.Name,
                                                    currencySymbol = cur.Symbol,
                                                    listName = lst.Name,
                                                    year = itm.Year,
                                                    vehicleTrimName = vtm.Name,
                                                    listID = lst.ListID
                                                })
                                            .AsEnumerable()
                                            .Select(obj => new WebCarListView
                                            {
                                                carID = obj.carID,
                                                listName = obj.listName,
                                                smallImage = obj.smallImage,
                                                largeImage = obj.largeImage,
                                                makeName = obj.makeName,
                                                modelName = obj.modelName,
                                                vehicleTrimName = obj.vehicleTrimName,
                                                year = obj.year,
                                                listID = obj.listID,
                                                currencySymbol = obj.currencySymbol,
                                                statusName = Enum.GetName(typeof(CarStatus), obj.status),
                                                transmissionName = Enum.GetName(typeof(CarTransmission), obj.transmission),
                                                conditionName = Enum.GetName(typeof(CarCondition), obj.condition)
                                            })
                                            .OrderBy(obj => obj.makeName).ToList();

                    foreach (WebCarListView item in _lst)
                    {
                        item.carImage = (from cimg in _ctx.CarImages
                                         where cimg.CarID == item.carID
                                         select cimg).ToList();
                    }
                    HttpContext.Current.Session["Global_WebCarListView"] = _lst;
                }
                return HttpContext.Current.Session["Global_WebCarListView"] as List<WebCarListView>;
            }
            set { HttpContext.Current.Session["Global_WebCarListView"] = value; }
        }
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