using Data.Infrastructure;
using Data.Models;
using Service;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace Jobs.Controllers
{
    public class MaterialBalanceUpdateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        IPersonService _PersonService;
        IMaterialBalanceService _MaterialService;
        private bool EventException = false;
        public MaterialBalanceUpdateController(IUnitOfWork unitOfWork,IPersonService PersonServices ,IMaterialBalanceService MaterialServices,IExceptionHandlingService exec)
        {
            _exception = exec;
            _unitOfWork = unitOfWork;
            _PersonService = PersonServices;
            _MaterialService = MaterialServices;
        }
        public ActionResult MaterialBalanceUpdate(int PersonId, int ProcessId)
        {
            MaterialBalanceUpDateViewModel vm = new MaterialBalanceUpDateViewModel();
            vm.AsOnDate = "01/Apr/" + DateTime.Now.Date.Year.ToString();
            vm.JobWorker = PersonId.ToString();
            vm.Process = ProcessId.ToString();
            vm.NegativeBalance = false;
            vm.ReportHeaderCompanyDetail = _MaterialService.GetReportHeaderCompanyDetail();
            return View("MaterialBalanceUpdate",vm);
        }
        public ActionResult MaterialBalanceFill(MaterialBalanceUpDateViewModel vm)
        {
            string UserName = User.Identity.Name;
            IEnumerable<MatereialBalanceJobWorkerViewModel> MaterialBalance  = _MaterialService.MaterialBalanceDisplay(vm);
            if (MaterialBalance != null)
            {
                JsonResult json = Json(new { Success = true, Data = MaterialBalance.ToList() }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Post(List<StockLineAndProcessViewModel> PostedViewModel)
        {
            try { 
            MaterialBalanceUpDateViewModel vm = new MaterialBalanceUpDateViewModel();
            List<StockLineAndProcessViewModel> list = PostedViewModel.ToList();
            System.Web.HttpContext.Current.Cache.Remove("Json");
            System.Web.HttpContext.Current.Cache["Json"] = list;
            if (list != null)
            {
                JsonResult json = Json(new { Success = true, Data = list.ToList() }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            else
                {
                    ModelState.AddModelError("Error", list.ToString());
                }
            
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                throw ex;
                //string message = _exception.HandleException(ex);
                //TempData["CSEXC"] += message;
                //EventException = true;
                //return ex;
            }

        }
    }
}