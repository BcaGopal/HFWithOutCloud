using Data.Infrastructure;
using Data.Models;
using Jobs.Helpers;
using Model.Models;
using Model.ViewModels;
using Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Jobs.Controllers
{
    public class JobConsumptionAdsustmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        IPersonService _PersonService;
        IMaterialBalanceService _MaterialService;
        IDocumentTypeService _DocTypeService;
        public JobConsumptionAdsustmentController(IUnitOfWork unitOfWork, IPersonService PersonServices, IMaterialBalanceService MaterialServices,IDocumentTypeService DocTypeService, IExceptionHandlingService exec)
        {
            _exception = exec;
            _unitOfWork = unitOfWork;
            _PersonService = PersonServices;
            _MaterialService = MaterialServices;
            _DocTypeService = DocTypeService;
        }

        public ActionResult JobConsumptionAdsustment(int DocTypeId)
        {
            try
            {
                System.Web.HttpContext.Current.Cache.Remove("SaveData");
                StockHeadJobConjumptionViewModel vm = new StockHeadJobConjumptionViewModel();
                var list = (List<StockLineAndProcessViewModel>)System.Web.HttpContext.Current.Cache["Json"];
                vm.EntryDate = DateTime.Now;

                vm.DocTypeId = DocTypeId;
                vm.PersonId = list[0].PersonId;
                vm.ProcessId = list[0].ProcessId;
                int DivisionId = 1;
                int SiteId = 1;
                System.Web.HttpContext.Current.Session["Person"] = vm.PersonId;
                System.Web.HttpContext.Current.Session["process"] = vm.ProcessId;
                vm.EntryNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".StockHeaders", DocTypeId, vm.EntryDate, DivisionId, SiteId);

                return View("JobConsumptionAdsustment", vm);
            }
            catch (Exception ex) { throw ex; }
        }

        public ActionResult Posted()
        {
            List<StockLineAndProcessViewModel> list = (List<StockLineAndProcessViewModel>)System.Web.HttpContext.Current.Cache["Json"];
            if (list != null)
            {
                JsonResult json = Json(new { Success = true, Data = list.ToList() }, JsonRequestBehavior.AllowGet);
                
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
           
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveRecord(List<StockLineAndProcessViewModel> PostedViewModel)
        {
            try
            {
                List<StockLineAndProcessViewModel> PostedViewModel_NonZero = new List<StockLineAndProcessViewModel>();

                if (PostedViewModel.Count > 0)
                {
                    foreach (var item in PostedViewModel)
                    {
                        if (item.ConsumeQty != 0)
                        {
                            PostedViewModel_NonZero.Add(item);
                        }
                    }
                }
                int StockHeaderId = _MaterialService.InsertJobStockLineAProcesess(PostedViewModel_NonZero);

                return RedirectToAction("Modify", "JobConsumptionHeader", new { Id = StockHeaderId }).Success("Data saved successfully");

            }
            catch (Exception ex)
            {
               
                return View("Create", PostedViewModel);
            }
        }
        public JsonResult GetReceiveNo(string searchTerm, int pageSize, int pageNum)
        {
            int filter1 = (int)System.Web.HttpContext.Current.Session["Person"];
            int filter2 = (int)System.Web.HttpContext.Current.Session["process"];

            var Query = _MaterialService.GetRecieveNoProcessAndPerson(filter1, filter2, searchTerm);
            var temp = Query.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
            var count = Query.Count();
            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;
            return new JsonpResult
            {
                Data = Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SetSingleReceiveNo(int Ids)
        {
            ComboBoxResult RecieveJson = new ComboBoxResult();

            JobReceiveHeader Receive = (from b in db.JobReceiveHeader
                                        where b.JobWorkerId == Ids
                                        select new JobReceiveHeader
                                        {
                                            JobReceiveHeaderId = b.JobReceiveHeaderId,
                                            DocNo = b.DocNo
                                        }).FirstOrDefault();

            RecieveJson.id = Receive.JobReceiveHeaderId.ToString();
            RecieveJson.text = Receive.DocNo;

            return Json(RecieveJson);
        }
    }
}