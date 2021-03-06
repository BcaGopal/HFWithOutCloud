﻿using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModels;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Configuration;


namespace Service
{
    public interface IStockLineService : IDisposable
    {
        StockLine Create(StockLine s);
        void Delete(int id);
        void Delete(StockLine s);
        StockLineViewModel GetStockLine(int id);
        StockLineViewModel GetStockLineForIssue(int id);
        StockLineViewModel GetStockLineForReceive(int id);
        StockLine Find(int id);
        void Update(StockLine s);
        IEnumerable<StockLine> GetStockLineforDelete(int id);
        IEnumerable<StockLineViewModel> GetStockLineListForIndex(int id);
        IEnumerable<StockLineViewModel> GetStockLineListForIssueReceive(int id, bool Issue);
        IEnumerable<ProductBalanceForProcessViewModel> GetProcessBalanceProducts(int ProcessId, int? CostCenterId, int HeaderId);
        IEnumerable<StockLineViewModel> GetJobConsumptionForFilters(StockLineFilterViewModel vm);
        IEnumerable<ComboBoxList> GetPendingProdFromStocProcBal(int? CostCenterId, int ProcessId, int JobWorkerId, string term);
        IEnumerable<ComboBoxList> GetPendingCCFromStockProcBal(int StockHeaderId, int? CostCenterId, int ProcessId, int JobWorkerId, string term);
        StockLineViewModel GetJobConsumptionLine(int id);
        StockLineViewModel GetRateConversionLine(int id);
        RequisitionLineViewModel GetRequisitionBalanceForIssue(int id);
        IEnumerable<ProductHelpListViewModel> GetProductHelpList(int Id, int PersonId, string term, int Limit);
        IEnumerable<ComboBoxList> GetPendingRequisitionHelpList(int Id, int PersonId, string term);
        IEnumerable<ComboBoxList> GetProductHelpListForFilters(int Id, int PersonId, string term, int Limit);
        IEnumerable<StockLineViewModel> GetRequisitionsForFilters(RequisitionFiltersForIssue vm, bool All);
        IEnumerable<StockExchangeLineViewModel> GetRequisitionsForExchange(RequisitionFiltersForExchange vm);
        IEnumerable<StockReceiveLineViewModel> GetRequisitionsForReceive(RequisitionFiltersForReceive vm);
        IEnumerable<StockReceiveLineViewModel> GetStockProcessForReceive(StockProcessFiltersForReceive vm);
        IEnumerable<ComboBoxList> GetPendingCostCenterHelpList(int Id, int PersonId, string term);
        decimal GetExcessStock(int ProductId, int? Dim1, int? Dim2, int? ProcId, string Lot, int MaterilIssueId, string ProcName);
        decimal GetExcessStockForTransfer(int ProductId, int? Dim1, int? Dim2, int? ProcId, string Lot, int MaterilIssueId, string ProcName);
        IQueryable<ComboBoxResult> GetCostCentersForIssueFilters(int id, string term);
        IQueryable<ComboBoxResult> GetProductHelpList(int id, string term);
        IQueryable<ComboBoxResult> GetCostCentersForLine(int id, string term);
        IEnumerable<StockExchangeLineViewModel> GetProcessTransfersForFilters(FiltersForProcessTransfer vm);
        IQueryable<ComboBoxResult> GetCostCentersForProcessTransfer(int SiteId, int DivisionId, string FilterDocTypes, string term, int? PersonId);
        IEnumerable<StockLineViewModel> GetStockLineListForTransfer(int id);
        int GetMaxSr(int id);
        IQueryable<ComboBoxResult> GetCustomProducts(int Id, string term);
        IQueryable<ComboBoxResult> GetRequisitionsForReceive(int Id, string term);
        UIDValidationViewModel ValidateBarCodeOnStock(string ProductUidName, int StockHeader);
        UIDValidationViewModel ValidateBarCodeOnStockReceive(string ProductUidName, int StockHeader);
        decimal  ProductStockProcessBalance(int ProductId, int StockHeaderId);
        IQueryable<ComboBoxResult> GetCostCentersForReceive(int id, string term);
        IQueryable<ComboBoxResult> GetProductsForReceiveFilters(int id, string term);
        IQueryable<ComboBoxResult> GetProductsForReceiveLine(int id, string term);
        IQueryable<ComboBoxResult> GetDimension1ForReceive(int id, string term);
        IQueryable<ComboBoxResult> GetDimension2ForReceive(int id, string term);

        IEnumerable<ComboBoxResult> GetPendingStockInForIssue(int id, int? ProductId, int? Dimension1Id, int? Dimension2Id, int? Dimension3Id, int? Dimension4Id, string term);
        IEnumerable<ComboBoxResult> GetPendingStockInHeaderForIssue(int StockHeaderId, string term);
        IEnumerable<ComboBoxResult> GetLotNo(int StockHeaderId, string term);

        IQueryable<ComboBoxResult> GetCustomProductGroups(int Id, string term);
        IQueryable<ComboBoxResult> GetCustomReferenceDocIds(int Id, string term);
        ComboBoxResult SetCustomReferenceDocIds(int StockLineId);
        IEnumerable<ComboBoxResult> FGetProductUidHelpList(int Id, string term);
        IEnumerable<StockLineViewModel> GetStockInForFilters(StockInFiltersForIssue vm);
        IEnumerable<ComboBoxResult> GetStockProcessBalanceForReceive(int StockHeaderId, string term);
        IEnumerable<StockIssueForProductsFilterViewModel> GetProductsForFilters(ProductsFiltersForIssue svm);
        IEnumerable<StockLineViewModel> GetBOMDetailForProducts(ProductsFilterViewModel vm);
        IEnumerable<StockExchangeLineViewModel> GetReceiveProductBomForExchange(int StockHeaderId);

    }

    public class StockLineService : IStockLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public StockLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public StockLine Create(StockLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<StockLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<StockLine>().Delete(id);
        }

        public void Delete(StockLine s)
        {
            _unitOfWork.Repository<StockLine>().Delete(s);
        }

        public void Update(StockLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<StockLine>().Update(s);
        }


