using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModel;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace Service
{
    public interface IDashBoardAutoService : IDisposable
    {
        IEnumerable<DashBoardDoubleValue> GetSaleOrder();
        IEnumerable<DashBoardDoubleValue> GetSale();
        IEnumerable<DashBoardDoubleValue> GetPurchase();
        IEnumerable<DashBoardDoubleValue> GetPacking();
        IEnumerable<DashBoardSingleValue> GetSaleOrderBalance();
        IEnumerable<DashBoardSingleValue> GetPackedButNotShipped();
        IEnumerable<DashBoardSingleValue> GetJobOrderBalance();
        IEnumerable<DashBoardSingleValue> GetProcessReceive();


        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderDetailProductGroupWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleDetailProductGroupWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetPurchaseDetailProductGroupWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetPackingDetailProductGroupWise();

        IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailProductGroupWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetPackedButNotShippedDetailProductGroupWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetJobOrderBalanceDetailProductGroupWise();
        IEnumerable<DashBoardTabularData_ThreeColumns> GetProcessReceiveDetailProductGroupWise();


        IEnumerable<DashBoardPieChartData> GetSalePieChartData();
        IEnumerable<DashBoardSaleBarChartData> GetSaleBarChartData();
    }

    public class DashBoardAutoService : IDashBoardAutoService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";
        private readonly IUnitOfWorkForService _unitOfWork;

        DateTime? MonthStartDate = null;
        DateTime? MonthEndDate = null;
        DateTime? YearStartDate = null;
        DateTime? YearEndDate = null;
        DateTime? SoftwareStartDate = null;
        DateTime? TodayDate = null;
        DateTime? BarChartStartDate = null;


        public DashBoardAutoService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Init();
        }
        public DashBoardAutoService()
        {
            Init();
        }

        private void Init()
        {
            mQry = @"DECLARE @Month INT 
                    DECLARE @Year INT
                    SELECT @Month =  Datepart(MONTH,getdate())
                    SELECT @Year =  Datepart(YEAR,getdate())
                    DECLARE @FromDate DATETIME
                    DECLARE @ToDate DATETIME
                    SELECT DATEADD(month,@Month-1,DATEADD(year,@Year-1900,0)) As MonthStartDate, 
                    DATEADD(day,-1,DATEADD(month,@Month,DATEADD(year,@Year-1900,0))) As MonthEndDate,
                    CASE WHEN DATEPART(MM,GETDATE()) < 4 THEN DATEADD(MONTH,-9,DATEADD(DD,-DATEPART(DY,GETDATE())+1,GETDATE()))
                    ELSE DATEADD(MONTH,3,DATEADD(DD,-DATEPART(DY,GETDATE())+1,GETDATE())) END AS YearStartDate,
                    CASE WHEN DATEPART(MM,GETDATE()) < 4 THEN DATEADD(MONTH,-9,DATEADD(DD,-1,DATEADD(YY,DATEDIFF(YY,0,GETDATE())+1,0)))
                    ELSE DATEADD(MONTH,3,DATEADD(DD,-1,DATEADD(YY,DATEDIFF(YY,0,GETDATE())+1,0))) END AS YearEndDate,
                    Convert(DATETIME,'01/Apr/2001') AS SoftwareStartDate,
                    Convert(DATE,getdate()) As TodayDate ";
            SessnionValues SessnionValues = db.Database.SqlQuery<SessnionValues>(mQry).FirstOrDefault();

            MonthStartDate = SessnionValues.MonthStartDate;
            MonthEndDate = SessnionValues.MonthEndDate;
            YearStartDate = SessnionValues.YearStartDate;
            YearEndDate = SessnionValues.YearEndDate;
            SoftwareStartDate = SessnionValues.SoftwareStartDate;
            TodayDate = SessnionValues.TodayDate;

            BarChartStartDate = Convert.ToDateTime(MonthStartDate).AddMonths(-11);
        }

        public IEnumerable<DashBoardDoubleValue> GetSaleOrder()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetSaleOrderSummarySubQry(245) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> SaleOrder = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return SaleOrder;
        }
        public IEnumerable<DashBoardDoubleValue> GetSale()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetSaleSummarySubQry(244) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> Sale = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return Sale;
        }
        public IEnumerable<DashBoardDoubleValue> GetPurchase()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetPurchaseSummarySubQry(217) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> Purchase = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return Purchase;
        }
        public IEnumerable<DashBoardDoubleValue> GetPacking()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.MonthSale),0)) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.TodaySale),0)) AS Value2
                    From (" + GetPackingSummarySubQry(184) + ") As VMain ";

            IEnumerable<DashBoardDoubleValue> Packing = db.Database.SqlQuery<DashBoardDoubleValue>(mQry, SqlParameterFromDate, SqlParameterToDate, SqlParameterTodayDate).ToList();
            return Packing;
        }
        public IEnumerable<DashBoardSingleValue> GetSaleOrderBalance()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.BalanceAmount),0)) AS Value
                    From (" + GetSaleOrderBalanceDetailSubQry(245) + ") As VMain ";

            IEnumerable<DashBoardSingleValue> SaleOrderBalance = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterTodayDate).ToList();
            return SaleOrderBalance;
        }
        public IEnumerable<DashBoardSingleValue> GetPackedButNotShipped()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.BalanceAmount),0)) AS Value
                    From (" + GetPackedButNotDispatchedDetailSubQry(184) + ") As VMain ";

            IEnumerable<DashBoardSingleValue> PackedButNotShipped = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterTodayDate).ToList();
            return PackedButNotShipped;
        }
        public IEnumerable<DashBoardSingleValue> GetJobOrderBalance()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.BalanceQty),0)) AS Value
                    From (" + GetJobOrderBalanceDetailSubQry(309) + ") As VMain ";

            IEnumerable<DashBoardSingleValue> JobOrderBalance = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterTodayDate).ToList();
            return JobOrderBalance;
        }
        public IEnumerable<DashBoardSingleValue> GetProcessReceive()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"Select Convert(NVARCHAR,IsNull(Sum(VMain.Qty),0)) AS Value
                    From (" + GetProcessReceiveDetailSubQry(309) + ") As VMain ";

            IEnumerable<DashBoardSingleValue> ProcessReceive = db.Database.SqlQuery<DashBoardSingleValue>(mQry, SqlParameterTodayDate).ToList();
            return ProcessReceive;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetSaleOrderDetailSubQry(245) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }

        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetSaleDetailSubQry(244) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetPurchaseDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetPurchaseDetailSubQry(217) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetPackingDetailProductGroupWise()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetPackingDetailSubQry(184) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }






        public IEnumerable<DashBoardTabularData_ThreeColumns> GetSaleOrderBalanceDetailProductGroupWise()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.BalanceQty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.BalanceAmount),0)) AS Value2
                    FROM ( " + GetSaleOrderBalanceDetailSubQry(245) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetPackedButNotShippedDetailProductGroupWise()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.BalanceQty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.BalanceAmount),0)) AS Value2
                    FROM ( " + GetPackedButNotDispatchedDetailSubQry(184) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetJobOrderBalanceDetailProductGroupWise()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.BalanceQty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.BalanceAmount),0)) AS Value2
                    FROM ( " + GetJobOrderBalanceDetailSubQry(245) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }
        public IEnumerable<DashBoardTabularData_ThreeColumns> GetProcessReceiveDetailProductGroupWise()
        {
            SqlParameter SqlParameterTodayDate = new SqlParameter("@TodayDate", TodayDate);

            mQry = @"SELECT VMain.ProductGroup AS Head, Convert(NVARCHAR,Convert(Int,Sum(VMain.Qty))) AS Value1,
                    Convert(NVARCHAR,IsNull(Sum(VMain.Amount),0)) AS Value2
                    FROM ( " + GetProcessReceiveDetailSubQry(245) + @") As VMain
                    GROUP BY VMain.ProductGroup
                    ORDER BY VMain.ProductGroup ";

            IEnumerable<DashBoardTabularData_ThreeColumns> DashBoardTabularData_ThreeColumns = db.Database.SqlQuery<DashBoardTabularData_ThreeColumns>(mQry, SqlParameterTodayDate).ToList();
            return DashBoardTabularData_ThreeColumns;
        }

        public IEnumerable<DashBoardPieChartData> GetSalePieChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", MonthStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);

            mQry = @"SELECT Pg.ProductGroupName As label, Round(Sum(L.Amount)/100000,2) AS value,
                    CASE WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 1 THEN '#f56954'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 2 THEN '#00a65a'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 3 THEN '#f39c12'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 4 THEN '#00c0ef'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 5 THEN '#3c8dbc'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 6 THEN '#d2d6de'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 7 THEN '#c685c3'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 8 THEN '#b2d6ce'
                            WHEN row_number() OVER (ORDER BY Pg.ProductGroupName) = 9 THEN '#a2d6ce'
                            ELSE '#f56954'
                    END AS color 
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.SaleDispatchLines Sdl ON L.SaleDispatchLineId  = Sdl.SaleDispatchLineId
                    LEFT JOIN Web.PackingLines Pl ON Sdl.PackingLineId = Pl.PackingLineId
                    LEFT JOIN Web.Products P ON Pl.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    WHERE H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY Pg.ProductGroupName ";

            IEnumerable<DashBoardPieChartData> SalePieChartData = db.Database.SqlQuery<DashBoardPieChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return SalePieChartData;
        }
        public IEnumerable<DashBoardSaleBarChartData> GetSaleBarChartData()
        {
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", BarChartStartDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", MonthEndDate);


            mQry = @"SELECT LEFT(DATENAME(month, H.DocDate),3) AS Month, 
                    Round(Sum(Hc.Amount)/100000,2) AS Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    GROUP BY DATENAME(month, H.DocDate)
                    ORDER BY DatePart(Year,Max(H.DocDate)) + Convert(DECIMAL(18,2),DatePart(month,Max(H.DocDate))) / 100 ";

            IEnumerable<DashBoardSaleBarChartData> SaleBarChartData = db.Database.SqlQuery<DashBoardSaleBarChartData>(mQry, SqlParameterFromDate, SqlParameterToDate).ToList();
            return SaleBarChartData;
        }

        public string GetSaleOrderDetailSubQry(int DocumentCategoryId)
        {
            mQry = @" SELECT Pt.ProductTypeName AS ProductType, Pg.ProductGroupName AS ProductGroup, 
                        L.Qty As Qty, L.Amount As Amount
                        FROM Web.SaleOrderHeaders H 
                        LEFT JOIN Web.SaleOrderLines L ON H.SaleOrderHeaderId = L.SaleOrderHeaderId
                        LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
                        LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                        LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                        LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                        WHERE Pn.ProductNatureName Not In ('Addition/Deduction')
                        And H.DocDate BETWEEN @FromDate AND @ToDate
                        AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetSaleDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName AS ProductType, Pg.ProductGroupName AS ProductGroup, 
                    L.Qty As Qty, L.Amount As Amount
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId = L.SaleInvoiceHeaderId
                    LEFT JOIN Web.SaleDispatchLines Sdl ON L.SaleDispatchLineId = Sdl.SaleDispatchLineId
                    LEFT JOIN Web.PackingLines Pl ON Sdl.PackingLineId = Pl.PackingLineId
                    LEFT JOIN Web.Products P ON Pl.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    WHERE Pn.ProductNatureName Not In ('Addition/Deduction')
                    And H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();

            return mQry;
        }
        public string GetPurchaseDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName AS ProductType, Pg.ProductGroupName AS ProductGroup, 
                    L.Qty As Qty, L.Amount As Amount
                    FROM Web.JobInvoiceHeaders H 
                    LEFT JOIN Web.JobInvoiceLines L ON H.JobInvoiceHeaderId = L.JobInvoiceHeaderId
                    LEFT JOIN Web.JobReceiveLines Jrl ON L.JobReceiveLineId = Jrl.JobReceiveLineId
                    LEFT JOIN Web.Products P ON Jrl.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    WHERE Pn.ProductNatureName Not In ('Addition/Deduction')
                    AND H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();

            return mQry;
        }
        
        public string GetPackingDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName AS ProductType, Pg.ProductGroupName AS ProductGroup, 
                    L.Qty As Qty, IsNull(L.Qty,0) * IsNull(Sol.Rate,0) As Amount
                    FROM Web.PackingHeaders H 
                    LEFT JOIN Web.PackingLines L ON H.PackingHeaderId = L.PackingHeaderId
                    LEFT JOIN Web.SaleOrderLines SoL ON L.SaleOrderLineId = Sol.SaleOrderLineId
                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.ProductNatures Pn ON Pt.ProductNatureId = Pn.ProductNatureId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    WHERE Pn.ProductNatureName Not In ('Addition/Deduction')
                    AND H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }


        public string GetSaleOrderBalanceDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName As ProductType, Pg.ProductGroupName As ProductGroup, 
                    L.Qty - Isnull(VDispatch.Qty,0) AS BalanceQty, (L.Qty - Isnull(VDispatch.Qty,0)) * IsNull(L.Rate,0) AS BalanceAmount
                    FROM Web.SaleOrderHeaders H 
                    LEFT JOIN Web.SaleOrderLines L ON H.SaleOrderHeaderId = L.SaleOrderHeaderId
                    LEFT JOIN (
	                    SELECT Pl.SaleOrderLineId, Sum(L.Qty) AS Qty
	                    FROM Web.SaleInvoiceLines L 
	                    LEFT JOIN Web.SaleDispatchLines SdL ON L.SaleDispatchLineId = Sdl.SaleDispatchLineId
	                    LEFT JOIN Web.PackingLines Pl ON Sdl.PackingLineId = Pl.PackingLineId
	                    GROUP BY Pl.SaleOrderLineId
                    ) AS VDispatch ON L.SaleOrderLineId = VDispatch.SaleOrderLineId
                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.DocumentTypes D On H.DocTypeId = D.DocumentTypeId
                    WHERE H.DocDate <= @TodayDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetPackedButNotDispatchedDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName As ProductType, Pg.ProductGroupName As ProductGroup, 
                    L.Qty AS BalanceQty, L.Qty * IsNull(Sol.Rate,0) AS BalanceAmount
                    FROM Web.PackingHeaders H 
                    LEFT JOIN Web.PackingLines L ON H.PackingHeaderId = L.PackingHeaderId
                    LEFT JOIN Web.SaleDispatchLines Sdl ON L.PackingLineId = Sdl.PackingLineId
                    LEFT JOIN Web.SaleOrderLines Sol ON L.SaleOrderLineId = Sol.SaleOrderLineId
                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.DocumentTypes D On H.DocTypeId = D.DocumentTypeId
                    WHERE Sdl.SaleDispatchLineId IS NOT NULL
                    AND H.DocDate <= @TodayDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetJobOrderBalanceDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName As ProductType, Pg.ProductGroupName As ProductGroup, 
                    L.Qty - Isnull(VReceive.Qty,0) AS BalanceQty, (L.Qty - Isnull(VReceive.Qty,0)) * IsNull(L.Rate,0) AS BalanceAmount
                    FROM Web.JobOrderHeaders H 
                    LEFT JOIN Web.JobOrderLines L ON H.JobOrderHeaderId = L.JobOrderHeaderId
                    LEFT JOIN (
	                    SELECT L.JobOrderLineId, Sum(L.Qty) AS Qty
	                    FROM Web.JobReceiveLines L
	                    GROUP BY L.JobOrderLineId
                    ) AS VReceive ON L.JobOrderLineId = VReceive.JobOrderLineId
                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.DocumentTypes D On H.DocTypeId = D.DocumentTypeId
                    WHERE H.DocDate <= @TodayDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetProcessReceiveDetailSubQry(int DocumentCategoryId)
        {
            mQry = @"SELECT Pt.ProductTypeName As ProductType, Pg.ProductGroupName As ProductGroup, 
                    L.Qty AS Qty, L.Qty * IsNull(Jol.Rate,0) AS Amount
                    FROM Web.JobReceiveHeaders H 
                    LEFT JOIN Web.JobReceiveLines L ON H.JobReceiveHeaderId = L.JobReceiveHeaderId
                    LEFT JOIN Web.JobOrderLines Jol ON L.JobOrderLineId = Jol.JobOrderLineId
                    LEFT JOIN Web.Products P ON L.ProductId = P.ProductId
                    LEFT JOIN Web.ProductGroups Pg ON P.ProductGroupId = Pg.ProductGroupId
                    LEFT JOIN Web.ProductTypes Pt ON Pg.ProductTypeId = Pt.ProductTypeId
                    LEFT JOIN Web.DocumentTypes D On H.DocTypeId = D.DocumentTypeId
                    WHERE H.DocDate <= @TodayDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }



        public string GetSaleOrderSummarySubQry(int DocumentCategoryId)
        {
            mQry = @" SELECT IsNull(Sum(VLine.Amount),0) AS MonthSale,
                        IsNull(Sum(Case When H.DocDate = @TodayDate Then VLine.Amount Else 0 End),0) AS TodaySale
                        FROM Web.SaleOrderHeaders H 
                        LEFT JOIN (
	                        SELECT L.SaleOrderHeaderId, Sum(L.Amount) AS Amount
	                        FROM Web.SaleOrderLines L 
	                        GROUP BY L.SaleOrderHeaderId
                        ) AS VLine ON H.SaleOrderHeaderId = VLine.SaleOrderHeaderId
                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                        WHERE H.DocDate BETWEEN @FromDate AND @ToDate
                        AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetSaleSummarySubQry(int DocumentCategoryId)
        {
            mQry = @" SELECT IsNull(Sum(Hc.Amount),0) AS MonthSale,
                    IsNull(Sum(Case When H.DocDate = @TodayDate Then Hc.Amount Else 0 End),0) AS TodaySale
                    FROM Web.SaleInvoiceHeaders H 
                    LEFT JOIN Web.SaleInvoiceHeaderCharges Hc ON H.SaleInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetPurchaseSummarySubQry(int DocumentCategoryId)
        {
            mQry = @" SELECT IsNull(Sum(Hc.Amount),0) AS MonthSale,
                    IsNull(Sum(Case When H.DocDate = @TodayDate Then Hc.Amount Else 0 End),0) AS TodaySale
                    FROM Web.JobInvoiceHeaders H 
                    LEFT JOIN Web.JobInvoiceHeaderCharges Hc ON H.JobInvoiceHeaderId = Hc.HeaderTableId
                    LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                    LEFT JOIN Web.Charges C ON Hc.ChargeId = C.ChargeId
                    WHERE C.ChargeName = 'Net Amount'
                    AND  H.DocDate BETWEEN @FromDate AND @ToDate
                    AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }
        public string GetPackingSummarySubQry(int DocumentCategoryId)
        {
            mQry = @" SELECT IsNull(Sum(VLine.Amount),0) AS MonthSale,
                        IsNull(Sum(Case When H.DocDate = @TodayDate Then VLine.Amount Else 0 End),0) AS TodaySale
                        FROM Web.PackingHeaders H 
                        LEFT JOIN (
                            SELECT L.PackingHeaderId, Sum(L.Qty * Sol.Rate) AS Amount
                            FROM Web.PackingLines L 
                            LEFT JOIN Web.SaleOrderLines Sol ON L.SaleOrderLineId = Sol.SaleOrderLineId
                            GROUP BY L.PackingHeaderId
                        ) AS VLine ON H.PackingHeaderId = VLine.PackingHeaderId
                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                        WHERE H.DocDate BETWEEN @FromDate AND @ToDate
                        AND D.DocumentCategoryId = " + DocumentCategoryId.ToString();
            return mQry;
        }



        public string GetFormattedValue(string FieldName)
        {
            string Value = @" SELECT 
                            CASE WHEN IsNull(@Value, 0) <= 100000 THEN Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 1000, 2))) +' Thousand'
                                WHEN IsNull(@Value,0) <= 10000000 THEN Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 100000, 2))) +' Lakh'
     ELSE Convert(NVARCHAR, Convert(DECIMAL(18, 2), Round(IsNull(@Value, 0) / 10000000, 2))) END     ";

            return Value;
        }

        public void Dispose()
        {
        }
    }
    public class DashBoardPieChartData
    {
        public string label { get; set; }
        public Decimal value { get; set; }
        public string color { get; set; }
        public string highlight { get; set; }
    }
    public class DashBoardSaleBarChartData
    {
        public string Month { get; set; }
        public Decimal Amount { get; set; }
    }

    
    public class DashBoardTabularData
    {
        public string Head { get; set; }
        public string Value { get; set; }
    }
    public class DashBoardSingleValue
    {
        public string Value { get; set; }
    }

    public class DashBoardTabularData_ThreeColumns
    {
        public string Head { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }
    public class DashBoardDoubleValue
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    public class DashBoardTrippleValue
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
    }

    public class SessnionValues
    {
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthEndDate { get; set; }
        public DateTime YearStartDate { get; set; }
        public DateTime YearEndDate { get; set; }
        public DateTime SoftwareStartDate { get; set; }
        public DateTime TodayDate { get; set; }
    }

}
