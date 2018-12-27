using Data.Infrastructure;
using Data.Models;
using Model.ViewModels;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jobs.Controllers
{
    public class MaterialBalanceAtJobworkerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        IMaterialBalanceService _MaterialServices;
      public MaterialBalanceAtJobworkerController(IUnitOfWork unitOfWork, IMaterialBalanceService MaterialBalanceServices , IExceptionHandlingService exec)
      {
            _exception = exec;
            _unitOfWork = unitOfWork;
            _MaterialServices = MaterialBalanceServices;
      }
        public ActionResult MaterialBalanceJobWorker(int PersonId)
        {
            return View("MaterialBalanceJobWorker");
        }
        public JsonResult GetPersonBalanceDetails(int PersonId)
        {
            var p = _MaterialServices.GetPersonValues(PersonId);
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonValuesDetails(int PersonId)
        {
            var p = _MaterialServices.GetPersonWiseBalance(PersonId);
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonInformation(int PersonId)
        {
            var p = _MaterialServices.GetPersonInfo(PersonId);
            return Json(p, JsonRequestBehavior.AllowGet);
        }

    }
}