        public StockLineViewModel GetStockLine(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Weight  = p.Weight,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            PlanNo = p.PlanNo,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            PersonId = p.StockHeader.PersonId,
                            CostCenterId = p.CostCenterId,
                            LockReason = p.LockReason,
                            ProductCode = tab.ProductCode,
                            ReferenceDocId = p.ReferenceDocId,
                            ReferenceDocTypeId = p.ReferenceDocTypeId,
                            StockInId = p.StockInId,
                            StockInNo = p.StockIn.StockHeader.DocNo
                        }).FirstOrDefault();

            return temp;
        }


        public StockLineViewModel GetStockLineForReceive(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        join sh in db.StockHeader on p.StockHeaderId equals sh.StockHeaderId
                        join per in db.Persons on sh.PersonId equals per.PersonID into PerTable
                        from PerTab in PerTable.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Weight = p.Weight,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            PersonId = p.StockHeader.PersonId,
                            PersonName = (PerTab == null ? "" : PerTab.Name + " | " + PerTab.Code),
                            CostCenterId = p.CostCenterId,
                            LockReason = p.LockReason,
                            ReferenceDocId = p.ReferenceDocId,
                            ReferenceDocTypeId = p.ReferenceDocTypeId
                        }

                        ).FirstOrDefault();

            if (temp.RequisitionLineId != null)
            {
                var Rec = (from p in db.RequisitionLineStatus
                           where p.RequisitionLineId == p.RequisitionLineId
                           select p).FirstOrDefault();
                if (Rec != null)
                    temp.RequisitionBalanceQty = Rec.IssueQty;
            }

            return temp;
        }

        public StockLineViewModel GetStockLineForIssue(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        join t in db.ViewRequisitionBalance on p.RequisitionLineId equals t.RequisitionLineId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join sh in db.StockHeader on p.StockHeaderId equals sh.StockHeaderId
                        join per in db.Persons on sh.PersonId equals per.PersonID into PerTable
                        from PerTab in PerTable.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            RequiredProductId = p.RequisitionLine.ProductId,
                            RequiredProductName = p.RequisitionLine.Product.ProductName,
                            RequisitionBalanceQty = p.Qty + tab2.BalanceQty,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Weight = p.Weight,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            PlanNo = p.PlanNo,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            PersonId = p.StockHeader.PersonId,
                            PersonName = (PerTab == null ? "" : PerTab.Name + " | " + PerTab.Code),
                            CostCenterId = p.CostCenterId,
                            StockInId = p.StockInId,
                            StockInNo = p.StockIn.StockHeader.DocNo,
                            LockReason = p.LockReason,
                            ReferenceDocId = p.ReferenceDocId,
                            ReferenceDocTypeId = p.ReferenceDocTypeId
                        }).FirstOrDefault();




            if (temp.RequisitionLineId != null)
            {
                temp.RequisitionBalanceQty = (from p in db.ViewMaterialRequestBalance
                                              where p.RequisitionLineId == temp.RequisitionLineId
                                              select p.BalanceQty
                                                ).FirstOrDefault() + temp.Qty;
            }

            return temp;
        }

        public StockLine Find(int id)
        {
            return _unitOfWork.Repository<StockLine>().Find(id);
        }

        public IEnumerable<StockLine> GetStockLineforDelete(int id)
        {
            return (from p in db.StockLine
                    where p.StockHeaderId == id
                    select p
                        );
        }

        public IEnumerable<StockLineViewModel> GetStockLineListForIndex(int id)
        {

            return (from p in db.StockLine
                    where p.StockHeaderId == id
                    orderby p.Sr
                    select new StockLineViewModel
                    {
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        Dimension3Name = p.Dimension3.Dimension3Name,
                        Dimension4Name = p.Dimension4.Dimension4Name,
                        LotNo = p.LotNo,
                        PlanNo = p.PlanNo,
                        RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                        Qty = p.Qty,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        Rate = p.Rate,
                        Amount = p.Amount,
                        Remark = p.Remark,
                        StockLineId = p.StockLineId,
                        StockHeaderId = p.StockHeaderId,
                        UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                        ProductUidIdName = p.ProductUid.ProductUidName,
                        FromProcessName = p.FromProcess.ProcessName,
                        CostCenterName = p.CostCenter.CostCenterName,
                    }
                        );

        }

        public IEnumerable<StockLineViewModel> GetStockLineListForIssueReceive(int id, bool Issue)
        {

            return (from p in db.StockLine
                    //join t in db.Stock on p.StockId equals t.StockId
                    //where p.StockHeaderId == id && (Issue ? t.Qty_Iss > 0 : t.Qty_Rec > 0)
                    where p.StockHeaderId == id && (Issue ? p.DocNature == StockNatureConstants.Issue : p.DocNature == StockNatureConstants.Receive)
                    orderby p.Sr
                    select new StockLineViewModel
                    {
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        Dimension3Name = p.Dimension3.Dimension3Name,
                        Dimension4Name = p.Dimension4.Dimension4Name,
                        LotNo = p.LotNo,
                        RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                        Qty = p.Qty,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        Rate = p.Rate,
                        Weight = p.Weight,
                        Amount = p.Amount,
                        Remark = p.Remark,
                        StockLineId = p.StockLineId,
                        StockHeaderId = p.StockHeaderId,
                        UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                        ProductUidIdName = p.ProductUid.ProductUidName,
                        FromProcessName = p.FromProcess.ProcessName,
                        CostCenterName = p.CostCenter.CostCenterName,
                        StockInId = p.StockInId,
                        StockInNo = p.StockIn.StockHeader.DocNo,
                    });

        }


        public IEnumerable<StockLineViewModel> GetStockLineListForTransfer(int id)
        {

            return (from p in db.StockLine
                    //join t in db.Stock on p.StockId equals t.StockId
                    //where p.StockHeaderId == id && (Issue ? t.Qty_Iss > 0 : t.Qty_Rec > 0)
                    where p.StockHeaderId == id && p.DocNature == StockNatureConstants.Transfer
                    select new StockLineViewModel
                    {
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        Dimension3Name = p.Dimension3.Dimension3Name,
                        Dimension4Name = p.Dimension4.Dimension4Name,
                        LotNo = p.LotNo,
                        RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                        Qty = p.Qty,
                        UnitId = p.Product.UnitId,
                        UnitName = p.Product.Unit.UnitName,
                        Rate = p.Rate,
                        Amount = p.Amount,
                        Weight = p.Weight,
                        Remark = p.Remark,
                        StockLineId = p.StockLineId,
                        StockHeaderId = p.StockHeaderId,
                        UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                        ProductUidIdName = p.ProductUid.ProductUidName,
                        FromProcessName = p.FromProcess.ProcessName,
                        CostCenterName = p.CostCenter.CostCenterName,

                    }
                        );

        }



        public IEnumerable<ProductBalanceForProcessViewModel> GetProcessBalanceProducts(int ProcessId, int? CostCenterId, int HeaderId)
        {
            //SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            //SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);

            //IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".ProcStockProcessBalance @ProcessId, @CostCenterId", SqlParameterProcessId, SqlParameterCostCenterId).ToList();

            StockHeader Header = new StockHeaderService(_unitOfWork).Find(HeaderId);

            SqlParameter SqlParameterPersonIdId = new SqlParameter("@PersonId", Header.PersonId);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonIdId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();


            return FifoSaleOrderLineList;
        }

        public decimal ProductStockProcessBalance(int ProductId, int StockHeaderId)
        {

            StockHeader Header = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            SqlParameter SqlParameterPersonIdId = new SqlParameter("@PersonId", Header.PersonId);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", Header.ProcessId ?? 0);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", -1);
            SqlParameter SqlParameterProductId = new SqlParameter("@ProductId", ProductId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId, @ProductId ", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonIdId, SqlParameterProcessId, SqlParameterCostCenterId, SqlParameterProductId).ToList();

            if (FifoSaleOrderLineList.Count() > 0)
            {
                return FifoSaleOrderLineList.FirstOrDefault().BalanceQty;
            }
            else
                return 0;

        }

        public IEnumerable<StockLineViewModel> GetJobConsumptionForFilters(StockLineFilterViewModel vm)
        {

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }

            string[] Dimension1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dimension1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }

            string[] Dimension2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dimension2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }

            string[] Dimension3IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension3Id)) { Dimension3IdArr = vm.Dimension3Id.Split(",".ToCharArray()); }

            string[] Dimension4IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension4Id)) { Dimension4IdArr = vm.Dimension4Id.Split(",".ToCharArray()); }


            string[] CostCenterId = null;
            if (!string.IsNullOrEmpty(vm.CostCenterIds)) { CostCenterId = vm.CostCenterIds.Split(",".ToCharArray()); }

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", vm.ProcessId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vm.JobWorkerId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", vm.CostCenterId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            //IEnumerable<StockLineViewModel> StockProcessBalance = db.Database.SqlQuery<StockLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate,@SiteId,@DivisionId,@PersonId,@ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();
            IEnumerable<StockLineViewModel> StockProcessBalance = db.Database.SqlQuery<StockLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalanceOnlyProductWise  @UptoDate,@SiteId,@DivisionId,@PersonId,@ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();

            var temp = from H in StockProcessBalance
                       join Proc in db.Process on H.ProcessId equals Proc.ProcessId into table
                       from tab in table.DefaultIfEmpty()
                       where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(H.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(H.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dimension1IdArr.Contains(H.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dimension2IdArr.Contains(H.Dimension2Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension3Id) ? 1 == 1 : Dimension3IdArr.Contains(H.Dimension3Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension4Id) ? 1 == 1 : Dimension4IdArr.Contains(H.Dimension4Id.ToString()))
                        && (string.IsNullOrEmpty(vm.CostCenterIds) ? 1 == 1 : CostCenterId.Contains(H.CostCenterId.ToString()))
                        && H.BalanceQty != 0
                       select new StockLineViewModel
                       {
                           StockHeaderId = vm.StockHeaderId,
                           Dimension1Id = H.Dimension1Id,
                           Dimension1Name = H.Dimension1Name,
                           Dimension2Id = H.Dimension2Id,
                           Dimension2Name = H.Dimension2Name,
                           Dimension3Id = H.Dimension3Id,
                           Dimension3Name = H.Dimension3Name,
                           Dimension4Id = H.Dimension4Id,
                           Dimension4Name = H.Dimension4Name,
                           Specification = H.Specification,
                           Qty = H.BalanceQty,
                           BalanceQty = H.BalanceQty,
                           UnitName = H.UnitName,
                           UnitDecimalPlaces = H.UnitDecimalPlaces,
                           //UnitId=P.UnitId,
                           Rate = vm.Rate,
                           ProductName = H.ProductName,
                           ProductId = H.ProductId,
                           FromProcessId = tab == null ? null : (int?)tab.ProcessId,
                           ProcessName = tab == null ? null : tab.ProcessName,
                           CostCenterId = H.CostCenterId,
                           CostCenterName = H.CostCenterName,
                           LotNo = H.LotNo
                       };

            return temp;


        }

        public IEnumerable<ComboBoxList> GetPendingProdFromStocProcBal(int? CostCenterId, int ProcessId, int JobWorkerId, string term)//DocTypeId
        {

            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", JobWorkerId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate,@SiteId,@DivisionId,@PersonId, @ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();



            return (from p in FifoSaleOrderLineList
                    where p.BalanceQty != 0 && (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                    group new { p } by p.ProductId into g
                    orderby g.Key descending
                    select new ComboBoxList
                    {
                        Id = g.Key,
                        PropFirst = g.Max(m => m.p.ProductName)
                    }
                        );
        }

        public IEnumerable<ComboBoxList> GetPendingCCFromStockProcBal(int StockHeaderId, int? CostCenterId, int ProcessId, int JobWorkerId, string term)//DocTypeId
        {

            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", CostCenterId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", JobWorkerId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            StockHeader header = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(header.DocTypeId, header.DivisionId, header.SiteId);

            string ProcName = "";

            if (string.IsNullOrEmpty(settings.SqlProcStockProcessBalance))
            {
                ProcName = "Web.spStockProcessBalance";
            }
            else
            {
                ProcName = settings.SqlProcStockProcessBalance;
            }

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>(ProcName + "  @UptoDate,@SiteId,@DivisionId,@PersonId, @ProcessId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId).ToList();



            return (from p in FifoSaleOrderLineList
                    where p.BalanceQty != 0 && p.CostCenterName != null && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                    group new { p } by p.CostCenterId into g
                    orderby g.Key descending
                    where g.Key != null
                    select new ComboBoxList
                    {
                        Id = g.Key.Value,
                        PropFirst = g.Max(m => m.p.CostCenterName)
                    }
                        ).Take(40).ToList();
        }

        public StockLineViewModel GetJobConsumptionLine(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            CostCenterId = p.CostCenterId,
                            CostCenterName = p.CostCenter.CostCenterName,
                            LockReason = p.LockReason,
                        }

                        ).FirstOrDefault();



            StockHeader header = new StockHeaderService(_unitOfWork).Find(temp.StockHeaderId);

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", header.ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", (object)temp.CostCenterId ?? DBNull.Value);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", header.PersonId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();

            temp.BalanceQty = (from p in FifoSaleOrderLineList
                               where p.BalanceQty > 0 && p.Dimension1Id == temp.Dimension1Id && p.Dimension2Id == temp.Dimension2Id
                               && p.Dimension3Id == temp.Dimension3Id && p.Dimension4Id == temp.Dimension4Id
                               && p.Specification == temp.Specification && p.LotNo == temp.LotNo
                               select p.BalanceQty).FirstOrDefault() + temp.Qty;

            return temp;

        }


        public StockLineViewModel GetRateConversionLine(int id)
        {
            var temp = (from p in db.StockLine
                        join t in db.Product on p.ProductId equals t.ProductId into table
                        from tab in table.DefaultIfEmpty()
                        where p.StockLineId == id
                        select new StockLineViewModel
                        {
                            Qty = p.Qty,
                            Remark = p.Remark,
                            StockHeaderId = p.StockHeaderId,
                            ProductUidId = p.ProductUidId,
                            ProductUidIdName = p.ProductUid.ProductUidName,
                            StockLineId = p.StockLineId,
                            ProductName = tab.ProductName,
                            RequisitionLineId = p.RequisitionLineId,
                            RequisitionHeaderDocNo = p.RequisitionLine.RequisitionHeader.DocNo,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            Specification = p.Specification,
                            Rate = p.Rate,
                            Amount = p.Amount,
                            ProductId = p.ProductId,
                            FromProcessId = p.FromProcessId,
                            LotNo = p.LotNo,
                            UnitId = p.Product.UnitId,
                            CostCenterId = p.CostCenterId,
                            CostCenterName = p.CostCenter.CostCenterName,
                            LockReason = p.LockReason,
                        }

                        ).FirstOrDefault();



            StockHeader header = new StockHeaderService(_unitOfWork).Find(temp.StockHeaderId);

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", header.ProcessId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", (object)temp.CostCenterId ?? DBNull.Value);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", header.PersonId);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<ProductBalanceForProcessViewModel> FifoSaleOrderLineList = db.Database.SqlQuery<ProductBalanceForProcessViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate, @SiteId, @DivisionId, @PersonId, @ProcessId, @CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();


            temp.BalanceQty = (from p in FifoSaleOrderLineList
                               where p.BalanceQty > 0 && p.Dimension1Id == temp.Dimension1Id && p.Dimension2Id == temp.Dimension2Id
                               && p.Dimension3Id == temp.Dimension3Id && p.Dimension4Id == temp.Dimension4Id
                               && p.Specification == temp.Specification && p.LotNo == temp.LotNo
                               select p.BalanceQty).FirstOrDefault() + temp.Qty;

            return temp;

        }


        public RequisitionLineViewModel GetRequisitionBalanceForIssue(int id)
        {
            return (from b in db.ViewRequisitionBalance
                    join p in db.RequisitionLine on b.RequisitionLineId equals p.RequisitionLineId
                    join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                    where b.RequisitionLineId == id
                    select new RequisitionLineViewModel
                    {
                        RequisitionHeaderDocNo = b.RequisitionNo,
                        RequisitionLineId = b.RequisitionLineId,
                        Qty = b.BalanceQty,
                        Remark = p.Remark,
                        Dimension1Id = p.Dimension1Id,
                        Dimension1Name = p.Dimension1.Dimension1Name,
                        Dimension2Id = p.Dimension2Id,
                        Dimension2Name = p.Dimension2.Dimension2Name,
                        Dimension3Id = p.Dimension3Id,
                        Dimension3Name = p.Dimension3.Dimension3Name,
                        Dimension4Id = p.Dimension4Id,
                        Dimension4Name = p.Dimension4.Dimension4Name,
                        ProductId = p.ProductId,
                        ProductName = p.Product.ProductName,
                        Specification = p.Specification,
                        UnitName = p.Product.Unit.UnitName,
                        ProcessId = p.ProcessId,
                        ProcessName = p.Process.ProcessName,
                        CostCenterId = t.CostCenterId,
                        CostCenterName = t.CostCenter.CostCenterName,
                    }).FirstOrDefault();

        }


        public IEnumerable<ProductHelpListViewModel> GetProductHelpList(int Id, int PersonId, string term, int Limit)
        {
            var Stock = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        join t2 in db.RequisitionLine on p.RequisitionLineId equals t2.RequisitionLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : t2.Specification.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension3.Dimension3Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension4.Dimension4Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower())
                        )
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == SiteId : ContraSites.Contains(t.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == DivisionId : ContraDivisions.Contains(t.DivisionId.ToString()))
                        && p.PersonId == PersonId
                        orderby t.DocDate, t.DocNo
                        select new ProductHelpListViewModel
                        {
                            ProductName = p.Product.ProductName,
                            ProductId = p.ProductId,
                            Specification = t2.Specification,
                            Dimension1Name = p.Dimension1.Dimension1Name,
                            Dimension2Name = p.Dimension2.Dimension2Name,
                            Dimension3Name = p.Dimension3.Dimension3Name,
                            Dimension4Name = p.Dimension4.Dimension4Name,
                            HeaderDocNo = p.RequisitionNo,
                            LineId = p.RequisitionLineId,
                            BalanceQty = p.BalanceQty,
                        }
                          ).Take(Limit);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetPendingRequisitionHelpList(int Id, int PersonId, string term)
        {

            var GoodsReceipt = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(GoodsReceipt.DocTypeId, GoodsReceipt.DivisionId, GoodsReceipt.SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower())) && p.PersonId == GoodsReceipt.PersonId && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && p.PersonId == PersonId
                        group new { p } by p.RequisitionHeaderId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.p.RequisitionNo),
                            Id = g.Key,
                        }
                          ).Take(20);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetPendingCostCenterHelpList(int Id, int PersonId, string term)
        {

            var GoodsReceipt = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(GoodsReceipt.DocTypeId, GoodsReceipt.DivisionId, GoodsReceipt.SiteId);

            string[] contraDocTypes = null;
            if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
            else { contraDocTypes = new string[] { "NA" }; }

            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.RequisitionNo.ToLower().Contains(term.ToLower())) && p.PersonId == GoodsReceipt.PersonId && p.BalanceQty > 0
                        && (string.IsNullOrEmpty(settings.filterContraDocTypes) ? 1 == 1 : contraDocTypes.Contains(t.DocTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && p.PersonId == PersonId
                        group new { p } by p.CostCenterId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.p.CostCenter.CostCenterName),
                            Id = (int)g.Key,
                        }
                          ).Take(20);

            return list.ToList();
        }


        public IEnumerable<ComboBoxList> GetProductHelpListForFilters(int Id, int PersonId, string term, int Limit)
        {
            var Stock = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            var list = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        join t2 in db.RequisitionLine on p.RequisitionLineId equals t2.RequisitionLineId
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(t.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(t.DivisionId.ToString()))
                        && p.PersonId == PersonId
                        orderby t.DocDate, t.DocNo
                        group p by p.ProductId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.Product.ProductName),
                            Id = g.Key,
                        }
                          ).Take(Limit);

            return list.ToList();
        }



        public IEnumerable<StockLineViewModel> GetRequisitionsForFilters(RequisitionFiltersForIssue vm, bool All)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId)) { SaleOrderIdArr = vm.RequisitionHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] Dim3IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension3Id)) { Dim3IdArr = vm.Dimension3Id.Split(",".ToCharArray()); }
            else { Dim3IdArr = new string[] { "NA" }; }

            string[] Dim4IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension4Id)) { Dim4IdArr = vm.Dimension4Id.Split(",".ToCharArray()); }
            else { Dim4IdArr = new string[] { "NA" }; }

            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            var temp = (from p in db.ViewRequisitionBalance
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join product in db.Product on p.ProductId equals product.ProductId into table2
                        from tab2 in table2.DefaultIfEmpty()
                        join PG in db.ProductGroups on tab2.ProductGroupId equals PG.ProductGroupId into tablePG
                        from tabPG in tablePG.DefaultIfEmpty()
                        join t2 in db.RequisitionLine on p.RequisitionLineId equals t2.RequisitionLineId into table3
                        from tab3 in table3.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.RequisitionHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(tab2.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dim1IdArr.Contains(tab3.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dim2IdArr.Contains(tab3.Dimension2Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension3Id) ? 1 == 1 : Dim3IdArr.Contains(tab3.Dimension3Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension4Id) ? 1 == 1 : Dim4IdArr.Contains(tab3.Dimension4Id.ToString()))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(tab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(tab.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(tab.DivisionId.ToString()))
                        && (All ? 1 == 1 : p.BalanceQty > 0) && p.PersonId == vm.PersonId
                        orderby p.ProductId, p.Dimension1Id, p.Dimension2Id, p.Dimension3Id, p.Dimension4Id
                        select new StockLineViewModel
                        {
                            Dimension1Name = tab3.Dimension1.Dimension1Name,
                            Dimension2Name = tab3.Dimension2.Dimension2Name,
                            Dimension3Name = tab3.Dimension3.Dimension3Name,
                            Dimension4Name = tab3.Dimension4.Dimension4Name,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            ProductGroupId = tabPG.ProductGroupId,
                            ProductGroupName = tabPG.ProductGroupName.Replace(" ", "").Replace("/", ""),
                            ProcessId = tab3.ProcessId,
                            Specification = tab3.Specification,
                            RequisitionBalanceQty = p.BalanceQty,
                            Qty = 0,
                            RequisitionHeaderDocNo = tab.DocNo,
                            ProductName = tab2.ProductName,
                            ProductId = p.ProductId,
                            CostCenterId = p.CostCenterId,
                            StockHeaderId = vm.StockHeaderId,
                            RequisitionLineId = p.RequisitionLineId,
                            UnitId = tab2.UnitId,
                            UnitName = tab2.Unit.UnitName,
                            UnitDecimalPlaces = tab2.Unit.DecimalPlaces,
                        }



                        );
            return temp;
        }


        public IEnumerable<StockExchangeLineViewModel> GetRequisitionsForExchange(RequisitionFiltersForExchange vm)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId)) { SaleOrderIdArr = vm.RequisitionHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] Dim3IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension3Id)) { Dim3IdArr = vm.Dimension3Id.Split(",".ToCharArray()); }
            else { Dim3IdArr = new string[] { "NA" }; }

            string[] Dim4IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension4Id)) { Dim4IdArr = vm.Dimension4Id.Split(",".ToCharArray()); }
            else { Dim4IdArr = new string[] { "NA" }; }


            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            var temp = (from p in db.RequisitionLine
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
                        from tab in table.DefaultIfEmpty()
                        join t2 in db.StockLine on p.RequisitionLineId equals t2.RequisitionLineId into sltable
                        from stockline in sltable.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : (stockline == null ? ProductIdArr.Contains(p.ProductId.ToString()) : ProductIdArr.Contains(stockline.ProductId.ToString())))
                        && (string.IsNullOrEmpty(vm.RequisitionHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(p.Product.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : (stockline == null ? Dim1IdArr.Contains(p.Dimension1Id.ToString()) : Dim1IdArr.Contains(stockline.Dimension1Id.ToString())))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : (stockline == null ? Dim2IdArr.Contains(p.Dimension2Id.ToString()) : Dim2IdArr.Contains(stockline.Dimension2Id.ToString())))
                        && (string.IsNullOrEmpty(vm.Dimension3Id) ? 1 == 1 : (stockline == null ? Dim3IdArr.Contains(p.Dimension3Id.ToString()) : Dim3IdArr.Contains(stockline.Dimension3Id.ToString())))
                        && (string.IsNullOrEmpty(vm.Dimension4Id) ? 1 == 1 : (stockline == null ? Dim4IdArr.Contains(p.Dimension4Id.ToString()) : Dim4IdArr.Contains(stockline.Dimension4Id.ToString())))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(tab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
                         && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(tab.SiteId.ToString()))
                          && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(tab.DivisionId.ToString()))
                         && tab.PersonId == vm.PersonId
                        join t in db.ViewRequisitionBalance on p.RequisitionLineId equals t.RequisitionLineId into reqtable
                        from reqtab in reqtable.DefaultIfEmpty()
                        select new StockExchangeLineViewModel
                        {
                            Dimension1Name = (stockline == null ? p.Dimension1.Dimension1Name : stockline.Dimension1.Dimension1Name),
                            Dimension2Name = (stockline == null ? p.Dimension2.Dimension2Name : stockline.Dimension2.Dimension2Name),
                            Dimension3Name = (stockline == null ? p.Dimension3.Dimension3Name : stockline.Dimension3.Dimension3Name),
                            Dimension4Name = (stockline == null ? p.Dimension4.Dimension4Name : stockline.Dimension4.Dimension4Name),
                            Specification = (stockline == null ? p.Specification : stockline.Specification),
                            Dimension1Id = (stockline == null ? p.Dimension1Id : stockline.Dimension1Id),
                            Dimension2Id = (stockline == null ? p.Dimension2Id : stockline.Dimension2Id),
                            Dimension3Id = (stockline == null ? p.Dimension3Id : stockline.Dimension3Id),
                            Dimension4Id = (stockline == null ? p.Dimension4Id : stockline.Dimension4Id),
                            ProcessId = p.ProcessId,
                            RequisitionHeaderDocNo = tab.DocNo,
                            ProductName = (stockline == null ? p.Product.ProductName : stockline.Product.ProductName),
                            ProductId = p.ProductId,
                            CostCenterId = tab.CostCenterId,
                            CostCenterName = tab.CostCenter.CostCenterName,
                            StockHeaderId = vm.StockHeaderId,
                            RequisitionLineId = p.RequisitionLineId,
                            UnitId = p.Product.UnitId,
                            UnitName = p.Product.Unit.UnitName,
                            UnitDecimalPlaces = p.Product.Unit.DecimalPlaces,
                            RequisitionBalanceQty = reqtab.BalanceQty,
                        }



                        );
            return temp;
        }

        public IEnumerable<StockExchangeLineViewModel> GetReceiveProductBomForExchange(int StockHeaderId)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            int Process_Manufacturing = (from P in db.Process where P.ProcessName == ProcessConstants.Manufacturing select P).FirstOrDefault().ProcessId;

            var ReceiveProductBomDetail = (from L in db.StockLine
                        join Bd in db.BomDetail on L.ProductId equals Bd.BaseProductId into BomDetailTable
                        from BomDetailTab in BomDetailTable.DefaultIfEmpty()
                        join P in db.Product on BomDetailTab.ProductId equals P.ProductId into ProductTable from ProductTab in ProductTable.DefaultIfEmpty()
                        join D1 in db.Dimension1 on BomDetailTab.ProductId equals D1.Dimension1Id into Dimension1Table
                        from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
                        join D2 in db.Dimension2 on BomDetailTab.ProductId equals D2.Dimension2Id into Dimension2Table
                        from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
                        join D3 in db.Dimension3 on BomDetailTab.ProductId equals D3.Dimension3Id into Dimension3Table
                        from Dimension3Tab in Dimension3Table.DefaultIfEmpty()
                        join D4 in db.Dimension4 on BomDetailTab.ProductId equals D4.Dimension4Id into Dimension4Table
                        from Dimension4Tab in Dimension4Table.DefaultIfEmpty()
                        where L.StockHeaderId == StockHeaderId && L.DocNature == StockNatureConstants.Receive && BomDetailTab.BomDetailId != null && BomDetailTab.ProcessId == Process_Manufacturing
                                           group new { L, BomDetailTab, ProductTab, Dimension1Tab, Dimension2Tab, Dimension3Tab, Dimension4Tab } by new { BomDetailTab.ProductId, BomDetailTab.Dimension1Id, BomDetailTab.Dimension2Id, BomDetailTab.Dimension3Id, BomDetailTab.Dimension4Id } into Results
                        select new StockExchangeLineViewModel
                        {
                            Dimension1Name = Results.Max(i => i.Dimension1Tab.Dimension1Name),
                            Dimension2Name = Results.Max(i => i.Dimension2Tab.Dimension2Name),
                            Dimension3Name = Results.Max(i => i.Dimension3Tab.Dimension3Name),
                            Dimension4Name = Results.Max(i => i.Dimension4Tab.Dimension4Name),
                            Specification = null,
                            Dimension1Id = Results.Max(i => i.BomDetailTab.Dimension1Id),
                            Dimension2Id = Results.Max(i => i.BomDetailTab.Dimension2Id),
                            Dimension3Id = Results.Max(i => i.BomDetailTab.Dimension3Id),
                            Dimension4Id = Results.Max(i => i.BomDetailTab.Dimension4Id),
                            ProcessId = null,
                            RequisitionHeaderDocNo = null,
                            ProductName = Results.Max(i => i.ProductTab.ProductName),
                            ProductId = Results.Max(i => i.BomDetailTab.ProductId),
                            CostCenterId = null,
                            CostCenterName = null,
                            StockHeaderId = StockHeaderId,
                            RequisitionLineId = null,
                            UnitId = Results.Max(i => i.ProductTab.UnitId),
                            UnitName = Results.Max(i => i.ProductTab.Unit.UnitName),
                            UnitDecimalPlaces = Results.Max(i => i.ProductTab.Unit.DecimalPlaces),
                            QtyIss = Results.Sum(i => i.L.Qty * i.BomDetailTab.Qty),
                        }).ToList();

            List<StockExchangeLineViewModel> BomWithStockInList = new List<StockExchangeLineViewModel>();

            foreach (var ReceiveProductBomDetailItem in ReceiveProductBomDetail)
            {
                var StockInBalanceList = (from L in db.ViewStockInBalance
                                          join S in db.Stock on L.StockInId equals S.StockId into StockTable
                                          from StockTab in StockTable.DefaultIfEmpty()
                                          where L.ProductId == ReceiveProductBomDetailItem.ProductId
                                         && L.Dimension1Id == ReceiveProductBomDetailItem.Dimension1Id
                                         && L.Dimension2Id == ReceiveProductBomDetailItem.Dimension2Id
                                         && L.Dimension3Id == ReceiveProductBomDetailItem.Dimension3Id
                                         && L.Dimension4Id == ReceiveProductBomDetailItem.Dimension4Id
                                         && StockTab.GodownId == StockHeader.GodownId
                                          select L).ToList();

                foreach(var StockInBalance in StockInBalanceList)
                {
                    if (StockInBalance.BalanceQty > 0)
                    {
                        StockExchangeLineViewModel vm = new StockExchangeLineViewModel();
                        vm.Dimension1Name = ReceiveProductBomDetailItem.Dimension1Name;
                        vm.Dimension2Name = ReceiveProductBomDetailItem.Dimension2Name;
                        vm.Dimension3Name = ReceiveProductBomDetailItem.Dimension3Name;
                        vm.Dimension4Name = ReceiveProductBomDetailItem.Dimension4Name;
                        vm.Specification = ReceiveProductBomDetailItem.Specification;
                        vm.Dimension1Id = ReceiveProductBomDetailItem.Dimension1Id;
                        vm.Dimension2Id = ReceiveProductBomDetailItem.Dimension2Id;
                        vm.Dimension3Id = ReceiveProductBomDetailItem.Dimension3Id;
                        vm.Dimension4Id = ReceiveProductBomDetailItem.Dimension4Id;
                        vm.ProcessId = ReceiveProductBomDetailItem.ProcessId;
                        vm.RequisitionHeaderDocNo = ReceiveProductBomDetailItem.RequisitionHeaderDocNo;
                        vm.ProductName = ReceiveProductBomDetailItem.ProductName;
                        vm.ProductId = ReceiveProductBomDetailItem.ProductId;
                        vm.CostCenterId = ReceiveProductBomDetailItem.CostCenterId;
                        vm.CostCenterName = ReceiveProductBomDetailItem.CostCenterName;
                        vm.StockHeaderId = ReceiveProductBomDetailItem.StockHeaderId;
                        vm.RequisitionLineId = ReceiveProductBomDetailItem.RequisitionLineId;
                        vm.UnitId = ReceiveProductBomDetailItem.UnitId;
                        vm.UnitName = ReceiveProductBomDetailItem.UnitName;
                        vm.UnitDecimalPlaces = ReceiveProductBomDetailItem.UnitDecimalPlaces;

                        vm.StockInId = StockInBalance.StockInId;
                        vm.StockInNo = StockInBalance.StockInNo;
                        if (ReceiveProductBomDetailItem.QtyIss <= StockInBalance.BalanceQty)
                        {
                            vm.QtyIss = ReceiveProductBomDetailItem.QtyIss;
                        }
                        else
                        {
                            vm.QtyIss = StockInBalance.BalanceQty;
                        }
                        StockInBalance.BalanceQty = StockInBalance.BalanceQty - vm.QtyIss;
                        ReceiveProductBomDetailItem.QtyIss = ReceiveProductBomDetailItem.QtyIss - vm.QtyIss;

                        if (StockInBalance.StockInId != null)
                        { 
                            vm.FromProcessId = db.Stock.Find(StockInBalance.StockInId).ProcessId;

                            if (vm.FromProcessId != null)
                                vm.FromProcessName = db.Process.Find(vm.FromProcessId).ProcessName;
                        }

                        BomWithStockInList.Add(vm);
                    }
                }
            }




            //return BomWithStockInList;
            return BomWithStockInList.Where(i => i.QtyIss > 0);
        }


        public IEnumerable<StockReceiveLineViewModel> GetRequisitionsForReceive(RequisitionFiltersForReceive vm)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] SaleOrderIdArr = null;
            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId)) { SaleOrderIdArr = vm.RequisitionHeaderId.Split(",".ToCharArray()); }
            else { SaleOrderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] Dim3IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension3Id)) { Dim3IdArr = vm.Dimension3Id.Split(",".ToCharArray()); }
            else { Dim3IdArr = new string[] { "NA" }; }

            string[] Dim4IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension4Id)) { Dim4IdArr = vm.Dimension4Id.Split(",".ToCharArray()); }
            else { Dim4IdArr = new string[] { "NA" }; }

            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            //var temp = (from p in db.RequisitionLine
            //            join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
            //            from tab in table.DefaultIfEmpty()
            //            join t2 in db.StockLine on p.RequisitionLineId equals t2.RequisitionLineId into sltable
            //            from stockline in sltable.DefaultIfEmpty()
            //            join t in db.RequisitionLineStatus on p.RequisitionLineId equals t.RequisitionLineId into reqtable
            //            from reqtab in reqtable.DefaultIfEmpty()
            //            where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : (stockline == null ? ProductIdArr.Contains(p.ProductId.ToString()) : ProductIdArr.Contains(stockline.ProductId.ToString())))
            //            && (string.IsNullOrEmpty(vm.RequisitionHeaderId) ? 1 == 1 : SaleOrderIdArr.Contains(p.RequisitionHeaderId.ToString()))
            //            && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(p.Product.ProductGroupId.ToString()))
            //            && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : (stockline == null ? Dim1IdArr.Contains(p.Dimension1Id.ToString()) : Dim1IdArr.Contains(stockline.Dimension1Id.ToString())))
            //            && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : (stockline == null ? Dim2IdArr.Contains(p.Dimension2Id.ToString()) : Dim2IdArr.Contains(stockline.Dimension2Id.ToString())))
            //            && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(tab.CostCenterId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.Product.ProductGroup.ProductTypeId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(tab.SiteId.ToString()))
            //            && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(tab.DivisionId.ToString()))
            //            where tab.PersonId == vm.PersonId && (reqtab.IssueQty ?? 0 - reqtab.ReceiveQty ?? 0) > 0 && stockline.DocNature == StockNatureConstants.Issue
            //            group new { stockline, p, tab, reqtab } by new { stockline.RequisitionLineId, stockline.ProductId, stockline.Dimension1Id, stockline.Dimension2Id } into g
            //            select new StockReceiveLineViewModel
            //            {
            //                Dimension1Name = (g.Select(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension1.Dimension1Name) : g.Max(m => m.p.Dimension1.Dimension1Name)),
            //                Dimension2Name = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension2.Dimension2Name) : g.Max(m => m.p.Dimension2.Dimension2Name)),
            //                Specification = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Specification) : g.Max(m => m.p.Specification)),
            //                Dimension1Id = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension1Id) : g.Max(m => m.p.Dimension1Id)),
            //                Dimension2Id = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Dimension2Id) : g.Max(m => m.p.Dimension2Id)),
            //                ProcessId = g.Max(m => m.p.ProcessId),
            //                RequisitionHeaderDocNo = g.Max(m => m.tab.DocNo),
            //                ProductName = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.Product.ProductName) : g.Max(m => m.p.Product.ProductName)),
            //                ProductId = (g.Where(m => m.stockline != null).Any() ? g.Max(m => m.stockline.ProductId) : g.Max(m => m.p.ProductId)),
            //                CostCenterId = g.Max(m => m.tab.CostCenterId),
            //                CostCenterName = g.Max(m => m.tab.CostCenter.CostCenterName),
            //                StockHeaderId = vm.StockHeaderId,
            //                RequisitionLineId = g.Max(m => m.p.RequisitionLineId),
            //                UnitId = g.Max(m => m.p.Product.UnitId),
            //                UnitName = g.Max(m => m.p.Product.Unit.UnitName),
            //                UnitDecimalPlaces = g.Max(m => m.p.Product.Unit.DecimalPlaces),
            //                RequisitionBalanceQty = g.Where(m => m.stockline != null).Any() ? g.Sum(m => m.stockline.Qty) : 0,
            //            }
            //           ).ToList();




            var temp2 = (from p in db.RequisitionLine
                         join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId into table
                         from tab in table.DefaultIfEmpty()
                         join t2 in db.StockLine on p.RequisitionLineId equals t2.RequisitionLineId into sltable
                         from stockline in sltable.DefaultIfEmpty()
                         join t in db.RequisitionLineStatus on p.RequisitionLineId equals t.RequisitionLineId into reqtable
                         from reqtab in reqtable.DefaultIfEmpty()
                         join d1 in db.Dimension1 on stockline.Dimension1Id equals d1.Dimension1Id
                         into dtable2
                         from dim1 in dtable2.DefaultIfEmpty()
                         join d2 in db.Dimension2 on stockline.Dimension2Id equals d2.Dimension2Id
                         into dtable
                         from dim2 in dtable.DefaultIfEmpty()
                         join prod in db.Product on stockline.ProductId equals prod.ProductId
                         join unit in db.Units on prod.UnitId equals unit.UnitId
                         join pg in db.ProductGroups on prod.ProductGroupId equals pg.ProductGroupId
                         join cs in db.CostCenter on tab.CostCenterId equals cs.CostCenterId into cstable
                         from cstab in cstable.DefaultIfEmpty()
                         where tab.PersonId == vm.PersonId && (reqtab.IssueQty ?? 0 - reqtab.ReceiveQty ?? 0) > 0 && stockline.DocNature == StockNatureConstants.Issue
                         select new
                         {
                             Dimension1Name = dim1.Dimension1Name,
                             //Dimension1Name = p.Dimension1.Dimension1Name,
                             Dimension2Name = dim2.Dimension2Name,
                             //Dimension2Name = p.Dimension2.Dimension2Name,
                             Specification = stockline.Specification,
                             //Specification = p.Specification,
                             Dimension1Id = stockline.Dimension1Id,
                             //Dimension1Id = p.Dimension1Id,
                             Dimension2Id = stockline.Dimension2Id,
                             //Dimension2Id = p.Dimension2Id,
                             ProcessId = p.ProcessId,
                             RequisitionHeaderDocNo = tab.DocNo,
                             ProductName = prod.ProductName,
                             //ProductName = p.Product.ProductName,
                             ProductId = stockline.ProductId,
                             //ProductId = p.ProductId,
                             CostCenterId = tab.CostCenterId,
                             CostCenterName = cstab.CostCenterName,
                             StockHeaderId = vm.StockHeaderId,
                             RequisitionLineId = stockline.RequisitionLineId,
                             UnitId = prod.UnitId,
                             UnitName = unit.UnitName,
                             UnitDecimalPlaces = unit.DecimalPlaces,
                             RequisitionBalanceQty = stockline.Qty,
                             RequisitionHeaderId = p.RequisitionHeaderId,
                             ProductGroupId = prod.ProductGroupId,
                             ProductTypeId = pg.ProductTypeId,
                             SiteId = tab.SiteId,
                             DivisionId = tab.DivisionId,
                         });

            if (!string.IsNullOrEmpty(vm.ProductId))
                temp2 = temp2.Where(m => ProductIdArr.Contains(m.ProductId.ToString()));

            if (!string.IsNullOrEmpty(vm.RequisitionHeaderId))
                temp2 = temp2.Where(m => SaleOrderIdArr.Contains(m.RequisitionHeaderId.ToString()));

            if (!string.IsNullOrEmpty(vm.ProductGroupId))
                temp2 = temp2.Where(m => ProductGroupIdArr.Contains(m.ProductGroupId.ToString()));

            if (!string.IsNullOrEmpty(vm.Dimension1Id))
                temp2 = temp2.Where(m => Dim1IdArr.Contains(m.Dimension1Id.ToString()));

            if (!string.IsNullOrEmpty(vm.Dimension2Id))
                temp2 = temp2.Where(m => Dim2IdArr.Contains(m.Dimension2Id.ToString()));

            if (!string.IsNullOrEmpty(vm.CostCenterId))
                temp2 = temp2.Where(m => CostCenterArr.Contains(m.CostCenterId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterProductTypes))
                temp2 = temp2.Where(m => ProductTypes.Contains(m.ProductTypeId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterContraSites))
                temp2 = temp2.Where(m => ContraSites.Contains(m.SiteId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterContraDivisions))
                temp2 = temp2.Where(m => ContraDivisions.Contains(m.DivisionId.ToString()));


            var GRecords = from p in temp2
                           group p by new { p.RequisitionLineId, p.ProductId, p.Dimension1Id, p.Dimension2Id } into g
                           select new StockReceiveLineViewModel
                           {
                               Dimension1Name = g.Max(m => m.Dimension1Name),
                               Dimension2Name = g.Max(m => m.Dimension2Name),
                               Specification = g.Max(m => m.Specification),
                               Dimension1Id = g.Max(m => m.Dimension1Id),
                               Dimension2Id = g.Max(m => m.Dimension2Id),
                               ProcessId = g.Max(m => m.ProcessId),
                               RequisitionHeaderDocNo = g.Max(m => m.RequisitionHeaderDocNo),
                               ProductName = g.Max(m => m.ProductName),
                               ProductId = g.Max(m => m.ProductId),
                               CostCenterId = g.Max(m => m.CostCenterId),
                               CostCenterName = g.Max(m => m.CostCenterName),
                               StockHeaderId = vm.StockHeaderId,
                               RequisitionLineId = g.Max(m => m.RequisitionLineId),
                               UnitId = g.Max(m => m.UnitId),
                               UnitName = g.Max(m => m.UnitName),
                               UnitDecimalPlaces = g.Max(m => m.UnitDecimalPlaces),
                               RequisitionBalanceQty = g.Sum(m => m.RequisitionBalanceQty),
                           };

            return GRecords.ToList();
        }


        public IEnumerable<StockReceiveLineViewModel> GetStockProcessForReceive(StockProcessFiltersForReceive vm)
        {
            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            //string[] StockProcessInHeaderIdArr = null;
            //if (!string.IsNullOrEmpty(vm.StockProcessInHeaderId)) { StockProcessInHeaderIdArr = vm.StockProcessInHeaderId.Split(",".ToCharArray()); }
            //else { StockProcessInHeaderIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] Dim3IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension3Id)) { Dim3IdArr = vm.Dimension3Id.Split(",".ToCharArray()); }
            else { Dim3IdArr = new string[] { "NA" }; }

            string[] Dim4IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension4Id)) { Dim4IdArr = vm.Dimension4Id.Split(",".ToCharArray()); }
            else { Dim4IdArr = new string[] { "NA" }; }


            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            var temp2 = (from L in db.StockProcess
                         where L.StockHeader.PersonId == vm.PersonId 
                         select new
                         {
                             Dimension1Name = L.Dimension1.Dimension1Name,
                             Dimension2Name = L.Dimension2.Dimension2Name,
                             Dimension3Name = L.Dimension3.Dimension3Name,
                             Dimension4Name = L.Dimension4.Dimension4Name,
                             Specification = L.Specification,
                             Dimension1Id = L.Dimension1Id,
                             Dimension2Id = L.Dimension2Id,
                             Dimension3Id = L.Dimension3Id,
                             Dimension4Id = L.Dimension4Id,
                             ProcessId = L.ProcessId,
                             ProductName = L.Product.ProductName,
                             ProductId = L.ProductId,
                             CostCenterId = L.CostCenterId,
                             CostCenterName = L.CostCenter.CostCenterName,
                             StockHeaderId = vm.StockHeaderId,
                             UnitId = L.Product.UnitId,
                             UnitName = L.Product.Unit.UnitName,
                             UnitDecimalPlaces = L.Product.Unit.DecimalPlaces,
                             ProductGroupId = L.Product.ProductGroupId,
                             ProductTypeId = L.Product.ProductGroup.ProductTypeId,
                             SiteId = L.StockHeader.SiteId,
                             DivisionId = L.StockHeader.DivisionId,
                             Qty_Iss = L.Qty_Iss,
                             Qty_Rec = L.Qty_Rec,
                             LotNo=L.LotNo,
                         });

            if (!string.IsNullOrEmpty(vm.ProductId))
                temp2 = temp2.Where(m => ProductIdArr.Contains(m.ProductId.ToString()));

            if (!string.IsNullOrEmpty(vm.ProductGroupId))
                temp2 = temp2.Where(m => ProductGroupIdArr.Contains(m.ProductGroupId.ToString()));

            if (!string.IsNullOrEmpty(vm.Dimension1Id))
                temp2 = temp2.Where(m => Dim1IdArr.Contains(m.Dimension1Id.ToString()));

            if (!string.IsNullOrEmpty(vm.Dimension2Id))
                temp2 = temp2.Where(m => Dim2IdArr.Contains(m.Dimension2Id.ToString()));

            if (!string.IsNullOrEmpty(vm.Dimension3Id))
                temp2 = temp2.Where(m => Dim3IdArr.Contains(m.Dimension3Id.ToString()));

            if (!string.IsNullOrEmpty(vm.Dimension4Id))
                temp2 = temp2.Where(m => Dim4IdArr.Contains(m.Dimension4Id.ToString()));

            if (!string.IsNullOrEmpty(vm.CostCenterId))
                temp2 = temp2.Where(m => CostCenterArr.Contains(m.CostCenterId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterProductTypes))
                temp2 = temp2.Where(m => ProductTypes.Contains(m.ProductTypeId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterContraSites))
                temp2 = temp2.Where(m => ContraSites.Contains(m.SiteId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterContraDivisions))
                temp2 = temp2.Where(m => ContraDivisions.Contains(m.DivisionId.ToString()));


            var GRecords = from p in temp2
                           group p by new { p.ProductId, p.Dimension1Id, p.Dimension2Id, p.Dimension3Id, p.Dimension4Id,p.LotNo,p.ProcessId } into g
                           where g.Sum(m => m.Qty_Rec) - g.Sum(m => m.Qty_Iss) > 0
                           select new StockReceiveLineViewModel
                           {
                               Dimension1Name = g.Max(m => m.Dimension1Name),
                               Dimension2Name = g.Max(m => m.Dimension2Name),
                               Dimension3Name = g.Max(m => m.Dimension3Name),
                               Dimension4Name = g.Max(m => m.Dimension4Name),
                               Specification = g.Max(m => m.Specification),
                               Dimension1Id = g.Max(m => m.Dimension1Id),
                               Dimension2Id = g.Max(m => m.Dimension2Id),
                               Dimension3Id = g.Max(m => m.Dimension3Id),
                               Dimension4Id = g.Max(m => m.Dimension4Id),
                               ProcessId = g.Max(m => m.ProcessId),
                               ProductName = g.Max(m => m.ProductName),
                               ProductId = g.Max(m => m.ProductId),
                               CostCenterId = g.Max(m => m.CostCenterId),
                               CostCenterName = g.Max(m => m.CostCenterName),
                               StockHeaderId = vm.StockHeaderId,
                               UnitId = g.Max(m => m.UnitId),
                               UnitName = g.Max(m => m.UnitName),
                               UnitDecimalPlaces = g.Max(m => m.UnitDecimalPlaces),
                               RequisitionBalanceQty = g.Sum(m => m.Qty_Rec) - g.Sum(m => m.Qty_Iss),
                               LotNo=g.Max(m=>m.LotNo),
                           };

            return GRecords.ToList();
        }


        public decimal GetExcessStock(int ProductId, int? Dim1, int? Dim2, int? ProcId, string Lot, int MaterilIssueId, string ProcName)
        {
            StockHeader Header = new StockHeaderService(_unitOfWork).Find(MaterilIssueId);
            decimal EXS = 0;

            using (SqlConnection sqlConnection = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]))
            {
                sqlConnection.Open();

                SqlCommand Totalf = new SqlCommand("SELECT " + (string.IsNullOrEmpty(ProcName) ? "Web.FStockBalance" : ProcName) + "( " + ProductId + ", " + (!Dim1.HasValue ? "NULL" : "" + Dim1 + "") + ", " + (!Dim2.HasValue ? "NULL" : "" + Dim2 + "") + ", " + (!ProcId.HasValue ? "NULL" : "" + ProcId + "") + ", " + (string.IsNullOrEmpty(Lot) ? "NULL" : Lot) + ", " + Header.SiteId + ", NULL" + ", " + (!Header.GodownId.HasValue ? "NULL" : "" + Header.GodownId + "") + ")", sqlConnection);

                EXS = Convert.ToDecimal(Totalf.ExecuteScalar() == DBNull.Value ? 0 : Totalf.ExecuteScalar());
            }

            return EXS;
        }

        public decimal GetExcessStockForTransfer(int ProductId, int? Dim1, int? Dim2, int? ProcId, string Lot, int MaterilIssueId, string ProcName)
        {
            StockHeader Header = new StockHeaderService(_unitOfWork).Find(MaterilIssueId);
            decimal EXS = 0;

            using (SqlConnection sqlConnection = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]))
            {
                sqlConnection.Open();

                SqlCommand Totalf = new SqlCommand("SELECT " + (string.IsNullOrEmpty(ProcName) ? "Web.FStockBalance" : ProcName) + "( " + ProductId + ", " + (!Dim1.HasValue ? "NULL" : "" + Dim1 + "") + ", " + (!Dim2.HasValue ? "NULL" : "" + Dim2 + "") + ", " + (!ProcId.HasValue ? "NULL" : "" + ProcId + "") + ", " + (string.IsNullOrEmpty(Lot) ? "NULL" : Lot) + ", " + Header.SiteId + ", NULL" + ", " + (!Header.GodownId.HasValue ? "NULL" : "" + Header.FromGodownId + "") + ")", sqlConnection);

                EXS = Convert.ToDecimal(Totalf.ExecuteScalar() == DBNull.Value ? 0 : Totalf.ExecuteScalar());
            }

            return EXS;
        }

        public IQueryable<ComboBoxResult> GetCostCentersForIssueFilters(int id, string term)
        {

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            return (from p in db.ViewRequisitionBalance
                    where p.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenter.CostCenterName.ToLower().Contains(term.ToLower()))
                    && p.SiteId == SiteId && p.DivisionId == DivisionId && p.CostCenter.IsActive == true && p.CostCenter.Status != (int)StatusConstants.Closed
                    group p by p.CostCenterId into g
                    orderby g.Max(m => m.CostCenter.CostCenterName)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.CostCenter.CostCenterName),
                        id = g.Key.Value.ToString(),
                    });

        }

        public IQueryable<ComboBoxResult> GetProductHelpList(int id, string term)
        {

            var StockHeader = db.StockHeader.Find(id);

            var StockHeaderSettigns = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProducts)) { ProductIdArr = StockHeaderSettigns.filterProducts.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProductGroups)) { ProductGroupIdArr = StockHeaderSettigns.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] ProductTypeIdArr = null;
            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProductTypes)) { ProductTypeIdArr = StockHeaderSettigns.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypeIdArr = new string[] { "NA" }; }

            string[] ProductDivisionArr = null;
            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterContraProductDivisions)) { ProductDivisionArr = StockHeaderSettigns.filterContraProductDivisions.Split(",".ToCharArray()); }
            else { ProductDivisionArr = new string[] { "NA" }; }

            var Query = (from p in db.Product
                         join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId
                         select new
                         {
                             DivisionId = p.DivisionId,
                             ProductId = p.ProductId,
                             ProductTypeId = t.ProductTypeId,
                             ProductName = p.ProductName,
                             ProductGroupId = p.ProductGroupId,
                         });

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProductTypes))
                Query = Query.Where(m => ProductTypeIdArr.Contains(m.ProductTypeId.ToString()));

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterContraProductDivisions))
                Query = Query.Where(m => ProductDivisionArr.Contains(m.DivisionId.ToString()));

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProductGroups))
                Query = Query.Where(m => ProductGroupIdArr.Contains(m.ProductGroupId.ToString()));

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProducts))
                Query = Query.Where(m => ProductIdArr.Contains(m.ProductId.ToString()));

            if (!string.IsNullOrEmpty(term))
                Query = Query.Where(m => m.ProductName.ToLower().Contains(term.ToLower()));

            var UQuery = (from pa in db.ProductAlias
                          join p in db.Product on pa.ProductId equals p.ProductId
                          join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId
                          select new
                          {
                              DivisionId = p.DivisionId,
                              ProductId = p.ProductId,
                              ProductTypeId = t.ProductTypeId,
                              ProductName = pa.ProductAliasName,
                              ProductGroupId = p.ProductGroupId,
                          });

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProductTypes))
                UQuery = UQuery.Where(m => ProductTypeIdArr.Contains(m.ProductTypeId.ToString()));

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterContraProductDivisions))
                UQuery = UQuery.Where(m => ProductDivisionArr.Contains(m.DivisionId.ToString()));

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProductGroups))
                UQuery = UQuery.Where(m => ProductGroupIdArr.Contains(m.ProductGroupId.ToString()));

            if (!string.IsNullOrEmpty(StockHeaderSettigns.filterProducts))
                UQuery = UQuery.Where(m => ProductIdArr.Contains(m.ProductId.ToString()));

            if (!string.IsNullOrEmpty(term))
                UQuery = UQuery.Where(m => m.ProductName.ToLower().Contains(term.ToLower()));

            var union = (from p in Query
                         select new ComboBoxResult
                         {
                             id = p.ProductId.ToString(),
                             text = p.ProductName,
                         }).Union(from p in UQuery
                                  select new ComboBoxResult
                                  {
                                      id = p.ProductId.ToString(),
                                      text = p.ProductName
                                  });

            return (from p in union
                    orderby p.text
                    select p);

            //return Query.Union(UQuery).Select(m => new ComboBoxResult { id = m.ProductId.ToString(), text = m.ProductName }).OrderBy(m=>m.text);



        }

        public IQueryable<ComboBoxResult> GetCostCentersForLine(int id, string term)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(id);

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            //return (from p in db.CostCenter
            //        where p.LedgerAccount.PersonId == StockHeader.PersonId && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
            //        && p.ProcessId == StockHeader.ProcessId
            //        && p.SiteId == SiteId && p.DivisionId == DivisionId && p.IsActive == true
            //        group p by p.CostCenterId into g
            //        orderby g.Max(m => m.CostCenterName)
            //        select new ComboBoxResult
            //        {
            //            text = g.Max(m => m.CostCenterName),
            //            id = g.Key.ToString(),
            //        });

            var temp = from p in db.CostCenter
                       where p.ProcessId == StockHeader.ProcessId
                   && (p.LedgerAccount.PersonId == StockHeader.PersonId || p.ReferenceDocNo == "Weaving Defaulter")
                   && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                   && p.SiteId == SiteId && p.DivisionId == DivisionId && p.IsActive == true && p.Status != (int)StatusConstants.Closed
                       group p by p.CostCenterId into g
                       orderby g.Max(m => m.CostCenterName)
                       select new ComboBoxResult
                       {
                           text = g.Max(m => m.CostCenterName),
                           id = g.Key.ToString(),
                       };

            return (from p in db.CostCenter
                    where p.ProcessId == StockHeader.ProcessId
                && (p.LedgerAccount.PersonId == StockHeader.PersonId || p.ReferenceDocNo == "Weaving Defaulter")
                && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                && p.SiteId == SiteId && p.DivisionId == DivisionId && p.IsActive == true && p.Status != (int)StatusConstants.Closed
                    group p by p.CostCenterId into g
                    orderby g.Max(m => m.CostCenterName)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.CostCenterName),
                        id = g.Key.ToString(),
                    });
        }

        public IEnumerable<StockExchangeLineViewModel> GetProcessTransfersForFilters(FiltersForProcessTransfer vm)
        {

            var Stock = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Stock.DocTypeId, Stock.DivisionId, Stock.SiteId);


            //string[] ContraSites = null;
            //if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            //else { ContraSites = new string[] { "NA" }; }

            //string[] ContraDivisions = null;
            //if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            //else { ContraDivisions = new string[] { "NA" }; }


            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dimension1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dimension1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dimension1IdArr = new string[] { "NA" }; }

            string[] Dimension2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dimension2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dimension2IdArr = new string[] { "NA" }; }

            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", vm.ProcessId);
            SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vm.PersonId);
            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", vm.FromCostCenterId);
            SqlParameter SqlParameterUptoDate = new SqlParameter("@UptoDate", DateTime.Now.Date);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@DivisionId", System.Web.HttpContext.Current.Session["DivisionId"]);
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", System.Web.HttpContext.Current.Session["SiteId"]);

            IEnumerable<StockLineViewModel> StockProcessBalance = db.Database.SqlQuery<StockLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spStockProcessBalance  @UptoDate,@SiteId,@DivisionId,@PersonId,@ProcessId,@CostCenterId", SqlParameterUptoDate, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterPersonId, SqlParameterProcessId, SqlParameterCostCenterId).ToList();

            var temp = from H in StockProcessBalance
                       join Proc in db.Process on H.ProcessId equals Proc.ProcessId into table
                       from tab in table.DefaultIfEmpty()
                       where (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(H.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(H.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dimension1IdArr.Contains(H.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dimension2IdArr.Contains(H.Dimension2Id.ToString()))
                        && H.BalanceQty != 0
                       select new StockExchangeLineViewModel
                       {
                           StockHeaderId = vm.StockHeaderId,
                           Dimension1Name = H.Dimension1Name,
                           Dimension1Id = H.Dimension1Id,
                           Dimension2Id = H.Dimension2Id,
                           Dimension2Name = H.Dimension2Name,
                           Specification = H.Specification,
                           BalanceQty = H.BalanceQty,
                           Qty = H.BalanceQty,
                           UnitName = H.UnitName,
                           UnitDecimalPlaces = H.UnitDecimalPlaces,
                           //UnitId=P.UnitId,
                           ProductName = H.ProductName,
                           ProductId = H.ProductId,
                           ProcessId = (tab == null ? null : (int?)tab.ProcessId),
                           ProcessName = (tab == null ? null : tab.ProcessName),
                           CostCenterId = H.CostCenterId,
                           ToCostCenterId = vm.ToCostCenterId,
                           CostCenterName = H.CostCenterName,
                           LotNo = H.LotNo
                       };

            return temp;


        }

        public IQueryable<ComboBoxResult> GetCostCentersForProcessTransfer(int SiteId, int DivisionId, string FilterDocTypes, string term, int? PersonId)
        {

            //var Header = new StockHeaderService(_unitOfWork).Find(id);

            //var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(Header.DocTypeId, Header.DivisionId, Header.SiteId);


            string[] ContraDocTypes = null;
            if (!string.IsNullOrEmpty(FilterDocTypes)) { ContraDocTypes = FilterDocTypes.Split(",".ToCharArray()); }
            else { ContraDocTypes = new string[] { "NA" }; }

            //var list = (from p in db.CostCenter
            //            join t in db.LedgerAccount on p.LedgerAccountId equals t.LedgerAccountId
            //            where (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
            //              && (string.IsNullOrEmpty(FilterDocTypes) ? 1 == 1 : ContraDocTypes.Contains(p.DocTypeId.ToString()))
            //              && p.SiteId == SiteId && p.DivisionId == DivisionId 
            //              && ( PersonId.HasValue ? t.PersonId == PersonId : 1==1 )
            //              && p.Status != (int)StatusConstants.Closed
            //            orderby p.CostCenterName
            //            select new ComboBoxResult
            //            {
            //                id = p.CostCenterId.ToString(),
            //                text = p.CostCenterName,
            //            }
            //              );


            var list = (from p in db.CostCenter
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenterName.ToLower().Contains(term.ToLower()))
                          && (string.IsNullOrEmpty(FilterDocTypes) ? 1 == 1 : ContraDocTypes.Contains(p.DocTypeId.ToString()))
                          && p.SiteId == SiteId && p.DivisionId == DivisionId
                          && p.Status != (int)StatusConstants.Closed
                        orderby p.CostCenterName
                        select new ComboBoxResult
                        {
                            id = p.CostCenterId.ToString(),
                            text = p.CostCenterName,
                        }
              );


            return list;
        }

        public IQueryable<ComboBoxResult> GetCustomProducts(int Id, string term)//DocTypeId
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            return (from p in db.Product
                    where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.ProductGroup.ProductTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(p.ProductId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(p.ProductGroupId.ToString()))
                    && p.ProductName.ToLower().Contains(term.ToLower())
                    group new { p } by p.ProductId into g
                    orderby g.Key
                    select new ComboBoxResult
                    {
                        id = g.Key.ToString(),
                        text = g.Max(m => m.p.ProductName),
                    }
                        );
        }

        public IQueryable<ComboBoxResult> GetRequisitionsForReceive(int Id, string term)//DocTypeId
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            return (from p in db.RequisitionLineStatus
                    join t in db.RequisitionLine on p.RequisitionLineId equals t.RequisitionLineId
                    join t2 in db.RequisitionHeader on t.RequisitionHeaderId equals t2.RequisitionHeaderId
                    where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(t.Product.ProductGroup.ProductTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(t.ProductId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? t2.SiteId == SiteId : ContraSites.Contains(t2.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? t2.DivisionId == DivisionId : ContraDivisions.Contains(t2.DivisionId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(t.Product.ProductGroupId.ToString()))
                    && t2.DocNo.ToLower().Contains(term.ToLower()) && t2.PersonId == StockHeader.PersonId
                    && (p.IssueQty ?? 0 - p.ReceiveQty ?? 0) > 0
                    group new { t2 } by t2.RequisitionHeaderId into g
                    orderby g.Key
                    select new ComboBoxResult
                    {
                        id = g.Key.ToString(),
                        text = g.Max(m => m.t2.DocNo),
                    }
                        );
        }

        public int GetMaxSr(int id)
        {
            var Max = (from p in db.StockLine
                       where p.StockHeaderId == id
                       select p.Sr
                        );

            if (Max.Count() > 0)
                return Max.Max(m => m ?? 0) + 1;
            else
                return (1);
        }

        public UIDValidationViewModel ValidateBarCodeOnStock(string ProductUidName, int StockHeader)
        {

            UIDValidationViewModel temp = new UIDValidationViewModel();

            var UID = (from p in db.ProductUid
                       where p.ProductUidName == ProductUidName
                       join t in db.StockLine.Where(m => m.StockHeaderId == StockHeader) on p.ProductUIDId equals t.ProductUidId into linetab
                       from sl in linetab.DefaultIfEmpty()
                       select new UIDValidationViewModel
                       {
                           CurrenctGodownId = p.CurrenctGodownId,
                           CurrenctProcessId = p.CurrenctProcessId,
                           CurrentGodownName = p.CurrenctGodown.GodownName,
                           CurrentProcessName = p.CurrenctProcess.ProcessName,
                           GenDocDate = p.GenDocDate,
                           GenDocId = p.GenDocId,
                           GenDocNo = p.GenDocNo,
                           GenDocTypeId = p.GenDocTypeId,
                           GenDocTypeName = p.GenDocType.DocumentTypeName,
                           GenPersonId = p.GenPersonId,
                           GenPersonName = p.GenPerson.Person.Name,
                           IsActive = p.IsActive,
                           LastTransactionDocDate = p.LastTransactionDocDate,
                           LastTransactionDocId = p.LastTransactionDocId,
                           LastTransactionDocNo = p.LastTransactionDocNo,
                           LastTransactionDocTypeId = p.LastTransactionDocTypeId,
                           LastTransactionDocTypeName = p.LastTransactionDocType.DocumentTypeName,
                           LastTransactionPersonId = p.LastTransactionPersonId,
                           LastTransactionPersonName = p.LastTransactionPerson.Name,
                           ProductId = p.ProductId,
                           ProductName = p.Product.ProductName,
                           ProductUIDId = p.ProductUIDId,
                           ProductUidName = p.ProductUidName,
                           Status = p.Status,
                           LotNo = p.LotNo,
                           Dimension1Id = p.Dimension1Id,
                           Dimension1Name = p.Dimension1.Dimension1Name,
                           Dimension2Id = p.Dimension2Id,
                           Dimension2Name = p.Dimension2.Dimension2Name,
                           Validation = sl == null,
                       }).FirstOrDefault();

            if (UID == null)
            {
                UID = new UIDValidationViewModel();
                UID.ErrorType = "InvalidID";
                UID.ErrorMessage = "Invalid ProductUID";
            }
            else if (!UID.Validation)
            {
                UID.ErrorType = "Status";
                UID.ErrorMessage = "BarCode is already added.";
            }
            else if (UID.Status != "Receive" && UID.Status != "Gen")
            {
                UID.ErrorType = "Status";
                UID.ErrorMessage = "The Status of the Barcode is " + UID.Status;
            }
            else
            {
                UID.ErrorType = "Success";
            }

            return UID;


        }

        public UIDValidationViewModel ValidateBarCodeOnStockReceive(string ProductUidName, int StockHeader)
        {

            UIDValidationViewModel temp = new UIDValidationViewModel();

            var UID = (from p in db.ProductUid
                       where p.ProductUidName == ProductUidName
                       join t in db.StockLine.Where(m => m.StockHeaderId == StockHeader) on p.ProductUIDId equals t.ProductUidId into linetab
                       from sl in linetab.DefaultIfEmpty()
                       select new UIDValidationViewModel
                       {
                           CurrenctGodownId = p.CurrenctGodownId,
                           CurrenctProcessId = p.CurrenctProcessId,
                           CurrentGodownName = p.CurrenctGodown.GodownName,
                           CurrentProcessName = p.CurrenctProcess.ProcessName,
                           GenDocDate = p.GenDocDate,
                           GenDocId = p.GenDocId,
                           GenDocNo = p.GenDocNo,
                           GenDocTypeId = p.GenDocTypeId,
                           GenDocTypeName = p.GenDocType.DocumentTypeName,
                           GenPersonId = p.GenPersonId,
                           GenPersonName = p.GenPerson.Person.Name,
                           IsActive = p.IsActive,
                           LastTransactionDocDate = p.LastTransactionDocDate,
                           LastTransactionDocId = p.LastTransactionDocId,
                           LastTransactionDocNo = p.LastTransactionDocNo,
                           LastTransactionDocTypeId = p.LastTransactionDocTypeId,
                           LastTransactionDocTypeName = p.LastTransactionDocType.DocumentTypeName,
                           LastTransactionPersonId = p.LastTransactionPersonId,
                           LastTransactionPersonName = p.LastTransactionPerson.Name,
                           ProductId = p.ProductId,
                           ProductName = p.Product.ProductName,
                           ProductUIDId = p.ProductUIDId,
                           ProductUidName = p.ProductUidName,
                           Status = p.Status,
                           LotNo = p.LotNo,
                           Dimension1Id = p.Dimension1Id,
                           Dimension1Name = p.Dimension1.Dimension1Name,
                           Dimension2Id = p.Dimension2Id,
                           Dimension2Name = p.Dimension2.Dimension2Name,
                           Validation = sl == null,
                       }).FirstOrDefault();

            if (UID == null)
            {
                UID = new UIDValidationViewModel();
                UID.ErrorType = "InvalidID";
                UID.ErrorMessage = "Invalid ProductUID";
            }
            else if (!UID.Validation)
            {
                UID.ErrorType = "Status";
                UID.ErrorMessage = "BarCode is already added.";
            }
            else
            {
                UID.ErrorType = "Success";
            }

            return UID;
        }



        public IQueryable<ComboBoxResult> GetCostCentersForReceive(int id, string term)
        {

            return (from p in db.RequisitionHeader
                    where p.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.CostCenter.CostCenterName.ToLower().Contains(term.ToLower())) && p.CostCenter.IsActive == true && p.Status != (int)StatusConstants.Closed
                    group p by p.CostCenterId into g
                    orderby g.Max(m => m.CostCenter.CostCenterName)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.CostCenter.CostCenterName),
                        id = g.Key.Value.ToString(),
                    });

        }

        public IQueryable<ComboBoxResult> GetProductsForReceiveFilters(int id, string term)
        {

            var StockHeader = db.StockHeader.Find(id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            if (settings.isProductHelpFromStockProcess == true)
            {
                var Query = from p in db.StockProcess
                            where p.ProcessId == StockHeader.ProcessId && p.StockHeader.PersonId == StockHeader.PersonId
                            join t in db.Product on p.ProductId equals t.ProductId
                            join t2 in db.ProductGroups on t.ProductGroupId equals t2.ProductGroupId
                            into pgtable
                            from pg in pgtable.DefaultIfEmpty()
                            join t3 in db.ProductTypes on pg.ProductTypeId equals t3.ProductTypeId
                            into pttable
                            from pt in pttable.DefaultIfEmpty()
                            select new
                            {
                                ProductId = p.ProductId,
                                ProductName = t.ProductName,
                                ProductTypeId = pt.ProductTypeId,
                                ProductGroupId = pg.ProductGroupId,
                                BalanceQty = p.Qty_Rec - p.Qty_Iss,
                            };

                if (!string.IsNullOrEmpty(settings.filterProductTypes))
                    Query = Query.Where(m => ProductTypes.Contains(m.ProductTypeId.ToString()));

                if (!string.IsNullOrEmpty(settings.filterProducts))
                    Query = Query.Where(m => Products.Contains(m.ProductId.ToString()));

                if (!string.IsNullOrEmpty(settings.filterProductGroups))
                    Query = Query.Where(m => ProductGroups.Contains(m.ProductGroupId.ToString()));

                if (!string.IsNullOrEmpty(term))
                    Query = Query.Where(m => m.ProductName.ToLower().Contains(term.ToLower()));

                return (from p in Query
                        group p by p.ProductId
                            into g
                            where g.Sum(m => m.BalanceQty) > 0
                            orderby g.Max(m => m.ProductName)
                            select new ComboBoxResult
                         {
                             text = g.Max(m => m.ProductName),
                             id = g.Key.ToString(),
                             AProp1 = "BalQty: " + g.Sum(m => m.BalanceQty).ToString(),
                         });

            }
            else
                return (from p in db.RequisitionLine
                        join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                        where t.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.Product.ProductName.ToLower().Contains(term.ToLower()))
                        group p by p.ProductId into g
                        orderby g.Max(m => m.Product.ProductName)
                        select new ComboBoxResult
                        {
                            text = g.Max(m => m.Product.ProductName),
                            id = g.Key.ToString(),
                        });

        }

        //public IQueryable<ComboBoxResult> GetProductsForReceiveLine(int id, string term)
        //{

        //    var StockHeader = db.StockHeader.Find(id);

        //    var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

        //    string[] ProductTypes = null;
        //    if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
        //    else { ProductTypes = new string[] { "NA" }; }

        //    string[] Products = null;
        //    if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
        //    else { Products = new string[] { "NA" }; }

        //    string[] ProductGroups = null;
        //    if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
        //    else { ProductGroups = new string[] { "NA" }; }

        //    if (settings.isProductHelpFromStockProcess == true)
        //    {
        //        var Query = from p in db.StockProcess
        //                    where p.StockHeader.ProcessId == StockHeader.ProcessId && p.StockHeader.PersonId == StockHeader.PersonId
        //                    join t in db.Product on p.ProductId equals t.ProductId
        //                    join t2 in db.ProductGroups on t.ProductGroupId equals t2.ProductGroupId
        //                    into pgtable
        //                    from pg in pgtable.DefaultIfEmpty()
        //                    join t3 in db.ProductTypes on pg.ProductTypeId equals t3.ProductTypeId
        //                    into pttable
        //                    from pt in pttable.DefaultIfEmpty()
        //                    select new
        //                    {
        //                        ProductId = p.ProductId,
        //                        ProductName = t.ProductName,
        //                        Dimension1Id = p.Dimension1Id,
        //                        Dimension1Name = p.Dimension1.Dimension1Name,
        //                        Dimension2Id = p.Dimension2Id,
        //                        Dimension2Name = p.Dimension2.Dimension2Name,
        //                        Dimension3Id = p.Dimension3Id,
        //                        Dimension3Name = p.Dimension3.Dimension3Name,
        //                        Dimension4Id = p.Dimension4Id,
        //                        Dimension4Name = p.Dimension4.Dimension4Name,
        //                        ProductTypeId = pt.ProductTypeId,
        //                        ProductGroupId = pg.ProductGroupId,
        //                        BalanceQty = p.Qty_Rec - p.Qty_Iss,
        //                    };

        //        if (!string.IsNullOrEmpty(settings.filterProductTypes))
        //            Query = Query.Where(m => ProductTypes.Contains(m.ProductTypeId.ToString()));

        //        if (!string.IsNullOrEmpty(settings.filterProducts))
        //            Query = Query.Where(m => Products.Contains(m.ProductId.ToString()));

        //        if (!string.IsNullOrEmpty(settings.filterProductGroups))
        //            Query = Query.Where(m => ProductGroups.Contains(m.ProductGroupId.ToString()));

        //        if (!string.IsNullOrEmpty(term))
        //            Query = Query.Where(m => m.ProductName.ToLower().Contains(term.ToLower()));

        //        return (from p in Query
        //                group p by new {p.ProductId, p.Dimension1Id, p.Dimension2Id, p.Dimension3Id, p.Dimension4Id}
        //                    into g
        //                    where g.Sum(m => m.BalanceQty) > 0
        //                    orderby g.Max(m => m.ProductName)
        //                    select new ComboBoxResult
        //                    {
        //                        text = g.Max(m => m.ProductName).ToString(),
        //                        id = g.Key.ProductId.ToString(),
        //                        AProp2 = "BalQty: " + g.Sum(m => m.BalanceQty).ToString(),
        //                        AProp1 = g.Max(m => m.Dimension1Name == null ? "" : m.Dimension1Name).ToString() + 
        //                                g.Max(m => m.Dimension2Name == null ? "" : m.Dimension2Name).ToString() + 
        //                                g.Max(m => m.Dimension3Name == null ? "" : m.Dimension3Name).ToString() + 
        //                                g.Max(m => m.Dimension4Name == null ? "" : m.Dimension4Name).ToString()
        //                    });

        //                                //        AProp1 = g.Max((m => m.Dimension1Name == null) ? "" : m.Dimension1Name) +
        //                                //((Dimension2Tab.Dimension2Name == null) ? "" : "," + Dimension2Tab.Dimension2Name) +
        //                                //((Dimension3Tab.Dimension3Name == null) ? "" : "," + Dimension3Tab.Dimension3Name) +
        //                                //((Dimension4Tab.Dimension4Name == null) ? "" : "," + Dimension4Tab.Dimension4Name)

        //    }
        //    else
        //    {
        //        var Query = from p in db.Product
        //                    join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId
        //                    orderby p.ProductName
        //                    select new
        //                    {
        //                        ProductName = p.ProductName,
        //                        ProductId = p.ProductId,
        //                        ProductTypeId = t.ProductTypeId,
        //                        ProductGroupId = p.ProductGroupId,
        //                    };


        //        if (!string.IsNullOrEmpty(settings.filterProductTypes))
        //            Query = Query.Where(m => ProductTypes.Contains(m.ProductTypeId.ToString()));

        //        if (!string.IsNullOrEmpty(settings.filterProducts))
        //            Query = Query.Where(m => Products.Contains(m.ProductId.ToString()));

        //        if (!string.IsNullOrEmpty(settings.filterProductGroups))
        //            Query = Query.Where(m => ProductGroups.Contains(m.ProductGroupId.ToString()));

        //        if (!string.IsNullOrEmpty(term))
        //            Query = Query.Where(m => m.ProductName.ToLower().Contains(term.ToLower()));

        //        return (from p in Query
        //                select new ComboBoxResult
        //                {
        //                    id = p.ProductId.ToString(),
        //                    text = p.ProductName,
        //                });
        //    }

        //}



        public IQueryable<ComboBoxResult> GetProductsForReceiveLine(int id, string term)
        {

            var StockHeader = db.StockHeader.Find(id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            var Query = from p in db.Product
                        join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId
                        orderby p.ProductName
                        select new
                        {
                            ProductName = p.ProductName,
                            ProductId = p.ProductId,
                            ProductTypeId = t.ProductTypeId,
                            ProductGroupId = p.ProductGroupId,
                        };


            if (!string.IsNullOrEmpty(settings.filterProductTypes))
                Query = Query.Where(m => ProductTypes.Contains(m.ProductTypeId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterProducts))
                Query = Query.Where(m => Products.Contains(m.ProductId.ToString()));

            if (!string.IsNullOrEmpty(settings.filterProductGroups))
                Query = Query.Where(m => ProductGroups.Contains(m.ProductGroupId.ToString()));

            if (!string.IsNullOrEmpty(term))
                Query = Query.Where(m => m.ProductName.ToLower().Contains(term.ToLower()));

            return (from p in Query
                    select new ComboBoxResult
                    {
                        id = p.ProductId.ToString(),
                        text = p.ProductName,
                    });

        }

        public IQueryable<ComboBoxResult> GetDimension1ForReceive(int id, string term)
        {

            return (from p in db.RequisitionLine
                    join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                    where t.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower()))
                    group p by p.Dimension1Id into g
                    orderby g.Max(m => m.Dimension1.Dimension1Name)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.Dimension1.Dimension1Name),
                        id = g.Key.ToString(),
                    });

        }

        public IQueryable<ComboBoxResult> GetDimension2ForReceive(int id, string term)
        {

            return (from p in db.RequisitionLine
                    join t in db.RequisitionHeader on p.RequisitionHeaderId equals t.RequisitionHeaderId
                    where t.PersonId == id && (string.IsNullOrEmpty(term) ? 1 == 1 : p.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower()))
                    group p by p.Dimension2Id into g
                    orderby g.Max(m => m.Dimension2.Dimension2Name)
                    select new ComboBoxResult
                    {
                        text = g.Max(m => m.Dimension2.Dimension2Name),
                        id = g.Key.ToString(),
                    });

        }

        public IEnumerable<ComboBoxResult> GetPendingStockInForIssue(int StockHeaderId, int? ProductId, int? Dimension1Id, int? Dimension2Id, int? Dimension3Id, int? Dimension4Id, string term)
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            int GodownId = 0;

            if (StockHeader.FromGodownId != null)
            {
                GodownId = (int)StockHeader.FromGodownId;
            }
            else if (StockHeader.GodownId != null)
            {
                GodownId = (int)StockHeader.GodownId;
            }

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);


            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            return (from p in db.ViewStockInBalance
                    join L in db.Stock on p.StockInId equals L.StockId into StockTable from StockTab in StockTable.DefaultIfEmpty()
                    join pt in db.Product on p.ProductId equals pt.ProductId into ProductTable
                    from ProductTab in ProductTable.DefaultIfEmpty()
                    join D1 in db.Dimension1 on p.Dimension1Id equals D1.Dimension1Id into Dimension1Table
                    from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
                    join D2 in db.Dimension2 on p.Dimension2Id equals D2.Dimension2Id into Dimension2Table
                    from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
                    join D3 in db.Dimension3 on p.Dimension3Id equals D3.Dimension3Id into Dimension3Table
                    from Dimension3Tab in Dimension3Table.DefaultIfEmpty()
                    join D4 in db.Dimension4 on p.Dimension4Id equals D4.Dimension4Id into Dimension4Table
                    from Dimension4Tab in Dimension4Table.DefaultIfEmpty()
                    where p.BalanceQty > 0 && StockTab.GodownId == GodownId
                    && (ProductId == null || ProductId == 0 ? 1 == 1 : p.ProductId == ProductId)
                    && (Dimension1Id == null ? 1 == 1 : p.Dimension1Id == Dimension1Id)
                    && (Dimension2Id == null ? 1 == 1 : p.Dimension2Id == Dimension2Id)
                    && (Dimension3Id == null ? 1 == 1 : p.Dimension3Id == Dimension3Id)
                    && (Dimension4Id == null ? 1 == 1 : p.Dimension4Id == Dimension4Id)
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && (string.IsNullOrEmpty(term) ? 1 == 1 : p.StockInNo.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : ProductTab.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension1Tab.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension2Tab.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension3Tab.Dimension3Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension4Tab.Dimension4Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : StockTab.StockHeader.DocType.DocumentTypeShortName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : StockTab.Process.ProcessName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : StockTab.LotNo.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : StockTab.ProductUid.ProductUidName.ToLower().Contains(term.ToLower())
                        )
                    select new ComboBoxResult
                    {
                        id = p.StockInId.ToString(),
                        text = StockTab.StockHeader.DocType.DocumentTypeShortName + "-" + p.StockInNo,
                        TextProp1 = "Balance :" + p.BalanceQty,
                        TextProp2 = "Date :" + p.StockInDate,
                        AProp1 = ProductTab.ProductName + ((StockTab.ProductUid.ProductUidName == null) ? "" : "," + StockTab.ProductUid.ProductUidName),
                        AProp2 = ((Dimension1Tab.Dimension1Name == null) ? "" : Dimension1Tab.Dimension1Name) +
                                    ((Dimension2Tab.Dimension2Name == null) ? "" : "," + Dimension2Tab.Dimension2Name) +
                                    ((Dimension3Tab.Dimension3Name == null) ? "" : "," + Dimension3Tab.Dimension3Name) +
                                    ((Dimension4Tab.Dimension4Name == null) ? "" : "," + Dimension4Tab.Dimension4Name) +
                                    ((p.LotNo == null) ? "" : "," + p.LotNo)
                    });
        }


        public IEnumerable<ComboBoxResult> GetPendingStockInHeaderForIssue(int StockHeaderId, string term)
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            int GodownId = 0;

            if (StockHeader.FromGodownId != null)
            {
                GodownId = (int)StockHeader.FromGodownId;
            }
            else if (StockHeader.GodownId != null)
            {
                GodownId = (int)StockHeader.GodownId;
            }


            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            return (from p in db.ViewStockInBalance
                    join L in db.Stock on p.StockInId equals L.StockId into StockTable
                    from StockTab in StockTable.DefaultIfEmpty()
                    where p.BalanceQty > 0 && StockTab.GodownId == GodownId
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && (string.IsNullOrEmpty(term) ? 1 == 1 : p.StockInNo.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : StockTab.StockHeader.DocType.DocumentTypeShortName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : StockTab.StockHeader.Process.ProcessName.ToLower().Contains(term.ToLower())
                    )
                    group new { p, StockTab } by new { StockTab.StockHeaderId } into Result
                    select new ComboBoxResult
                    {
                        id = Result.Key.StockHeaderId.ToString(),
                        text = Result.Max(i => i.StockTab.StockHeader.DocType.DocumentTypeShortName + "-" + i.StockTab.StockHeader.DocNo),
                        TextProp1 = "Date :" + Result.Max(i => i.StockTab.StockHeader.DocDate),
                        TextProp2 = "Balance :" + Result.Sum(i => i.p.BalanceQty),
                        AProp1 = "Process :" + Result.Max(i => i.StockTab.StockHeader.Process.ProcessName),
                    });
        }


        public IEnumerable<ComboBoxResult> GetLotNo(int StockHeaderId, string term)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            int GodownId = 0;

            if (StockHeader.FromGodownId != null)
            {
                GodownId = (int)StockHeader.FromGodownId;
            }
            else if (StockHeader.GodownId != null)
            {
                GodownId = (int)StockHeader.GodownId;
            }


            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            return (from p in db.ViewStockInBalance
                    join L in db.Stock on p.StockInId equals L.StockId into StockTable
                    from StockTab in StockTable.DefaultIfEmpty()
                    where p.LotNo != null && p.BalanceQty > 0 && StockTab.GodownId == GodownId
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && (string.IsNullOrEmpty(term) ? 1 == 1 : p.LotNo.ToLower().Contains(term.ToLower()))
                    group new { p, StockTab } by new { StockTab.LotNo } into Result
                    select new ComboBoxResult
                    {
                        id = Result.Key.LotNo.ToString(),
                        text = Result.Max(i => i.StockTab.LotNo),
                        TextProp1 = "Balance :" + Result.Sum(i => i.p.BalanceQty),
                    });
        }


        public IQueryable<ComboBoxResult> GetCustomProductGroups(int Id, string term)
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            return (from p in db.ProductGroups
                    where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.ProductTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(p.ProductGroupId.ToString()))
                    && (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductGroupName.ToLower().Contains(term.ToLower()))
                    orderby p.ProductGroupName
                    select new ComboBoxResult
                    {
                        id = p.ProductGroupId.ToString(),
                        text = p.ProductGroupName,
                    });
        }


        public IQueryable<ComboBoxResult> GetCustomReferenceDocIds(int Id, string term)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            SqlParameter SqlParameterPersonIdId = new SqlParameter("@PersonId", StockHeader.PersonId);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", StockHeader.ProcessId);


            //IEnumerable<ComboBoxResult> ComboBoxResult1 = db.Database.SqlQuery<ComboBoxResult>(settings.SqlProcHelpListReferenceDocId + " @PersonId, @ProcessId", SqlParameterPersonIdId, SqlParameterProcessId).ToList();
            IQueryable<ComboBoxResult> ComboBoxResult = db.Database.SqlQuery<ComboBoxResult>(settings.SqlProcHelpListReferenceDocId + " @PersonId, @ProcessId", SqlParameterPersonIdId, SqlParameterProcessId).ToList().AsQueryable();

            return (from p in ComboBoxResult
                    where (string.IsNullOrEmpty(term) ? 1 == 1 : p.text.ToLower().Contains(term.ToLower()))
                    select new ComboBoxResult
                    {
                        id = p.id,
                        text = p.text,
                    });
        }

        public ComboBoxResult SetCustomReferenceDocIds(int StockLineId)
        {
            var StockLine = new StockLineService(_unitOfWork).Find(StockLineId);
            var StockHeader = new StockHeaderService(_unitOfWork).Find(StockLine.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            SqlParameter SqlParameterPersonIdId = new SqlParameter("@PersonId", StockHeader.PersonId);
            SqlParameter SqlParameterProcessId = new SqlParameter("@ProcessId", StockHeader.ProcessId);

            IEnumerable<ComboBoxResult> ComboBoxResult = db.Database.SqlQuery<ComboBoxResult>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + settings.SqlProcHelpListReferenceDocId + "@PersonId, @ProcessId", SqlParameterPersonIdId, SqlParameterProcessId).ToList();


            return (from p in ComboBoxResult
                    where p.id == StockLine.ReferenceDocId.ToString()
                    select new ComboBoxResult
                    {
                        id = p.id,
                        text = p.text,
                    }).FirstOrDefault();

        }

        public IEnumerable<ComboBoxResult> FGetProductUidHelpList(int Id, string term)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(Id);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            SqlParameter SQLStockHeaderId = new SqlParameter("@StockHeaderId", Id);
            IEnumerable<ComboBoxResult> ProductUidList = db.Database.SqlQuery<ComboBoxResult>(settings.SqlProcProductUidHelpList + " @StockHeaderId", SQLStockHeaderId).ToList();


            var temp = (from P in ProductUidList
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : P.text.ToLower().Contains(term.ToLower())
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : P.AProp1.ToLower().Contains(term.ToLower()))
                        || (string.IsNullOrEmpty(term) ? 1 == 1 : P.AProp2.ToLower().Contains(term.ToLower()))
                        )
                        select new ComboBoxResult
                        {
                            id = P.id,
                            text = P.text,
                            TextProp1 = P.TextProp1,
                            TextProp2 = P.TextProp2,
                            AProp1 = P.AProp1,
                            AProp2 = P.AProp2
                        }).ToList();

            return temp;
        }


        public IEnumerable<StockLineViewModel> GetStockInForFilters(StockInFiltersForIssue vm)
        {

            var StockHeader = new StockHeaderService(_unitOfWork).Find(vm.StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);

            int? GodownId = 0;
            if (StockHeader.FromGodownId != null)
                GodownId = StockHeader.FromGodownId;
            else
                GodownId = StockHeader.GodownId;

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ContraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { ContraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { ContraSites = new string[] { "NA" }; }

            string[] ContraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { ContraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { ContraDivisions = new string[] { "NA" }; }

            string[] ProductIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductId)) { ProductIdArr = vm.ProductId.Split(",".ToCharArray()); }
            else { ProductIdArr = new string[] { "NA" }; }

            string[] StockInIdArr = null;
            if (!string.IsNullOrEmpty(vm.StockInHeaderId)) { StockInIdArr = vm.StockInHeaderId.Split(",".ToCharArray()); }
            else { StockInIdArr = new string[] { "NA" }; }

            string[] ProductGroupIdArr = null;
            if (!string.IsNullOrEmpty(vm.ProductGroupId)) { ProductGroupIdArr = vm.ProductGroupId.Split(",".ToCharArray()); }
            else { ProductGroupIdArr = new string[] { "NA" }; }

            string[] Dim1IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension1Id)) { Dim1IdArr = vm.Dimension1Id.Split(",".ToCharArray()); }
            else { Dim1IdArr = new string[] { "NA" }; }

            string[] Dim2IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension2Id)) { Dim2IdArr = vm.Dimension2Id.Split(",".ToCharArray()); }
            else { Dim2IdArr = new string[] { "NA" }; }

            string[] Dim3IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension3Id)) { Dim3IdArr = vm.Dimension3Id.Split(",".ToCharArray()); }
            else { Dim3IdArr = new string[] { "NA" }; }

            string[] Dim4IdArr = null;
            if (!string.IsNullOrEmpty(vm.Dimension4Id)) { Dim4IdArr = vm.Dimension4Id.Split(",".ToCharArray()); }
            else { Dim4IdArr = new string[] { "NA" }; }

            string[] CostCenterArr = null;
            if (!string.IsNullOrEmpty(vm.CostCenterId)) { CostCenterArr = vm.CostCenterId.Split(",".ToCharArray()); }
            else { CostCenterArr = new string[] { "NA" }; }

            string[] LotNoArr = null;
            if (!string.IsNullOrEmpty(vm.LotNo)) { LotNoArr = vm.LotNo.Split(",".ToCharArray()); }
            else { LotNoArr = new string[] { "NA" }; }


            var temp = (from p in db.ViewStockInBalance
                        join S in db.Stock on p.StockInId equals S.StockId into StockTable
                        from StockTab in StockTable.DefaultIfEmpty()
                        where StockTab.GodownId == GodownId
                        && (string.IsNullOrEmpty(vm.ProductId) ? 1 == 1 : ProductIdArr.Contains(p.ProductId.ToString()))
                        && (string.IsNullOrEmpty(vm.StockInHeaderId) ? 1 == 1 : StockInIdArr.Contains(StockTab.StockHeaderId.ToString()))
                        && (string.IsNullOrEmpty(vm.ProductGroupId) ? 1 == 1 : ProductGroupIdArr.Contains(StockTab.Product.ProductGroup.ProductGroupId.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension1Id) ? 1 == 1 : Dim1IdArr.Contains(p.Dimension1Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension2Id) ? 1 == 1 : Dim2IdArr.Contains(p.Dimension2Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension3Id) ? 1 == 1 : Dim3IdArr.Contains(p.Dimension3Id.ToString()))
                        && (string.IsNullOrEmpty(vm.Dimension4Id) ? 1 == 1 : Dim4IdArr.Contains(p.Dimension4Id.ToString()))
                        && (string.IsNullOrEmpty(vm.CostCenterId) ? 1 == 1 : CostCenterArr.Contains(StockTab.CostCenterId.ToString()))
                        && (string.IsNullOrEmpty(vm.LotNo) ? 1 == 1 : LotNoArr.Contains(p.LotNo.ToString()))
                        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(StockTab.Product.ProductGroup.ProductTypeId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraSites) ? 1 == 1 : ContraSites.Contains(StockTab.StockHeader.SiteId.ToString()))
                        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? 1 == 1 : ContraDivisions.Contains(StockTab.StockHeader.DivisionId.ToString()))
                        && p.BalanceQty > 0
                        orderby p.ProductId, p.Dimension1Id, p.Dimension2Id, p.Dimension3Id, p.Dimension4Id
                        select new StockLineViewModel
                        {
                            Dimension1Name = StockTab.Dimension1.Dimension1Name,
                            Dimension2Name = StockTab.Dimension2.Dimension2Name,
                            Dimension3Name = StockTab.Dimension3.Dimension3Name,
                            Dimension4Name = StockTab.Dimension4.Dimension4Name,
                            Dimension1Id = p.Dimension1Id,
                            Dimension2Id = p.Dimension2Id,
                            Dimension3Id = p.Dimension3Id,
                            Dimension4Id = p.Dimension4Id,
                            ProcessId = StockTab.ProcessId,
                            LotNo = p.LotNo,
                            PlanNo = StockTab.PlanNo,
                            Specification = StockTab.Specification,
                            StockInBalanceQty = p.BalanceQty,
                            Qty = p.BalanceQty,
                            StockInNo = StockTab.StockHeader.DocNo,
                            ProductName = StockTab.Product.ProductName,
                            ProductId = p.ProductId,
                            CostCenterId = StockTab.CostCenterId,
                            StockHeaderId = vm.StockHeaderId,
                            StockInId = p.StockInId,
                            UnitId = StockTab.Product.UnitId,
                            UnitName = StockTab.Product.Unit.UnitName,
                            UnitDecimalPlaces = (byte?)StockTab.Product.Unit.DecimalPlaces ?? 0,
                        });
            return temp;
        }

        public IEnumerable<ComboBoxResult> GetStockProcessBalanceForReceive(int StockHeaderId, string term)
        {
            var StockHeader = new StockHeaderService(_unitOfWork).Find(StockHeaderId);

            var settings = new StockHeaderSettingsService(_unitOfWork).GetStockHeaderSettingsForDocument(StockHeader.DocTypeId, StockHeader.DivisionId, StockHeader.SiteId);


            string[] contraSites = null;
            if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
            else { contraSites = new string[] { "NA" }; }

            string[] contraDivisions = null;
            if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
            else { contraDivisions = new string[] { "NA" }; }

            string[] ProductTypes = null;
            if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            else { ProductTypes = new string[] { "NA" }; }

            string[] ProductGroups = null;
            if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            else { ProductGroups = new string[] { "NA" }; }

            string[] Products = null;
            if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            else { Products = new string[] { "NA" }; }


            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


            //var StockProcessBalance = (from H in db.StockHeader.Where(i => i.ProcessId == StockHeader.ProcessId && i.PersonId == StockHeader.PersonId)
            //                           join L in db.StockProcess on H.StockHeaderId equals L.StockHeaderId into StockProcessTable
            //                           from StockProcessTab in StockProcessTable.DefaultIfEmpty()
            //                           where (string.IsNullOrEmpty(settings.filterContraSites) ? H.SiteId == CurrentSiteId : contraSites.Contains(H.SiteId.ToString()))
            //        && (string.IsNullOrEmpty(settings.filterContraDivisions) ? H.DivisionId == CurrentDivisionId : contraDivisions.Contains(H.DivisionId.ToString()))
            //        && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(StockProcessTab.Product.ProductGroup.ProductTypeId.ToString()))
            //        && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(StockProcessTab.Product.ProductGroupId.ToString()))
            //        && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(StockProcessTab.ProductId.ToString()))
            //        && (string.IsNullOrEmpty(term) ? 1 == 1 : StockProcessTab.Product.ProductName.ToLower().Contains(term.ToLower())
            //            || string.IsNullOrEmpty(term) ? 1 == 1 : StockProcessTab.Dimension1.Dimension1Name.ToLower().Contains(term.ToLower())
            //            || string.IsNullOrEmpty(term) ? 1 == 1 : StockProcessTab.Dimension2.Dimension2Name.ToLower().Contains(term.ToLower())
            //            || string.IsNullOrEmpty(term) ? 1 == 1 : StockProcessTab.Dimension3.Dimension3Name.ToLower().Contains(term.ToLower())
            //            || string.IsNullOrEmpty(term) ? 1 == 1 : StockProcessTab.Dimension4.Dimension4Name.ToLower().Contains(term.ToLower())
            //            || string.IsNullOrEmpty(term) ? 1 == 1 : StockProcessTab.LotNo.ToLower().Contains(term.ToLower()))
            //                           group new { H, StockProcessTab } by new
            //                           {
            //                               H.SiteId,
            //                               H.DivisionId,
            //                               H.PersonId,
            //                               StockProcessTab.ProcessId,
            //                               StockProcessTab.ProductId,
            //                               StockProcessTab.Dimension1Id,
            //                               StockProcessTab.Dimension2Id,
            //                               StockProcessTab.Dimension3Id,
            //                               StockProcessTab.Dimension4Id,
            //                               StockProcessTab.LotNo
            //                           } into Result
            //                           where Result.Sum(i => i.StockProcessTab.Qty_Rec - i.StockProcessTab.Qty_Iss) > 0
            //                           select new ComboBoxResult
            //                           {
            //                               id = Result.Max(i => i.StockProcessTab.StockProcessId).ToString(),
            //                               text = Result.Max(i => i.StockProcessTab.Product.ProductName),
            //                               TextProp1 = "Balance :" + Result.Sum(i => i.StockProcessTab.Qty_Rec - i.StockProcessTab.Qty_Iss),
            //                               AProp1 = Result.Max(i => i.StockProcessTab.Dimension1.Dimension1Name == null ? "" : i.StockProcessTab.Dimension1.Dimension1Name) +
            //                                        Result.Max(i => i.StockProcessTab.Dimension2.Dimension2Name == null ? "" : i.StockProcessTab.Dimension2.Dimension2Name) +
            //                                        Result.Max(i => i.StockProcessTab.Dimension3.Dimension3Name == null ? "" : i.StockProcessTab.Dimension3.Dimension3Name) +
            //                                        Result.Max(i => i.StockProcessTab.Dimension4.Dimension4Name == null ? "" : i.StockProcessTab.Dimension4.Dimension4Name),
            //                               AProp2 = Result.Max(i => i.StockProcessTab.LotNo == null ? "" : i.StockProcessTab.LotNo)
            //                           });

            //return StockProcessBalance;




            return (from p in db.ViewStockProcessBalance
                    join pt in db.Product on p.ProductId equals pt.ProductId into ProductTable
                    from ProductTab in ProductTable.DefaultIfEmpty()
                    join D1 in db.Dimension1 on p.Dimension1Id equals D1.Dimension1Id into Dimension1Table
                    from Dimension1Tab in Dimension1Table.DefaultIfEmpty()
                    join D2 in db.Dimension2 on p.Dimension2Id equals D2.Dimension2Id into Dimension2Table
                    from Dimension2Tab in Dimension2Table.DefaultIfEmpty()
                    join D3 in db.Dimension3 on p.Dimension3Id equals D3.Dimension3Id into Dimension3Table
                    from Dimension3Tab in Dimension3Table.DefaultIfEmpty()
                    join D4 in db.Dimension4 on p.Dimension4Id equals D4.Dimension4Id into Dimension4Table
                    from Dimension4Tab in Dimension4Table.DefaultIfEmpty()
                    where p.BalanceQty > 0 && p.PersonId == StockHeader.PersonId && p.ProcessId == StockHeader.ProcessId
                    && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(ProductTab.ProductGroup.ProductTypeId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(ProductTab.ProductGroupId.ToString()))
                    && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(ProductTab.ProductId.ToString()))
                    && (string.IsNullOrEmpty(term) ? 1 == 1 : ProductTab.ProductName.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension1Tab.Dimension1Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension2Tab.Dimension2Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension3Tab.Dimension3Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : Dimension4Tab.Dimension4Name.ToLower().Contains(term.ToLower())
                        || string.IsNullOrEmpty(term) ? 1 == 1 : p.LotNo.ToLower().Contains(term.ToLower())
                        )
                    select new ComboBoxResult
                    {
                        id = p.StockProcessBalanceId.ToString(),
                        text = ProductTab.ProductName,
                        TextProp1 = "Balance :" + p.BalanceQty,
                        AProp1 = ((Dimension1Tab.Dimension1Name == null) ? "" : Dimension1Tab.Dimension1Name) +
                                    ((Dimension2Tab.Dimension2Name == null) ? "" : "," + Dimension2Tab.Dimension2Name) +
                                    ((Dimension3Tab.Dimension3Name == null) ? "" : "," + Dimension3Tab.Dimension3Name) +
                                    ((Dimension4Tab.Dimension4Name == null) ? "" : "," + Dimension4Tab.Dimension4Name),
                        AProp2 = ((p.LotNo == null) ? "" : "Lot No." + p.LotNo)
                    });
        }

        public void Dispose()
        {
        }

        public IEnumerable<StockIssueForProductsFilterViewModel> GetProductsForFilters(ProductsFiltersForIssue svm)
        {

            SqlParameter SqlParameterCostCenterId = new SqlParameter("@CostCenterId", svm.CostCenterId);

            IEnumerable<StockIssueForProductsFilterViewModel> StockAvailableForPacking = db.Database.SqlQuery<StockIssueForProductsFilterViewModel>("Web.spGetProductsFromCostCenter @CostCenterId", SqlParameterCostCenterId).ToList();

            var temp2 = (from p in StockAvailableForPacking
                         select new StockIssueForProductsFilterViewModel
                         {
                             JobHeaderDocNo = p.JobHeaderDocNo,
                             JobOrderHeaderId = p.JobOrderHeaderId,
                             JobOrderLineId = p.JobOrderLineId,
                             Qty = p.Qty,
                             ProductId = p.ProductId,
                             ProductName = p.ProductName,
                             CostCenterId = p.CostCenterId,
                             CostCenterName = p.CostCenterName,
                             StockHeaderId = svm.StockHeaderId,
                             IsBomPosted= p.IsBomPosted,
                         }).ToList();

            return temp2;
        }


        //public IEnumerable<StockLineViewModel> GetBOMDetailForProducts(ProductsFilterViewModel vm)
        //{
        //    //var JobOrderBomMaterialIssueVmList = (from H in vm.StockIssueForProductsFilterViewModel
        //    //                                  join JOB in db.JobOrderBom on H.JobOrderLineId equals JOB.JobOrderLineId into JobOrderBomTable
        //    //                                  from JobOrderBomTab in JobOrderBomTable.DefaultIfEmpty()
        //    //                                  where H.Qty > 0 && JobOrderBomTab.Qty > 0
        //    //                                  select new JobOrderBomMaterialIssueViewModel
        //    //                                {
        //    //                                    JobOrderBomId = JobOrderBomTab.JobOrderBomId,
        //    //                                    CostCenterId = JobOrderBomTab.JobOrderHeader.CostCenterId ?? 0,
        //    //                                    ProductId = JobOrderBomTab.ProductId,
        //    //                                    Dimension1Id = JobOrderBomTab.Dimension1Id ?? 0,
        //    //                                    IssueForQty = H.Qty ?? 0,
        //    //                                    Qty = JobOrderBomTab.Qty,
        //    //                                }).ToList();

        //    System.Web.HttpContext.Current.Session["JobOrderBomMaterialIssue"] = JobOrderBomMaterialIssueVmList;


        //    var tempN = (from H in vm.StockIssueForProductsFilterViewModel
        //                 join JOB in db.JobOrderBom on H.JobOrderLineId equals JOB.JobOrderLineId into tableJOB
        //                 from tabJOB in tableJOB.DefaultIfEmpty()
        //                 join JOL in db.JobOrderLine on H.JobOrderLineId equals JOL.JobOrderLineId into tableJOL
        //                 from tabJOL in tableJOL.DefaultIfEmpty()
        //                 where H.Qty > 0 && tabJOB.Qty > 0
        //                 group tabJOB by new { H.StockHeaderId, H.CostCenterId, tabJOB.JobOrderLineId, H.Qty, tabJOB.ProductId, tabJOB.Dimension1Id } into tableG
        //                 select new
        //                 {
        //                     Dimension1Id = tableG.Key.Dimension1Id,
        //                     ReqQty = (Decimal)tableG.Sum(m => m.Qty),
        //                     ProductId = tableG.Key.ProductId,
        //                     Qty = tableG.Key.Qty,
        //                     CostCenterId = tableG.Key.CostCenterId,
        //                     StockHeaderId = tableG.Key.StockHeaderId,
        //                     JobOrderLineId = tableG.Key.JobOrderLineId
        //                 }
        //    ).ToList();



        //    var temp1 = (from H in tempN
        //                 join JOL in db.JobOrderLine on H.JobOrderLineId equals JOL.JobOrderLineId into tableJOL
        //                 from tabJOL in tableJOL.DefaultIfEmpty()
        //                 join JOH in db.JobOrderHeader on tabJOL.JobOrderHeaderId equals JOH.JobOrderHeaderId into tableJOH
        //                 from tabJOH in tableJOH.DefaultIfEmpty()
        //                 select new
        //                 {

        //                     Dimension1Id = H.Dimension1Id,
        //                     ProcessId = tabJOH.ProcessId,
        //                     RequisitionBalanceQty = H.ReqQty / tabJOL.Qty * H.Qty,
        //                     Qty = (decimal)H.ReqQty / (decimal)tabJOL.Qty * (decimal)H.Qty,
        //                     ProductId = H.ProductId,
        //                     CostCenterId = H.CostCenterId,
        //                     StockHeaderId = H.StockHeaderId
        //                 }).ToList();


        //    var tempP = (from P in db.Product
        //                 join PG in db.ProductGroups on P.ProductGroupId equals PG.ProductGroupId into tablePG
        //                 from tabPG in tablePG.DefaultIfEmpty()
        //                 join PT in db.ProductTypes on tabPG.ProductTypeId equals PT.ProductTypeId into tablePT
        //                 from tabPT in tablePT.DefaultIfEmpty()
        //                 join U in db.Units on P.UnitId equals U.UnitId into tableU
        //                 from tabU in tableU.DefaultIfEmpty()
        //                 where tabPT.ProductTypeName == "YARN" || tabPT.ProductTypeName == "Yarn SKU"
        //                 select new
        //                 {

        //                     ProductId = P.ProductId,
        //                     ProductGroupId = P.ProductGroupId,
        //                     ProductName = P.ProductName,
        //                     ProductGroupName = tabPG.ProductGroupName.Replace(" ", "").Replace("/", ""),
        //                     UnitId = tabU.UnitId,
        //                     UnitName = tabU.UnitName,
        //                     UnitDecimalPlaces = tabU.DecimalPlaces,
        //                 }).ToList();


        //    var temp3 = (from H in temp1
        //                 join P in tempP on H.ProductId equals P.ProductId into tableP
        //                 from tabP in tableP.DefaultIfEmpty()
        //                 select new
        //                 {

        //                     Dimension1Id = H.Dimension1Id,
        //                     ProcessId = H.ProcessId,
        //                     RequisitionBalanceQty = H.RequisitionBalanceQty,
        //                     Qty = H.Qty,
        //                     ProductName = tabP.ProductName,
        //                     ProductId = H.ProductId,
        //                     ProductGroupName = tabP.ProductGroupName,
        //                     ProductGroupId = tabP.ProductGroupId,
        //                     CostCenterId = H.CostCenterId,
        //                     StockHeaderId = H.StockHeaderId,
        //                     UnitId = tabP.UnitId,
        //                     UnitName = tabP.UnitName,
        //                     UnitDecimalPlaces = tabP.UnitDecimalPlaces,
        //                 }
        //                ).ToList();

        //    var temp4 = (from H in temp3
        //                 group H by new { H.StockHeaderId, H.CostCenterId, H.ProcessId, H.ProductId, H.ProductName, H.ProductGroupName, H.Dimension1Id } into tableG
        //                 select new
        //                 {

        //                     Dimension1Id = tableG.Key.Dimension1Id,
        //                     ProcessId = tableG.Key.ProcessId,
        //                     RequisitionBalanceQty = (Decimal)tableG.Sum(m => m.Qty),
        //                     Qty = (Decimal)tableG.Sum(m => m.Qty),
        //                     ProductName = tableG.Key.ProductName,
        //                     ProductId = tableG.Key.ProductId,
        //                     ProductGroupName = tableG.Key.ProductGroupName,
        //                     CostCenterId = tableG.Key.CostCenterId,
        //                     StockHeaderId = tableG.Key.StockHeaderId,
        //                     UnitId = tableG.Max(m => m.UnitId),
        //                     UnitName = tableG.Max(m => m.UnitName),
        //                     UnitDecimalPlaces = tableG.Max(m => m.UnitDecimalPlaces),
        //                 }
        //            ).ToList();


        //    var temp = (from H in temp4
        //                join D1 in db.Dimension1 on H.Dimension1Id equals D1.Dimension1Id into tableD1
        //                from tabD1 in tableD1.DefaultIfEmpty()
        //                select new StockLineViewModel
        //                {
        //                    Dimension1Name = H.Dimension1Id == null ? null : tabD1.Dimension1Name,
        //                    Dimension1Id = H.Dimension1Id,
        //                    ProcessId = H.ProcessId,
        //                    RequisitionBalanceQty = H.RequisitionBalanceQty,
        //                    Qty = H.Qty,
        //                    ProductName = H.ProductName,
        //                    ProductId = H.ProductId,
        //                    ProductGroupName = H.ProductGroupName,
        //                    CostCenterId = H.CostCenterId,
        //                    StockHeaderId = H.StockHeaderId,
        //                    UnitId = H.UnitId,
        //                    UnitName = H.UnitName,
        //                    UnitDecimalPlaces = H.UnitDecimalPlaces,
        //                }
        //).ToList();

        //    return temp;
        //}

        public IEnumerable<StockLineViewModel> GetBOMDetailForProducts(ProductsFilterViewModel vm)
        {
            string[] JobOrderLineIds = null;
            JobOrderLineIds = vm.StockIssueForProductsFilterViewModel.Select(i => i.JobOrderLineId.ToString()).ToArray();

            var TempJobOrderBom = (from L in db.JobOrderBom 
                                   where JobOrderLineIds.Contains(L.JobOrderLineId.ToString())
                                   && L.Product.ProductGroup.ProductType.ProductTypeName == "YARN" || L.Product.ProductGroup.ProductType.ProductTypeName == "Yarn SKU"
                                   select new
                                   {
                                       JobOrderLineId = L.JobOrderLineId ?? 0,
                                       JobOrderBomId = L.JobOrderBomId,
                                       CostCenterId = L.JobOrderHeader.CostCenterId,
                                       ProductId = L.ProductId,
                                       ProductName = L.Product.ProductName,
                                       ProductGroupName = L.Product.ProductGroup.ProductGroupName.Replace(" ", "").Replace("/", ""),
                                       Dimension1Id = L.Dimension1Id,
                                       Dimension1Name = L.Dimension1.Dimension1Name,
                                       Qty = L.Qty,
                                       IssueForQty = L.JobOrderLine.Qty,
                                       UnitId = L.Product.UnitId,
                                       UnitName = L.Product.Unit.UnitName,
                                       UnitDecimalPlaces = L.Product.Unit.DecimalPlaces,
                                       ProcessId = L.JobOrderHeader.ProcessId
                                   }).ToList();


            var JobOrderBomMaterialIssueVmList = (from H in vm.StockIssueForProductsFilterViewModel
                                                  join JOB in TempJobOrderBom on H.JobOrderLineId equals JOB.JobOrderLineId
                                                  where H.Qty > 0 && JOB.Qty > 0
                                                  select new JobOrderBomMaterialIssueViewModel
                                                  {
                                                      JobOrderBomId = JOB.JobOrderBomId,
                                                      CostCenterId = JOB.CostCenterId ?? 0,
                                                      ProductId = JOB.ProductId,
                                                      Dimension1Id = JOB.Dimension1Id,
                                                      IssueForQty = H.Qty ?? 0,
                                                      Qty = (JOB.Qty / JOB.IssueForQty) * (H.Qty ?? 0),
                                                  }).ToList();


            System.Web.HttpContext.Current.Session["JobOrderBomMaterialIssue"] = JobOrderBomMaterialIssueVmList;


            var StockLineVm = (from H in vm.StockIssueForProductsFilterViewModel
                         join JOB in TempJobOrderBom on H.JobOrderLineId equals JOB.JobOrderLineId into TempJobOrderBomTable
                         from TempJobOrderBomTab in TempJobOrderBomTable.DefaultIfEmpty()
                         where H.Qty > 0 && TempJobOrderBomTab.Qty > 0
                         group new { H, TempJobOrderBomTab } by new { H.StockHeaderId, TempJobOrderBomTab.CostCenterId, TempJobOrderBomTab.ProductId, TempJobOrderBomTab.Dimension1Id } into Result
                         select new StockLineViewModel
                         {
                             Dimension1Name = Result.Max(i => i.TempJobOrderBomTab.Dimension1Name),
                             Dimension1Id = Result.Key.Dimension1Id,
                             ProcessId = Result.Max(i => i.TempJobOrderBomTab.ProcessId),
                             //RequisitionBalanceQty = Result.Sum(i => i.TempJobOrderBomTab.Qty),
                             //Qty = Result.Sum(i => i.TempJobOrderBomTab.Qty),
                             RequisitionBalanceQty = (decimal ?) Result.Sum(i => i.TempJobOrderBomTab.Qty)/ (decimal ?) Result.Max(i => i.TempJobOrderBomTab.IssueForQty) * (decimal ?)Result.Max(i => i.H.Qty),
                             Qty = (decimal) Result.Sum(i => i.TempJobOrderBomTab.Qty) / (decimal) Result.Max(i => i.TempJobOrderBomTab.IssueForQty) * (decimal) Result.Max(i => i.H.Qty),
                             ProductName = Result.Max(i => i.TempJobOrderBomTab.ProductName),
                             ProductId = Result.Key.ProductId,
                             ProductGroupName = Result.Max(i => i.TempJobOrderBomTab.ProductGroupName),
                             CostCenterId = Result.Key.CostCenterId,
                             StockHeaderId = Result.Key.StockHeaderId,
                             UnitId = Result.Max(i => i.TempJobOrderBomTab.UnitId),
                             UnitName = Result.Max(i => i.TempJobOrderBomTab.UnitName),
                             UnitDecimalPlaces = Result.Max(i => i.TempJobOrderBomTab.UnitDecimalPlaces),
                             IssueForQty = Result.Sum(i => i.H.Qty),
                         }).ToList();

            return StockLineVm;
        }


        


    }

    [Serializable]
    public class JobOrderBomMaterialIssueViewModel
    {
        public int StockLineId { get; set; }
        public int JobOrderBomId { get; set; }
        public int CostCenterId { get; set; }
        public int ProductId { get; set; }
        public int? Dimension1Id { get; set; }
        public decimal IssueForQty { get; set; }
        public decimal Qty { get; set; }
    }
}
