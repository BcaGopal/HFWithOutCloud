using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Models;
using Service;
using Data.Infrastructure;
using Model.ViewModels;
using System.Data.SqlClient;
using Model.ViewModel;
using Core.Common;
using Jobs.Helpers;

namespace Web
{
    [Authorize]
    public class ProductJobHistoryController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        IExceptionHandlingService _exception;
        public ProductJobHistoryController()
        {
            _exception = new ExceptionHandlingService();
        }
        // GET: /GateMaster/

        public ActionResult Index(int? ProductId)
        {
            ViewBag.ProductId = ProductId;
            return View();
        }



        public JsonResult AjaxGetJsonData(int? ProductId , int? Dimension1Id , int? Dimension2Id , int? Dimension3Id, int? Dimension4Id)
        {
            string search = Request.Form["search[value]"];
            int sortColumn = -1;
            string sortDirection = "asc";

            DataTableData dataTableData = new DataTableData();
            int recordsFiltered = 0;
            dataTableData.data = FilterData(ref recordsFiltered, ref TOTAL_ROWS, search, sortColumn, sortDirection, ProductId, Dimension1Id, Dimension2Id, Dimension3Id, Dimension4Id);
            dataTableData.recordsTotal = TOTAL_ROWS;
            dataTableData.recordsFiltered = recordsFiltered;

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AjaxGetInvoiceJsonData(int? ProductId, int? Dimension1Id, int? Dimension2Id, int? Dimension3Id, int? Dimension4Id)
        {
            string search = Request.Form["search[value]"];
            int sortColumn = -1;
            string sortDirection = "asc";

            DataTableData dataTableData = new DataTableData();
            int recordsFiltered = 0;
            var fdata = InvoiceFilterData(ref recordsFiltered, ref TOTAL_ROWS, search, sortColumn, sortDirection, ProductId, Dimension1Id, Dimension2Id, Dimension3Id, Dimension4Id);
            dataTableData.recordsTotal = TOTAL_ROWS;
            dataTableData.recordsFiltered = recordsFiltered;

            return Json(new { data = fdata, recordsTotal = TOTAL_ROWS, recordsFiltered = recordsFiltered }, JsonRequestBehavior.AllowGet);
        }


        private static int TOTAL_ROWS = 0;
        //private static readonly List<DataItem> _data = CreateData();    
        public class DataTableData
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<JobOrderHeaderViewModel> data { get; set; }
        }


        // here we simulate SQL search, sorting and paging operations
        private List<JobOrderHeaderViewModel> FilterData(ref int recordFiltered, ref int recordTotal,
         string search, int sortColumn, string sortDirection, int? ProductId , int? Dimension1Id, int? Dimension2Id, int? Dimension3Id, int? Dimension4Id)
        {

            var JobOrders = (from p in db.JobOrderLine
                             join t in db.JobOrderHeader on p.JobOrderHeaderId equals t.JobOrderHeaderId
                             where (string.IsNullOrEmpty(ProductId.ToString()) ? 1 == 1 : p.ProductId == ProductId)
                              && (string.IsNullOrEmpty(Dimension1Id.ToString()) ? 1==1 : p.Dimension1Id==Dimension1Id)
                              && (string.IsNullOrEmpty(Dimension2Id.ToString()) ? 1 == 1 :  p.Dimension2Id == Dimension2Id)
                              && (string.IsNullOrEmpty(Dimension3Id.ToString()) ? 1 == 1 :  p.Dimension3Id == Dimension3Id)
                             && (string.IsNullOrEmpty(Dimension4Id.ToString()) ? 1 ==  1 :  p.Dimension4Id == Dimension4Id)
                              //&& ((p.Dimension1Id == null)? 1 == 1 : p.Dimension1Id == Dimension1Id)
                             //&& ((p.Dimension2Id == null) ? 1 == 1 : p.Dimension2Id == Dimension2Id)
                             // && ((p.Dimension3Id == null) ? 1 == 1 : p.Dimension3Id == Dimension3Id)
                             //&& ((p.Dimension4Id == null) ? 1 == 1 : p.Dimension4Id == Dimension4Id)
                             join dt in db.DocumentType on t.DocTypeId equals dt.DocumentTypeId
                             join dc in db.DocumentCategory on dt.DocumentCategoryId equals dc.DocumentCategoryId
                             join per in db.Persons on t.JobWorkerId equals per.PersonID
                             where dc.DocumentCategoryName == TransactionDocCategoryConstants.PurchaseOrder
                             group new { t, dt, per, p } by p.JobOrderHeaderId into g
                             orderby g.Max(m => m.t.DocDate) descending, g.Max(m => m.t.DocNo) descending
                             select new JobOrderHeaderViewModel
                             {
                                 DocDate = g.Max(m => m.t.DocDate),
                                 DocNo = g.Max(m => m.t.DocNo),
                                 DocTypeName = g.Max(m => m.dt.DocumentTypeName),
                                 JobOrderHeaderId = g.Max(m => m.t.JobOrderHeaderId),
                                 DocTypeId=g.Max(m=>m.t.DocTypeId),
                                 Remark = g.Max(m => m.t.Remark),
                                 JobWorkerName = g.Max(m => m.per.Name),
                                 Rate = g.Max(m => m.p.Rate),
                             }).Take(5).ToList();




            recordTotal = JobOrders.Count();

            recordFiltered = JobOrders.Count();

            return (from p in JobOrders
                    select new JobOrderHeaderViewModel
                    {
                        sDocDate = p.DocDate.ToString("dd/MMM/yyyy"),
                        DocNo = p.DocNo,
                        DocTypeName = p.DocTypeName,
                        JobOrderHeaderId = p.JobOrderHeaderId,
                        DocTypeId = p.DocTypeId,
                        Remark = p.Remark,
                        JobWorkerName = p.JobWorkerName,
                        Rate = p.Rate,
                    }).ToList();

        }


