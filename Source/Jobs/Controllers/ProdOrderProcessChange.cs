using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModel;
using AutoMapper;
using System.Xml.Linq;
using Jobs.Helpers;
using System.Linq;
using Model.ViewModels;

namespace Jobs.Controllers
{
    [Authorize]
    public class ProdOrderProcessChangeController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();
        List<string> UserRoles = new List<string>();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public ProdOrderProcessChangeController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }
        // GET: /ProductMaster/



        // GET: /ProductMaster/Create

        public ActionResult Create(int id)
        {
            ProdOrderProcessChangeViewModel vm = new ProdOrderProcessChangeViewModel();

            var ProdOrderHeader = (from H in db.ProdOrderHeader where H.MaterialPlanHeaderId == id
                                   select H).FirstOrDefault();

            if (ProdOrderHeader != null)
                vm.ProdOrderHeaderId = ProdOrderHeader.ProdOrderHeaderId;

            return View("Create",vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(ProdOrderProcessChangeViewModel vm)
        {
            ProdOrderHeader Header = db.ProdOrderHeader.Find(vm.ProdOrderHeaderId);

            if (vm.ProdOrderHeaderId == 0)
                ModelState.AddModelError("ProdOrderHeaderID", "Production Order is required.");
            if (vm.ProductId == 0)
                ModelState.AddModelError("ProductId", "Product is required.");
            if (vm.OldProcessId == 0)
                ModelState.AddModelError("OldProcessId", "Old Process is required.");
            if (vm.NewProcessId == 0)
                ModelState.AddModelError("NewProcessId", "New Process is required.");

            if (ModelState.IsValid)
            {
                IEnumerable<ProdOrderLine> ProdOrderLineList = (from L in db.ProdOrderLine
                                                                where L.ProdOrderHeaderId == vm.ProdOrderHeaderId
                                                                && L.ProductId == vm.ProductId
                                                                && L.ProcessId == vm.OldProcessId
                                                                select L).ToList();

                foreach (ProdOrderLine item in ProdOrderLineList)
                {
                    var line = db.JobOrderLine.Where(i => i.ProdOrderLineId == item.ProdOrderLineId);
                    if (line == null)
                    {
                        string message = "Job Order is created for this selection.";
                        ModelState.AddModelError("", message);
                        return View("Create", vm);
                    }
                }

                foreach (ProdOrderLine item in ProdOrderLineList)
                {
                    item.ProcessId = vm.NewProcessId;
                    item.ObjectState = Model.ObjectState.Modified;
                    db.ProdOrderLine.Add(item);
                }

                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return View("Create", vm);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = Header.DocTypeId,
                    DocId = vm.ProdOrderHeaderId,
                    ActivityType = (int)ActivityTypeContants.Added,
                }));

                return RedirectToAction("Index", "ProdOrderHeader", new { id = Header.DocTypeId, IndexType = "" }).Success("Data saved successfully");
            }

            return View("Create", vm);
        }
        public ActionResult GetProdOrderList(string searchTerm, int pageSize, int pageNum, int filter)//DocTypeId
        {
            var Query = GetPendingProdOrders(filter, searchTerm);
            var temp = Query.Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .ToList();

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
        public IQueryable<ComboBoxResult> GetPendingProdOrders(int Id, string term)
        {
            int DocTypeId = Id;
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var settings = new JobOrderSettingsService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);

            string DivIdStr = "|" + DivisionId.ToString() + "|";
            string SiteIdStr = "|" + SiteId.ToString() + "|";


            var list = (from H in db.ProdOrderHeader
                          join L in db.ProdOrderLine on H.ProdOrderHeaderId equals L.ProdOrderHeaderId into ProdOrderLineTable
                          from ProdOrderLineTab in ProdOrderLineTable.DefaultIfEmpty()
                          join Jol in db.JobOrderLine on ProdOrderLineTab.ProdOrderLineId equals Jol.ProdOrderLineId into JobOrderLineTable
                          from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
                          where JobOrderLineTab.JobOrderLineId == null
                          group new { H } by new { H.ProdOrderHeaderId } into Result
                          select new ComboBoxResult
                          {
                              id = Result.Key.ProdOrderHeaderId.ToString(),
                              text = Result.Max(i => i.H.DocNo)
                          });

            return list;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class ProdOrderProcessChangeViewModel
    {
        public int DocTypeId { get; set; }
        public int ProdOrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public int OldProcessId { get; set; }
        public int NewProcessId { get; set; }
    }
}
