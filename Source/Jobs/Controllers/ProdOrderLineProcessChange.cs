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
    public class ProdOrderLineProcessChangeController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();
        List<string> UserRoles = new List<string>();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public ProdOrderLineProcessChangeController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
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
            ProdOrderLineProcessChangeViewModel vm = new ProdOrderLineProcessChangeViewModel();

            var ProdOrderHeader = (from H in db.ProdOrderHeader where H.MaterialPlanHeaderId == id
                                   select H).FirstOrDefault();

            if (ProdOrderHeader != null)
            {
                vm.MaterialPlanHeaderId = id;
                vm.ProdOrderHeaderId = ProdOrderHeader.ProdOrderHeaderId;
                vm.DocumentTypeSettings = new DocumentTypeSettingsService(_unitOfWork).GetDocumentTypeSettingsForDocument(ProdOrderHeader.DocTypeId);
            }

            return View("Create",vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(ProdOrderLineProcessChangeViewModel vm)
        {
            ProdOrderHeader Header = db.ProdOrderHeader.Find(vm.ProdOrderHeaderId);

            if (vm.ProdOrderLineId == 0)
                ModelState.AddModelError("ProdOrderLineId", "Product is required.");
            if (vm.NewProcessId == 0)
                ModelState.AddModelError("NewProcessId", "New Process is required.");

            if (ModelState.IsValid)
            {
                IEnumerable<ProdOrderLine> ProdOrderLineList = (from L in db.ProdOrderLine
                                                                where L.ProdOrderLineId == vm.ProdOrderLineId
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

                //return RedirectToAction("Index", "ProdOrderHeader", new { id = Header.DocTypeId, IndexType = "" }).Success("Data saved successfully");
                //return RedirectToAction("Create", "ProdOrderLineProcessChange", new { id = vm.ProdOrderHeaderId}).Success("Data saved successfully");
                //return Create(vm.MaterialPlanHeaderId);
                return RedirectToAction("Create", new { id = vm.MaterialPlanHeaderId });
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


            var list = (from L in db.ProdOrderLine 
                          join Jol in db.JobOrderLine on L.ProdOrderLineId equals Jol.ProdOrderLineId into JobOrderLineTable
                          from JobOrderLineTab in JobOrderLineTable.DefaultIfEmpty()
                          where L.ProdOrderHeaderId == Id && JobOrderLineTab.JobOrderLineId == null
                            && (
                            string.IsNullOrEmpty(term) ? 1 == 1 : L.ProdOrderHeader.DocNo.ToLower().Contains(term.ToLower())
                            || string.IsNullOrEmpty(term) ? 1 == 1 : L.Product.ProductName.ToLower().Contains(term.ToLower())
                            || string.IsNullOrEmpty(term) ? 1 == 1 : L.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
                            || string.IsNullOrEmpty(term) ? 1 == 1 : L.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                            || string.IsNullOrEmpty(term) ? 1 == 1 : L.Dimension3.Dimension3Name.ToLower().Contains(term.ToLower())
                            || string.IsNullOrEmpty(term) ? 1 == 1 : L.Dimension4.Dimension4Name.ToLower().Contains(term.ToLower())
                            || string.IsNullOrEmpty(term) ? 1 == 1 : L.Process.ProcessName.ToLower().Contains(term.ToLower())
                            )
                            orderby L.Product.ProductName
                        select new ComboBoxResult
                          {
                              id = L.ProdOrderLineId.ToString(),
                              text = L.ProdOrderHeader.DocNo,
                              AProp1 = L.Dimension1.Dimension1Name + ", " + L.Dimension2.Dimension2Name,
                              AProp2 = L.Dimension3.Dimension3Name + ", " + L.Dimension4.Dimension4Name,
                              TextProp1 = L.Product.ProductName,
                              TextProp2 = L.Process.ProcessName,
                        });

            return list;
        }
        public JsonResult GetProdOrderDetail(int ProdOrderLineId)
        {
            var temp = (from L in db.ProdOrderLine
                    where L.ProdOrderLineId == ProdOrderLineId
                    select new 
                    {
                        ProductName = L.Product.ProductName,
                        Dimension1Name = L.Dimension1.Dimension1Name,
                        Dimension2Name = L.Dimension2.Dimension2Name,
                        Dimension3Name = L.Dimension3.Dimension3Name,
                        Dimension4Name = L.Dimension4.Dimension4Name,
                        ProcessName = L.Process.ProcessName,
                    }).FirstOrDefault();

            return Json(temp);
        }

        public JsonResult SetSingleProdOrderLine(int Ids)
        {
            ComboBoxResult ProdOrderJson = new ComboBoxResult();

            var ProdOrderLine = from L in db.ProdOrderLine
                                join H in db.ProdOrderHeader on L.ProdOrderHeaderId equals H.ProdOrderHeaderId into ProdOrderHeaderTable
                                from ProdOrderHeaderTab in ProdOrderHeaderTable.DefaultIfEmpty()
                                where L.ProdOrderLineId == Ids
                                select new
                                {
                                    ProdOrderLineId = L.ProdOrderLineId,
                                    ProdOrderNo = L.ProdOrderHeader.DocNo
                                };

            ProdOrderJson.id = ProdOrderLine.FirstOrDefault().ToString();
            ProdOrderJson.text = ProdOrderLine.FirstOrDefault().ProdOrderNo;

            return Json(ProdOrderJson);
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

    public class ProdOrderLineProcessChangeViewModel
    {
        public int MaterialPlanHeaderId { get; set; }
        public int ProdOrderHeaderId { get; set; }
        public int ProdOrderLineId { get; set; }
        public string ProductName { get; set; }
        public string Dimension1Name { get; set; }
        public string Dimension2Name { get; set; }
        public string Dimension3Name { get; set; }
        public string Dimension4Name { get; set; }
        public string OldProcessName { get; set; }
        public int NewProcessId { get; set; }
        public DocumentTypeSettingsViewModel DocumentTypeSettings { get; set; }
    }
}
