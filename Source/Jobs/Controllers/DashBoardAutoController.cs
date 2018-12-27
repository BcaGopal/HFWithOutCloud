using System.Collections.Generic;
using System.Web.Mvc;
using Service;

//using ProjLib.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Module
{
    [Authorize]
    public class DashBoardAutoController : Controller
    {
        IDashBoardAutoService _DashBoardAutoService;
        public DashBoardAutoController(IDashBoardAutoService DashBoardAutoService)
        {
            _DashBoardAutoService = DashBoardAutoService;
        }

        public ActionResult DashBoardAuto()
        {
            return View();
        }
        public JsonResult GetSaleOrder()
        {
            IEnumerable<DashBoardDoubleValue> SaleOrder = _DashBoardAutoService.GetSaleOrder();

            JsonResult json = Json(new { Success = true, Data = SaleOrder }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSale()
        {
            IEnumerable<DashBoardDoubleValue> Sale = _DashBoardAutoService.GetSale();

            JsonResult json = Json(new { Success = true, Data = Sale }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetPurchase()
        {
            IEnumerable<DashBoardDoubleValue> Purchase = _DashBoardAutoService.GetPurchase();

            JsonResult json = Json(new { Success = true, Data = Purchase }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetPacking()
        {
            IEnumerable<DashBoardDoubleValue> Packing = _DashBoardAutoService.GetPacking();

            JsonResult json = Json(new { Success = true, Data = Packing }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleOrderBalance()
        {
            IEnumerable<DashBoardSingleValue> SaleOrderBalance = _DashBoardAutoService.GetSaleOrderBalance();

            JsonResult json = Json(new { Success = true, Data = SaleOrderBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetPackedButNotShipped()
        {
            IEnumerable<DashBoardSingleValue> PackedButNotShipped = _DashBoardAutoService.GetPackedButNotShipped();

            JsonResult json = Json(new { Success = true, Data = PackedButNotShipped }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetJobOrderBalance()
        {
            IEnumerable<DashBoardSingleValue> JobOrderBalance = _DashBoardAutoService.GetJobOrderBalance();

            JsonResult json = Json(new { Success = true, Data = JobOrderBalance }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetProcessReceive()
        {
            IEnumerable<DashBoardSingleValue> ProcessReceive = _DashBoardAutoService.GetProcessReceive();

            JsonResult json = Json(new { Success = true, Data = ProcessReceive }, JsonRequestBehavior.AllowGet);
            return json;
        }


        public JsonResult GetSalePieChartData()
        {
            IEnumerable<DashBoardPieChartData> SalePieChartData = _DashBoardAutoService.GetSalePieChartData();

            JsonResult json = Json(new { Success = true, Data = SalePieChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleChartData()
        {
            IEnumerable<DashBoardSaleBarChartData> SaleChartData = _DashBoardAutoService.GetSaleBarChartData();

            JsonResult json = Json(new { Success = true, Data = SaleChartData }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public JsonResult GetSaleOrderDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> SaleOrderDetailProductGroupWise = _DashBoardAutoService.GetSaleOrderDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = SaleOrderDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetSaleDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> SaleDetailProductGroupWise = _DashBoardAutoService.GetSaleDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = SaleDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetPurchaseDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> PurchaseDetailProductGroupWise = _DashBoardAutoService.GetPurchaseDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = PurchaseDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetPackingDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> PackingDetailProductGroupWise = _DashBoardAutoService.GetPackingDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = PackingDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }



        public JsonResult GetSaleOrderBalanceDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> SaleOrderBalanceDetailProductGroupWise = _DashBoardAutoService.GetSaleOrderBalanceDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = SaleOrderBalanceDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetPackedButNotShippedDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> PackedButNotShippedDetailProductGroupWise = _DashBoardAutoService.GetPackedButNotShippedDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = PackedButNotShippedDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetJobOrderBalanceDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> JobOrderBalanceDetailProductGroupWise = _DashBoardAutoService.GetJobOrderBalanceDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = JobOrderBalanceDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetProcessReceiveDetailProductGroupWise()
        {
            IEnumerable<DashBoardTabularData_ThreeColumns> ProcessReceiveDetailProductGroupWise = _DashBoardAutoService.GetProcessReceiveDetailProductGroupWise();

            JsonResult json = Json(new { Success = true, Data = ProcessReceiveDetailProductGroupWise }, JsonRequestBehavior.AllowGet);
            return json;
        }
    }   
}