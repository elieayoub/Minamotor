using Models;
using Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Web.Controllers
{
    public class InventoryController : MainController
    {
        public ActionResult Index(int Status, string MakeID, string ModelID, string VehicleTrimID, long MinYear, long MaxYear, long MinPrice, long MaxPrice)
        {
            ViewBag.MakeList = new SelectList(dataHelper.MakeList.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.MakeID.ToString() }).AsEnumerable(), "Value", "Text");
            ViewBag.ModelList = dataHelper.ModelList;
            ViewBag.VehicleTrimList = dataHelper.VehicleTrimList;

            InventoryListView _model = new InventoryListView();
            _model.carViewList = new List<CarView>();
            _model.searchGridView = new SearchGridView
            {
                Status = Status,
                MakeID = MakeID,
                MaxPrice = MaxPrice,
                MaxYear = MaxYear,
                MinPrice = MinPrice,
                MinYear = MinYear,
                ModelID = ModelID,
                VehicleTrimID = VehicleTrimID
            };

            try
            {
                List<CarView> _lst = dataHelper.CarViewList;
                _lst = _lst.Where(obj => obj.car.Status == Status
                            && ((!String.IsNullOrEmpty(MakeID) && obj.car.MakeID == Guid.Parse(MakeID)) || (String.IsNullOrEmpty(MakeID)))
                            && ((!String.IsNullOrEmpty(ModelID) && obj.car.ModelID == Guid.Parse(ModelID)) || (String.IsNullOrEmpty(ModelID)))
                            && ((!String.IsNullOrEmpty(VehicleTrimID) && obj.car.VehicleTrimID == Guid.Parse(VehicleTrimID)) || (String.IsNullOrEmpty(VehicleTrimID)))
                            && (obj.car.Year >= MinYear && ((MaxYear != 0 && obj.car.Year <= MaxYear) || MaxYear == 0))
                            && (obj.car.Price >= MinPrice && ((MaxPrice != 0 && obj.car.Price <= MaxPrice) || MaxPrice == 0))
                        ).ToList();
                _model.carViewList = _lst;
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "Index", "Inventory");
            }

            return View(_model);
        }

        public ActionResult CarDetails(string carID)
        {
            CarDetailsView _model = new CarDetailsView();

            if (!String.IsNullOrEmpty(carID))
            {
                try
                {
                    Guid guidCar = Guid.Parse(carID);
                    Entities _ctx = new Entities();
                    _model = (from itm in _ctx.Cars
                              join mke in _ctx.Makes on itm.MakeID equals mke.MakeID
                              join mdl in _ctx.Models on itm.ModelID equals mdl.ModelID
                              join cur in _ctx.Currencies on itm.CurrencyID equals cur.CurrencyID
                              join vtm in _ctx.VehicleTrims on itm.VehicleTrimID equals vtm.VehicleTrimID
                              where itm.CarID == guidCar
                              select new CarDetailsView
                              {
                                  car = itm,
                                  makeName = mke.Name,
                                  modelName = mdl.Name,
                                  currencySymbol = cur.Symbol,
                                  vehicleTrimName = vtm.Name
                              })
                                                                            .AsEnumerable()
                                                                            .Select(obj => new CarDetailsView
                                                                            {
                                                                                car = obj.car,
                                                                                makeName = obj.makeName,
                                                                                modelName = obj.modelName,
                                                                                vehicleTrimName = obj.vehicleTrimName,
                                                                                currencySymbol = obj.currencySymbol,
                                                                                statusName = Enum.GetName(typeof(Web.Helpers.DataHelper.CarStatus), obj.car.Status),
                                                                                transmissionName = Enum.GetName(typeof(Web.Helpers.DataHelper.CarTransmission), obj.car.Transmission),
                                                                                conditionName = Enum.GetName(typeof(Web.Helpers.DataHelper.CarCondition), obj.car.Conditon)
                                                                            }).FirstOrDefault();

                    _model.carImage = (from item in _ctx.CarImages
                                       where item.CarID == guidCar
                                       select item).ToList();
                    return View(_model);
                }
                catch (Exception ex)
                {
                    functionHelper.InsertErrorLog(ex, "CarDetails", "Inventory");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public int ContactUs(string firstName, string phone, string email, string message, string hfMakeName, string hfModelName,
            string hfYear, string hfVIN)
        {
            int _success = 2;
            try
            {
                //SmtpClient mySmtpClient = new SmtpClient(functionHelper.GetFromConfig("SMTP"));
                //mySmtpClient.Port = Convert.ToInt16(functionHelper.GetFromConfig("SMTPPort"));

                //// set smtp-client with basicAuthentication
                //mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //mySmtpClient.UseDefaultCredentials = false;
                //mySmtpClient.EnableSsl = true;
                //System.Net.NetworkCredential basicAuthenticationInfo = new
                //   System.Net.NetworkCredential(functionHelper.GetFromConfig("EmailUsername"), functionHelper.GetFromConfig("EmailPassword"));
                //mySmtpClient.Credentials = basicAuthenticationInfo;

                //// add from,to mailaddresses
                //MailAddress from = new MailAddress(email, firstName);
                //MailAddress to = new MailAddress(functionHelper.GetFromConfig("DetailsContactUsEmail"));
                //MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                //// set subject and encoding
                //myMail.Subject = functionHelper.GetFromConfig("DetailsContactUsSubject");
                //myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                //// set body-message and encoding
                //myMail.Body = String.Format("<b>Make:</b> {1} <br /> <b>Model:</b> {2} <br /> <b>Year:</b> {3} <br /> <b>VIN:</b> {4} <br /><br />{0}",
                //    message.Replace("\n", "<br />"), hfMakeName, hfModelName, hfYear, hfVIN);
                //myMail.BodyEncoding = System.Text.Encoding.UTF8;
                //// text or html
                //myMail.IsBodyHtml = true;

                //mySmtpClient.Send(myMail);

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mailMessage = new MailMessage();

                mailMessage.To.Add(new MailAddress(functionHelper.GetFromConfig("DetailsContactUsEmail")));
                mailMessage.Subject = functionHelper.GetFromConfig("DetailsContactUsSubject");
                mailMessage.Body = String.Format("<b>Make:</b> {1} <br /> <b>Model:</b> {2} <br /> <b>Year:</b> {3} <br /> <b>VIN:</b> {4} <br /> <b>User Email:</b> {5} <br /> <b>User Phone:</b> {6} <br /><br />{0}",
                    message.Replace("\n", "<br />"), hfMakeName, hfModelName, hfYear, hfVIN, email, phone);
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);
                _success = 1;
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "ContactUs", "Inventory");
            }
            return _success;
        }
    }
}