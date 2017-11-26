using Admin.Models;
using Models;
using Models.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class CarManagementController : AdminMainController
    {
        public ActionResult Car()
        {
            Entities _ctx = new Entities();
            List<CarView> _lst = new List<CarView>();
            try
            {
                dataHelper.Car_SmallImage = null;
                dataHelper.Car_LargeImage = null;

                _lst = (from itm in _ctx.Cars
                        join mke in _ctx.Makes on itm.MakeID equals mke.MakeID
                        join mdl in _ctx.Models on itm.ModelID equals mdl.ModelID
                        join cur in _ctx.Currencies on itm.CurrencyID equals cur.CurrencyID
                        select new CarView
                        {
                            car = itm,
                            makeName = mke.Name,
                            modelName = mdl.Name,
                            currencySymbol = cur.Symbol
                        })
                        .AsEnumerable()
                        .Select(obj => new CarView
                        {
                            car = obj.car,
                            makeName = obj.makeName,
                            modelName = obj.modelName,
                            currencySymbol = obj.currencySymbol,
                            statusName = Enum.GetName(typeof(Admin.Helpers.DataHelper.CarStatus), obj.car.Status),
                            transmissionName = Enum.GetName(typeof(Admin.Helpers.DataHelper.CarTransmission), obj.car.Transmission),
                            conditionName = Enum.GetName(typeof(Admin.Helpers.DataHelper.CarCondition), obj.car.Conditon)
                        })
                        .OrderBy(obj => obj.makeName).ToList();

                if (TempData["CarMessage"] != null)
                    ViewBag.Message = TempData["CarMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "Car", "CarManagement");
            }
            return View(_lst);
        }

        public ActionResult CarList(string carID)
        {
            try
            {
                Entities _ctx = new Entities();
                Guid guidCar = Guid.Parse(carID);
                List<List> _lstSelected = (from item in _ctx.CarLists
                                           join lst in _ctx.Lists on item.ListID equals lst.ListID
                                           where item.CarID == guidCar
                                           select lst).ToList();
                ViewBag.SelectedList = _lstSelected;

                List<List> _lstAvailable = (from item in _ctx.Lists
                                            where !(from cls in _ctx.CarLists
                                                    select cls.ListID)
                                                    .Contains(item.ListID)
                                            select item).ToList();
                ViewBag.AvailableList = _lstAvailable;
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "AddCarToList", "CarManagement");
            }
            return View();
        }

        public ActionResult CarImages(string carID)
        {
            try
            {
                Entities _ctx = new Entities();
                Guid guidCar = Guid.Parse(carID);
                List<CarImage> _lstAvailableImages = (from item in _ctx.CarImages
                                                   where item.CarID == guidCar
                                                  select item).ToList();
                ViewBag.AvailableImages = _lstAvailableImages;
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "CarImages", "CarManagement");
            }
            return View();
        }

        public ActionResult AddCarToList(FormCollection form)
        {
            try
            {
                string carID = form["CarID"];
                string ListIds = form["hfSelectedIds"];
                string[] arrListIds = ListIds.Split(',');
                Guid guidCar = Guid.Parse(carID);
                Entities _ctx = new Entities();

                List<CarList> _lstSelected = (from item in _ctx.CarLists
                                              where item.CarID == guidCar
                                              select item).ToList();
                foreach (var selected in _lstSelected)
                {
                    _ctx.CarLists.Remove(selected);
                }
                if (_lstSelected.Count() > 0)
                    _ctx.SaveChanges();

                foreach (var item in arrListIds)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        CarList _item = new CarList() { CarID = Guid.Parse(carID), ListID = Guid.Parse(item.ToString()), CarListID = Guid.NewGuid() };
                        _ctx.CarLists.Add(_item);
                    }
                }

                if (ListIds.Count() > 0)
                    _ctx.SaveChanges();
                TempData["CarMessage"] = "Car list updated";
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "AddCarToList", "CarManagement");
            }
            return RedirectToAction("Car", "CarManagement");
        }

        public String UploadCarImages(FormCollection form)
        {
            string uploadedImages = "";
            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    string tempPath = functionHelper.GetFromConfig("TempFolder");
                    if (file != null && file.ContentLength > 0)
                    {
                        UploadView _uploadView = functionHelper.UploadFile(file, tempPath);
                        if (_uploadView.Success)
                            uploadedImages += String.Concat(_uploadView.UploadedFilePath, "|");
                    }
                }
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UploadCarImages", "CarManagement");
            }
            return uploadedImages;
        }

        public ActionResult AddCarImages(FormCollection form)
        {
            try
            {
                string carID = form["CarID"];
                string ListImages = form["CarImages"];
                string[] arrListImages = ListImages.Split('|');
                Guid guidCar = Guid.Parse(carID);
                Entities _ctx = new Entities();

                List<CarImage> _lstSelected = (from item in _ctx.CarImages
                                              where item.CarID == guidCar
                                              select item).ToList();
                foreach (var selected in _lstSelected)
                {
                    _ctx.CarImages.Remove(selected);
                }
                if (_lstSelected.Count() > 0)
                    _ctx.SaveChanges();

                string _carImagePath = String.Concat(new DirectoryInfo(Server.MapPath("~")).Parent.FullName, "/", String.Format(functionHelper.GetFromConfig("CarImageFolder"), carID));
                if (!Directory.Exists(_carImagePath))
                    Directory.CreateDirectory(_carImagePath);
                string _carGalleryPath = String.Concat(_carImagePath, functionHelper.GetFromConfig("CarGalleryFolder"));
                if (!Directory.Exists(_carGalleryPath))
                    Directory.CreateDirectory(_carGalleryPath);
                string _imagePath = "";
                //Delete all images inside gallery
                if (ListImages.Count() > 0)
                {
                    foreach (var imgFile in Directory.GetFiles(_carGalleryPath))
                    {
                        if (ListImages.IndexOf(imgFile.Substring(imgFile.LastIndexOf("/") + 1)) == -1)
                            System.IO.File.Delete(imgFile);
                    }
                }
                foreach (var item in arrListImages)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        _imagePath = String.Concat(_carGalleryPath, item.Substring(item.LastIndexOf("/") + 1));
                        if (item.IndexOf("Temp") != -1)
                            System.IO.File.Copy(Server.MapPath(item.Replace("..", "~")), _imagePath, true);
                        //else
                        //    System.IO.File.Copy(String.Concat(new DirectoryInfo(Server.MapPath("~/")).Parent.FullName, "\\", item.Replace("../", "")), _imagePath, true);
                        CarImage _item = new CarImage() { 
                            CarID = Guid.Parse(carID), 
                            Image = String.Concat(String.Format(functionHelper.GetFromConfig("CarImageFolder"), guidCar),
                                    functionHelper.GetFromConfig("CarGalleryFolder"),
                                    item.Substring(item.LastIndexOf("/") + 1)), 
                            CarImagesID = Guid.NewGuid()};
                        _ctx.CarImages.Add(_item);
                    }
                }

                if (ListImages.Count() > 0)
                    _ctx.SaveChanges();
                TempData["CarMessage"] = "Car images updated";
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "AddCarImages", "CarManagement");
            }
            return RedirectToAction("Car", "CarManagement");
        }

        public ActionResult NewCar()
        {
            Car _itm = new Car();
            _itm.IsActive = true;
            _itm.IsDeleted = false;

            try
            {
                Entities _ctx = new Entities();
                List<Make> _lstMake = new List<Make>();
                _lstMake = (from mke in _ctx.Makes
                            where mke.IsActive == true
                            && mke.IsDeleted == false
                            select mke).OrderBy(obj => obj.Name).ToList();
                if (_lstMake != null)
                    ViewBag.MakeList = new SelectList(_lstMake.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.MakeID.ToString() }).AsEnumerable(), "Value", "Text");

                List<Model> _lstModel = new List<Model>();
                _lstModel = (from mdl in _ctx.Models
                             where mdl.IsActive == true
                             && mdl.IsDeleted == false
                             select mdl).OrderBy(obj => obj.Name).ToList();
                if (_lstModel != null)
                    ViewBag.ModelList = new SelectList(_lstModel.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.ModelID.ToString() }).AsEnumerable(), "Value", "Text");

                if (dataHelper.CurrencyList != null)
                    ViewBag.CurrencyList = new SelectList(dataHelper.CurrencyList.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.CurrencyID.ToString() }).AsEnumerable(), "Value", "Text");

                ViewBag.TransmissionList = new SelectList(Enum.GetValues(typeof(Admin.Helpers.DataHelper.CarTransmission))
                    .Cast<Admin.Helpers.DataHelper.CarTransmission>()
                    .Select(obj => new SelectListItem { Text = obj.ToString(), Value = ((int)obj).ToString() })
                    .AsEnumerable(), "Value", "Text");

                ViewBag.ConditionList = new SelectList(Enum.GetValues(typeof(Admin.Helpers.DataHelper.CarCondition))
                    .Cast<Admin.Helpers.DataHelper.CarCondition>()
                    .Select(obj => new SelectListItem { Text = obj.ToString(), Value = ((int)obj).ToString() })
                    .AsEnumerable(), "Value", "Text");

                ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(Admin.Helpers.DataHelper.CarStatus))
                    .Cast<Admin.Helpers.DataHelper.CarStatus>()
                    .Select(obj => new SelectListItem { Text = obj.ToString(), Value = ((int)obj).ToString() })
                    .AsEnumerable(), "Value", "Text");

                if (TempData["CarMessage"] != null)
                    ViewBag.Message = TempData["CarMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "NewCar", "CarManagement");
            }

            return View(_itm);
        }

        public ActionResult InsertCar(Car item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.CarID = Guid.NewGuid();
                    item.CreatedOn = DateTime.Now;
                    item.IsActive = true;
                    item.IsDeleted = false;

                    string _smallImagePath = "", _largeImagePath = "";
                    string _carImagePath = String.Concat(new DirectoryInfo(Server.MapPath("~")).Parent.FullName, "/", String.Format(functionHelper.GetFromConfig("CarImageFolder"), item.CarID));
                    if (!Directory.Exists(_carImagePath))
                        Directory.CreateDirectory(_carImagePath);
                    if (dataHelper.Car_SmallImage != null)
                    {
                        //Copy small image from temp folder
                        _smallImagePath = String.Concat(_carImagePath, "/",
                            dataHelper.Car_SmallImage.Substring(dataHelper.Car_SmallImage.LastIndexOf("/")));
                        System.IO.File.Copy(Server.MapPath(dataHelper.Car_SmallImage), _smallImagePath, true);
                    }
                    if (dataHelper.Car_LargeImage != null)
                    {
                        //Copy large image from temp folder
                        _largeImagePath = String.Concat(_carImagePath, "/",
                            dataHelper.Car_LargeImage.Substring(dataHelper.Car_LargeImage.LastIndexOf("/")));
                        System.IO.File.Copy(Server.MapPath(dataHelper.Car_LargeImage), _largeImagePath, true);
                    }

                    item.LargeImage = String.Concat(String.Format(functionHelper.GetFromConfig("CarImageFolder"), item.CarID),
                        dataHelper.Car_SmallImage.Substring(dataHelper.Car_SmallImage.LastIndexOf("/") + 1));
                    item.SmallImage = String.Concat(String.Format(functionHelper.GetFromConfig("CarImageFolder"), item.CarID),
                        dataHelper.Car_LargeImage.Substring(dataHelper.Car_LargeImage.LastIndexOf("/") + 1));
                    _ctx.Cars.Add(item);
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("NewCar", "CarManagement");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "InsertCar", "CarManagement");
            }
            TempData["CarMessage"] = "Car inserted successfully";
            return RedirectToAction("Car", "CarManagement");
        }

        public ActionResult EditCar(string id)
        {
            Entities _ctx = new Entities();
            Car _itm = null;
            try
            {
                Guid guidItem = new Guid(id);
                _itm = (from car in _ctx.Cars
                        where car.CarID == guidItem
                        select car).FirstOrDefault();

                List<Make> _lstMake = new List<Make>();
                _lstMake = (from mke in _ctx.Makes
                            where mke.IsActive == true
                            && mke.IsDeleted == false
                            select mke).OrderBy(obj => obj.Name).ToList();
                if (_lstMake != null)
                    ViewBag.MakeList = new SelectList(_lstMake.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.MakeID.ToString() }).AsEnumerable(), "Value", "Text");

                List<Model> _lstModel = new List<Model>();
                _lstModel = (from mdl in _ctx.Models
                             where mdl.IsActive == true
                             && mdl.IsDeleted == false
                             select mdl).OrderBy(obj => obj.Name).ToList();
                if (_lstModel != null)
                    ViewBag.ModelList = new SelectList(_lstModel.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.ModelID.ToString() }).AsEnumerable(), "Value", "Text");

                if (dataHelper.CurrencyList != null)
                    ViewBag.CurrencyList = new SelectList(dataHelper.CurrencyList.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.CurrencyID.ToString() }).AsEnumerable(), "Value", "Text");

                ViewBag.TransmissionList = new SelectList(Enum.GetValues(typeof(Admin.Helpers.DataHelper.CarTransmission))
                    .Cast<Admin.Helpers.DataHelper.CarTransmission>()
                    .Select(obj => new SelectListItem { Text = obj.ToString(), Value = ((int)obj).ToString() })
                    .AsEnumerable(), "Value", "Text");

                ViewBag.ConditionList = new SelectList(Enum.GetValues(typeof(Admin.Helpers.DataHelper.CarCondition))
                    .Cast<Admin.Helpers.DataHelper.CarCondition>()
                    .Select(obj => new SelectListItem { Text = obj.ToString(), Value = ((int)obj).ToString() })
                    .AsEnumerable(), "Value", "Text");

                ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(Admin.Helpers.DataHelper.CarStatus))
                    .Cast<Admin.Helpers.DataHelper.CarStatus>()
                    .Select(obj => new SelectListItem { Text = obj.ToString(), Value = ((int)obj).ToString() })
                    .AsEnumerable(), "Value", "Text");

                if (TempData["CarMessage"] != null)
                    ViewBag.Message = TempData["CarMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "EditCar", "CarManagement");
            }

            return View(_itm);
        }

        public ActionResult UpdateCar(Car item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.CreatedOn = DateTime.Now;

                    string _smallImagePath = "", _largeImagePath = "";
                    string _carImagePath = String.Concat(new DirectoryInfo(Server.MapPath("~")).Parent.FullName, "/", String.Format(functionHelper.GetFromConfig("CarImageFolder"), item.CarID));
                    if (!Directory.Exists(_carImagePath))
                        Directory.CreateDirectory(_carImagePath);
                    if (dataHelper.Car_SmallImage != null)
                    {
                        //Delete old image
                        foreach (var fileInfo in Directory.GetFiles(_carImagePath, String.Concat(functionHelper.GetFromConfig("CarSmallImagePrefix"), "*")))
                        {
                            if (System.IO.File.Exists(fileInfo))
                                System.IO.File.Delete(fileInfo);
                        }

                        //Copy small image from temp folder
                        _smallImagePath = String.Concat(_carImagePath, "/",
                            dataHelper.Car_SmallImage.Substring(dataHelper.Car_SmallImage.LastIndexOf("/")));
                        System.IO.File.Copy(Server.MapPath(dataHelper.Car_SmallImage), _smallImagePath, true);

                        item.SmallImage = String.Concat(String.Format(functionHelper.GetFromConfig("CarImageFolder"), item.CarID),
                        dataHelper.Car_SmallImage.Substring(dataHelper.Car_SmallImage.LastIndexOf("/") + 1));
                    }
                    if (dataHelper.Car_LargeImage != null)
                    {
                        //Delete old image
                        foreach (var fileInfo in Directory.GetFiles(_carImagePath, String.Concat(functionHelper.GetFromConfig("CarLargeImagePrefix"), "*")))
                        {
                            if (System.IO.File.Exists(fileInfo))
                                System.IO.File.Delete(fileInfo);
                        }

                        //Copy large image from temp folder
                        _largeImagePath = String.Concat(_carImagePath, "/",
                            dataHelper.Car_LargeImage.Substring(dataHelper.Car_LargeImage.LastIndexOf("/")));
                        System.IO.File.Copy(Server.MapPath(dataHelper.Car_LargeImage), _largeImagePath, true);

                        item.LargeImage = String.Concat(String.Format(functionHelper.GetFromConfig("CarImageFolder"), item.CarID),
                        dataHelper.Car_LargeImage.Substring(dataHelper.Car_LargeImage.LastIndexOf("/") + 1));
                    }

                    _ctx.Cars.Attach(item);
                    _ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("UpdateCar", "CarManagement");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UpdateCar", "CarManagement");
            }
            TempData["CarMessage"] = "Car updated successfully";
            return RedirectToAction("Car", "CarManagement");
        }

        public ActionResult UploadSmallImage(FormCollection form)
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string tempPath = functionHelper.GetFromConfig("TempFolder");
                if (file != null && file.ContentLength > 0)
                {
                    UploadView _uploadView = functionHelper.UploadFile(file, tempPath, functionHelper.GetFromConfig("CarSmallImagePrefix"));
                    if (_uploadView.Success)
                    {
                        dataHelper.Car_SmallImage = _uploadView.UploadedFilePath;
                        TempData["CarMessage"] = "Small Image Uploaded Successfully";
                    }
                    else
                    {
                        dataHelper.Car_SmallImage = null;
                        TempData["CarMessage"] = String.Concat("Error occured: ", _uploadView.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UploadSmallImage", "CarManagement");
                TempData["CarMessage"] = String.Concat("Error occured: ", ex.Message);
            }
            return RedirectToAction("NewCar", "CarManagement");
        }

        public ActionResult UploadLargeImage(FormCollection form)
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string tempPath = functionHelper.GetFromConfig("TempFolder");
                if (file != null && file.ContentLength > 0)
                {
                    UploadView _uploadView = functionHelper.UploadFile(file, tempPath, functionHelper.GetFromConfig("CarLargeImagePrefix"));
                    if (_uploadView.Success)
                    {
                        dataHelper.Car_LargeImage = _uploadView.UploadedFilePath;
                        TempData["CarMessage"] = "Large Image Uploaded Successfully";
                    }
                    else
                    {
                        dataHelper.Car_LargeImage = null;
                        TempData["CarMessage"] = String.Concat("Error occured: ", _uploadView.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UploadLargeImage", "CarManagement");
                TempData["CarMessage"] = String.Concat("Error occured: ", ex.Message);
            }
            return RedirectToAction("NewCar", "CarManagement");
        }


    }
}