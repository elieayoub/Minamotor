using Admin.Filters;
using Models;
using Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class DefinitionsController : AdminMainController
    {
        public ActionResult Make()
        {
            Entities _ctx = new Entities();
            List<Make> _lst = new List<Make>();
            try
            {
                _lst = (from mke in _ctx.Makes
                        select mke).OrderBy(obj => obj.Name).ToList();

                if (TempData["InsertMakeMessage"] != null)
                    ViewBag.Message = TempData["InsertMakeMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "Make", "Definitions");
            }
            return View(_lst);
        }

        public ActionResult NewMake()
        {
            Make _itm = new Make();
            _itm.IsActive = true;
            _itm.IsDeleted = false;
            return View(_itm);
        }

        public ActionResult InsertMake(Make item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.MakeID = Guid.NewGuid();
                    item.CreatedOn = DateTime.Now;
                    _ctx.Makes.Add(item);
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("NewMake", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "InserMake", "Definitions");
            }
            TempData["InsertMakeMessage"] = "Make inserted successfully";
            return RedirectToAction("Make", "Definitions");
        }

        public ActionResult EditMake(string id)
        {
            Entities _ctx = new Entities();
            Make _itm = null;
            try
            {
                Guid guidItem = new Guid(id);
                _itm = (from mke in _ctx.Makes
                        where mke.MakeID == guidItem
                        select mke).FirstOrDefault();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "EditMake", "Definitions");
            }
            return View(_itm);
        }

        public ActionResult UpdateMake(Make item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.CreatedOn = DateTime.Now;
                    _ctx.Makes.Attach(item);
                    _ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("EditMake", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UpdateMake", "Definitions");
            }
            TempData["InsertMakeMessage"] = "Make updated successfully";
            return RedirectToAction("Make", "Definitions");
        }

        public ActionResult Model()
        {
            Entities _ctx = new Entities();
            List<ModelView> _lst = new List<ModelView>();
            try
            {
                _lst = (from mdl in _ctx.Models
                        join mke in _ctx.Makes on mdl.MakeID equals mke.MakeID
                        select new ModelView
                        {
                            model = mdl,
                            makeName = mke.Name
                        }).OrderBy(obj => obj.model.Name).ToList();

                if (TempData["InsertModelMessage"] != null)
                    ViewBag.Message = TempData["InsertModelMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "Model", "Definitions");
            }
            return View(_lst);
        }

        public ActionResult NewModel()
        {
            Model _itm = new Model();
            _itm.IsActive = true;
            _itm.IsDeleted = false;

            try
            {
                Entities _ctx = new Entities();
                List<Make> _lst = new List<Make>();
                _lst = (from mke in _ctx.Makes
                        where mke.IsActive == true
                        && mke.IsDeleted == false
                        select mke).OrderBy(obj => obj.Name).ToList();
                if (_lst != null)
                    ViewBag.MakeList = new SelectList(_lst.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.MakeID.ToString()}).AsEnumerable(), "Value", "Text");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "NewModel", "Definitions");
            }

            return View(_itm);
        }

        public ActionResult InsertModel(Model item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.ModelID = Guid.NewGuid();
                    item.CreatedOn = DateTime.Now;
                    _ctx.Models.Add(item);
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("NewModel", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "InsertModel", "Definitions");
            }
            TempData["InsertModelMessage"] = "Model inserted successfully";
            return RedirectToAction("Model", "Definitions");
        }

        public ActionResult EditModel(string id)
        {
            Entities _ctx = new Entities();
            Model _itm = null;
            try
            {
                Guid guidItem = new Guid(id);
                _itm = (from mdl in _ctx.Models
                        where mdl.ModelID == guidItem
                        select mdl).FirstOrDefault();

                List<Make> _lst = new List<Make>();
                _lst = (from mke in _ctx.Makes
                        where mke.IsActive == true
                        && mke.IsDeleted == false
                        select mke).OrderBy(obj => obj.Name).ToList();
                if (_lst != null)
                    ViewBag.MakeList = new SelectList(_lst.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.MakeID.ToString() }).AsEnumerable(), "Value", "Text");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "EditModel", "Definitions");
            }
            return View(_itm);
        }

        public ActionResult UpdateModel(Model item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.CreatedOn = DateTime.Now;
                    _ctx.Models.Attach(item);
                    _ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("EditModel", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UpdateModel", "Definitions");
            }
            TempData["InsertModelMessage"] = "Model updated successfully";
            return RedirectToAction("Model", "Definitions");
        }

        public ActionResult UserManagement()
        {
            Entities _ctx = new Entities();
            List<User> _lst = new List<User>();
            try
            {
                _lst = (from usr in _ctx.Users
                        select usr).OrderBy(obj => obj.Username).ToList();

                if (TempData["UserMessage"] != null)
                    ViewBag.Message = TempData["UserMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UserManagement", "Definitions");
            }
            return View(_lst);
        }

        public ActionResult NewUser()
        {
            User _itm = new User();
            _itm.IsActive = true;
            _itm.IsDeleted = false;
            return View(_itm);
        }

        public ActionResult InsertUser(User item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.UserID = Guid.NewGuid();
                    item.CreatedOn = DateTime.Now;
                    _ctx.Users.Add(item);
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("NewUser", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "InserUser", "Definitions");
            }
            TempData["UserMessage"] = "User inserted successfully";
            return RedirectToAction("UserManagement", "Definitions");
        }

        public ActionResult EditUser(string id)
        {
            Entities _ctx = new Entities();
            User _itm = null;
            try
            {
                Guid guidItem = new Guid(id);
                _itm = (from usr in _ctx.Users
                        where usr.UserID == guidItem
                        select usr).FirstOrDefault();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "EditUser", "Definitions");
            }
            return View(_itm);
        }

        public ActionResult UpdateUser(User item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.CreatedOn = DateTime.Now;
                    _ctx.Users.Attach(item);
                    _ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("EditUser", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UpdateUser", "Definitions");
            }
            TempData["UserMessage"] = "User updated successfully";
            return RedirectToAction("UserManagement", "Definitions");
        }

        public ActionResult List()
        {
            Entities _ctx = new Entities();
            List<List> _lst = new List<List>();
            try
            {
                _lst = (from lst in _ctx.Lists
                        select lst).OrderBy(obj => obj.Name).ToList();

                if (TempData["ListMessage"] != null)
                    ViewBag.Message = TempData["ListMessage"].ToString();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "List", "Definitions");
            }
            return View(_lst);
        }

        public ActionResult NewList()
        {
            List _itm = new List();
            _itm.IsActive = true;
            _itm.IsDeleted = false;
            return View(_itm);
        }

        public ActionResult InsertList(List item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.ListID = Guid.NewGuid();
                    item.CreatedOn = DateTime.Now;
                    _ctx.Lists.Add(item);
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("NewList", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "InserList", "Definitions");
            }
            TempData["ListMessage"] = "List inserted successfully";
            return RedirectToAction("List", "Definitions");
        }

        public ActionResult EditList(string id)
        {
            Entities _ctx = new Entities();
            List _itm = null;
            try
            {
                Guid guidItem = new Guid(id);
                _itm = (from lst in _ctx.Lists
                        where lst.ListID == guidItem
                        select lst).FirstOrDefault();
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "EditList", "Definitions");
            }
            return View(_itm);
        }

        public ActionResult UpdateList(List item)
        {
            Entities _ctx = new Entities();
            try
            {
                if (ModelState.IsValid)
                {
                    item.CreatedOn = DateTime.Now;
                    _ctx.Lists.Attach(item);
                    _ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _ctx.SaveChanges();
                }
                else
                    return RedirectToAction("EditList", "Definitions");
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "UpdateList", "Definitions");
            }
            TempData["ListMessage"] = "List updated successfully";
            return RedirectToAction("List", "Definitions");
        }
    }
}