        // here we simulate SQL search, sorting and paging operations
        private List<JobInvoiceHeaderViewModel> InvoiceFilterData(ref int recordFiltered, ref int recordTotal,
            string search, int sortColumn, string sortDirection, int? ProductId, int? Dimension1Id, int? Dimension2Id, int? Dimension3Id, int? Dimension4Id)
        {

            var JobOrders = (from p in db.JobInvoiceLine
                             join t in db.JobInvoiceHeader on p.JobInvoiceHeaderId equals t.JobInvoiceHeaderId
                             join pr in db.JobReceiveLine on p.JobReceiveLineId equals pr.JobReceiveLineId
                             join pl in db.JobOrderLine on pr.JobOrderLineId equals pl.JobOrderLineId
                             where ((string.IsNullOrEmpty(ProductId.ToString()) ? 1 == 1 : pl.ProductId == ProductId))
                                 && ((string.IsNullOrEmpty(Dimension1Id.ToString()) ? 1 == 1 : pl.Dimension1Id == Dimension1Id))
                                 && ((string.IsNullOrEmpty(Dimension2Id.ToString()) ? 1 == 1 : pl.Dimension2Id == Dimension2Id))
                                 && ((string.IsNullOrEmpty(Dimension3Id.ToString()) ? 1 == 1 : pl.Dimension3Id == Dimension3Id))
                                 && ((string.IsNullOrEmpty(Dimension4Id.ToString()) ? 1 == 1 : pl.Dimension4Id == Dimension4Id))
                                 //&& ((pl.Dimension1Id == null) ? 1 == 1 : pl.Dimension1Id == Dimension1Id)
                                 //&& ((pl.Dimension2Id == null) ? 1 == 1 : pl.Dimension2Id == Dimension2Id)
                                 //&& ((pl.Dimension3Id == null) ? 1 == 1 : pl.Dimension3Id == Dimension3Id)
                                 //&& ((pl.Dimension4Id == null) ? 1 == 1 : pl.Dimension4Id == Dimension4Id)
                             join dt in db.DocumentType on t.DocTypeId equals dt.DocumentTypeId
                                 join dc in db.DocumentCategory on dt.DocumentCategoryId equals dc.DocumentCategoryId
                                 join per in db.Persons on t.JobWorkerId equals per.PersonID
                                 where dc.DocumentCategoryName == TransactionDocCategoryConstants.PurchaseInvoice
                                 group new { t, dt, per, p } by p.JobInvoiceHeaderId into g
                                 orderby g.Max(m => m.t.DocDate) descending, g.Max(m => m.t.DocNo) descending
                                 select new JobInvoiceHeaderViewModel
                                 {
                                     DocDate = g.Max(m => m.t.DocDate),
                                     DocNo = g.Max(m => m.t.DocNo),
                                     DocTypeName = g.Max(m => m.dt.DocumentTypeName),
                                     JobInvoiceHeaderId = g.Max(m => m.t.JobInvoiceHeaderId),
                                     DocTypeId=g.Max(m=>m.t.DocTypeId),
                                     Remark = g.Max(m => m.t.Remark),
                                     JobWorkerName = g.Max(m => m.per.Name),
                                     Rate = g.Max(m => m.p.Rate),
                                 }).Take(5).ToList();
            
            recordTotal = JobOrders.Count();

            recordFiltered = JobOrders.Count();

            return (from p in JobOrders
                    select new JobInvoiceHeaderViewModel
                    {
                        sDocDate = p.DocDate.ToString("dd/MMM/yyyy"),
                        DocNo = p.DocNo,
                        DocTypeName = p.DocTypeName,
                        JobInvoiceHeaderId = p.JobInvoiceHeaderId,
                        DocTypeId = p.DocTypeId,
                        Remark = p.Remark,
                        JobWorkerName = p.JobWorkerName,
                        Rate = p.Rate,
                    }).ToList();

         }

        public ActionResult RecordMenu(int id)
        {
            var PurchaseRec = db.JobOrderHeader.Find(id);

            string RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["AdminSetupDomain"] + "/Redirect/RedirectToDocument?DocTypeId=" + PurchaseRec.DocTypeId + "&DocId=" + (PurchaseRec.JobOrderHeaderId);

            return Redirect(RedirectUrl);
        }

        public ActionResult RecInvMenu(int id)
        {
            var PurchaseRec = db.JobInvoiceHeader.Find(id);

            string RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["AdminSetupDomain"] + "/Redirect/RedirectToDocument?DocTypeId=" + PurchaseRec.DocTypeId + "&DocId=" + (PurchaseRec.JobInvoiceHeaderId);

            return Redirect(RedirectUrl);
        }
        public ActionResult GetProducts(string searchTerm, int pageSize, int pageNum)//DocTypeId
        {
            return new JsonpResult { Data = new dbProductService(db).GetProductHelpListWithNatureType(searchTerm, pageSize, pageNum), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
}
