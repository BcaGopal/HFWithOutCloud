using Data.Infrastructure;
using Data.Models;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Model.Models;
using System.Data.SqlTypes;
using Model.ViewModels;

namespace Service
{
    public interface IMaterialBalanceService
    {
        IEnumerable<PersonWiseBalance> GetPersonWiseBalance(int PersonId);
        IEnumerable<PersonInfo> GetPersonInfo(int PersonId);
        IEnumerable<MatereialBalanceJobWorkerViewModel> MaterialBalanceDisplay(MaterialBalanceUpDateViewModel Vm);
        IEnumerable<PersonValue> GetPersonValues(int PersonId);
        int InsertJobStockLineAProcesess(List<StockLineAndProcessViewModel> PostedViewModel);
        ReportHeaderCompanyDetailForJobConJumption GetReportHeaderCompanyDetail();
        IQueryable<ComboBoxResult> GetRecieveNoProcessAndPerson(int PersonId, int ProcessId, string term);
    }
    public class MaterialBalanceService : IMaterialBalanceService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        public MaterialBalanceService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<PersonWiseBalance> GetPersonWiseBalance(int PersonId)
        {
            try
            {
                SqlParameter ParaMeterPerson = new SqlParameter("@JobWorker", PersonId);
                IEnumerable<PersonWiseBalance> Ballist = null;
                Ballist = db.Database.SqlQuery<PersonWiseBalance>("Web.GetJobWorkerBalance @JobWorker", ParaMeterPerson).ToList();
                return Ballist;
            }
            catch (Exception ex) { throw ex; }
        }
        public IEnumerable<PersonValue> GetPersonValues(int PersonId)
        {
            try
            {
                SqlParameter ParaMeterPerson = new SqlParameter("@JobWorker", PersonId);
                IEnumerable<PersonValue> Valuelist = null;
                Valuelist = db.Database.SqlQuery<PersonValue>("Web.GetPersonValue @JobWorker", ParaMeterPerson).ToList();
                return Valuelist;
            }
            catch (Exception ex) { throw ex; }
        }
        public IEnumerable<PersonInfo> GetPersonInfo(int PersonId)
        {
            try
            {
                // var InfoList = (from p in db.Persons where p.PersonID == PersonId select p).ToList();
                SqlParameter ParaMeterPerson = new SqlParameter("@JobWorker", PersonId);
                IEnumerable<PersonInfo> Ballist = null;
                Ballist = db.Database.SqlQuery<PersonInfo>("web.GetInfoForJobConjumtion @JobWorker", ParaMeterPerson).ToList();
                return Ballist;
            }
            catch (Exception ex) { throw ex; }
        }
        public IEnumerable<MatereialBalanceJobWorkerViewModel> MaterialBalanceDisplay(MaterialBalanceUpDateViewModel Vm)
        {
            int ShowNegativeBalance = 0;
            if (Vm.NegativeBalance == true)
                ShowNegativeBalance = 1;
            else
                ShowNegativeBalance = 0;
            try
            {
                SqlParameter ParaProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Vm.Product) ? Vm.Product : (object)DBNull.Value);
                SqlParameter ParaAsOnDate = new SqlParameter("@AsOnDate", !string.IsNullOrEmpty(Vm.AsOnDate) ? Vm.AsOnDate : (object)DBNull.Value);
                SqlParameter ParaProcess = new SqlParameter("@Process", !string.IsNullOrEmpty(Vm.Process) ? Vm.Process : (object)DBNull.Value);
                SqlParameter ParaDimension1 = new SqlParameter("@Dimension1", !string.IsNullOrEmpty(Vm.Dimension1Name) ? Vm.Dimension1Name : (object)DBNull.Value);
                SqlParameter ParaDimension2 = new SqlParameter("@Dimension2", !string.IsNullOrEmpty(Vm.Dimension2Name) ? Vm.Dimension2Name : (object)DBNull.Value);
                SqlParameter ParaDimension3 = new SqlParameter("@Dimension3", !string.IsNullOrEmpty(Vm.Dimension3Name) ? Vm.Dimension3Name : (object)DBNull.Value);
                SqlParameter ParaDimension4 = new SqlParameter("@Dimension4", !string.IsNullOrEmpty(Vm.Dimension4Name) ? Vm.Dimension4Name : (object)DBNull.Value);//@PersonId
                SqlParameter ParaPerson = new SqlParameter("@PersonId", !string.IsNullOrEmpty(Vm.JobWorker) ? Vm.JobWorker : (object)DBNull.Value);
                SqlParameter ParaNegativeBalance = new SqlParameter("@NegativeBalance", !string.IsNullOrEmpty(Vm.JobWorker) ? ShowNegativeBalance : (object)DBNull.Value);
                IEnumerable<MatereialBalanceJobWorkerViewModel> MaterialBalanceList = db.Database.SqlQuery<MatereialBalanceJobWorkerViewModel>("Web.JobConjumptionDetails @Product,@AsOnDate,@Process,@Dimension1,@Dimension2,@Dimension3,@Dimension4,@PersonId,@NegativeBalance", ParaProduct, ParaAsOnDate, ParaProcess, ParaDimension1, ParaDimension2, ParaDimension3, ParaDimension4, ParaPerson, ParaNegativeBalance).ToList();
                return MaterialBalanceList;
            }
            catch (Exception ex) { throw ex; }
        }

        public int InsertJobStockLineAProcesess(List<StockLineAndProcessViewModel> PostedViewModel)
        {
            try
            {
                SqlParameter ParaDocTypeIdvm = null;
                SqlParameter ParaDocDatevm = null;
                SqlParameter ParaDocNovm = null;
                SqlParameter ParaPersonvm = null;
                SqlParameter paraProcessvm = null;
                SqlParameter ParaRemarkvm = null;
                SqlParameter ParaStockHeaderId = null;
                SqlParameter ParaProductId = null;
                SqlParameter ParaDimension1Id = null;
                SqlParameter ParaDimension2Id = null;
                SqlParameter ParaDimension3Id = null;
                SqlParameter ParaDimension4Id = null;
                SqlParameter ParaFProcessId = null;
                SqlParameter ParaProcessId = null;
                SqlParameter ParaQty = null;
                SqlParameter ParaUserName = null;
                SqlParameter ParaRemark = null;
                SqlParameter ParaStockProcessId = null;
                SqlParameter ParaDocDate = null;
                SqlParameter ParaReferenceDocId = null;
                SqlParameter ParaReferenceDocTypeId = null;
                LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
                int? ReferenceDocId = PostedViewModel[0].JobReceiveHeaderId;
                JobReceiveHeader Receive = (from b in db.JobReceiveHeader.AsEnumerable()
                                            where b.JobReceiveHeaderId == ReferenceDocId
                                            select new JobReceiveHeader
                                            {
                                                DocTypeId = b.DocTypeId,

                                            }).FirstOrDefault();
                int ReferenceDocTypeId = Receive.DocTypeId;
                using (var context = new ApplicationDbContext())
                {
                    using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                    {
                        try
                        {
                            ParaDocTypeIdvm = new SqlParameter("@DocTypeId", PostedViewModel[0].DocTypeId);
                            ParaDocDatevm = new SqlParameter("@DocDate", PostedViewModel[0].EntryDate);
                            ParaDocNovm = new SqlParameter("@DocNo", PostedViewModel[0].EntryNo);
                            ParaPersonvm = new SqlParameter("@PersonId", PostedViewModel[0].PersonId);
                            paraProcessvm = new SqlParameter("@ProcessId", PostedViewModel[0].ProcessId);
                            ParaRemarkvm = new SqlParameter("@Remark", !string.IsNullOrEmpty(PostedViewModel[0].Remark) ? PostedViewModel[0].Remark : (object)DBNull.Value);
                            ParaUserName = new SqlParameter("@UserName", LogVm.User);
                            ParaReferenceDocId = new SqlParameter("@ReferenceDocId", ReferenceDocId);//RecieveHeaderId
                            ParaReferenceDocTypeId = new SqlParameter("@ReferenceDocTypeId", ReferenceDocTypeId);// DocTYpe

                            SqlParameter ParamStockHeaderId = new SqlParameter()
                            {
                                ParameterName = "@MyID",
                                SqlDbType = SqlDbType.Int,
                                Direction = System.Data.ParameterDirection.Output
                            };
                            context.Database.ExecuteSqlCommand("Exec @MyId = Web.InsertInJobStockHeader @DocTypeId,@DocDate,@DocNo, @PersonId,@ProcessId,@Remark,@UserName,@ReferenceDocId,@ReferenceDocTypeId", ParaDocTypeIdvm, ParaDocDatevm, ParaDocNovm, ParaPersonvm, paraProcessvm, ParaRemarkvm, ParaUserName, ParaReferenceDocId, ParaReferenceDocTypeId, ParamStockHeaderId);
                            int StockHeaderId = (int)ParamStockHeaderId.Value;
                            foreach (var Item in PostedViewModel)
                            {
                                SqlParameter ParamStockProcessId = new SqlParameter()
                                {
                                    ParameterName = "@MyID",
                                    SqlDbType = SqlDbType.Int,
                                    Direction = System.Data.ParameterDirection.Output
                                };

                                FGetParameters(ref ParaStockHeaderId, ref ParaDocDate, ref ParaProductId, ref ParaFProcessId, ref ParaProcessId,
                                        ref ParaQty, ref ParaRemark, ref ParaStockProcessId,
                                        ref ParaDimension1Id, ref ParaDimension2Id, ref ParaDimension3Id, ref ParaDimension4Id, ref ParaUserName,
                                        StockHeaderId, Item, PostedViewModel[0].Remark, 0);

                                context.Database.ExecuteSqlCommand("Exec @MyId = Web.InsertInJobStockProcess @StockHeaderId,@DocDate,@ProductId, @ProcessId,@Qty,@Remark,@Dimension1Id,@Dimension2Id,@UserName", ParaStockHeaderId, ParaDocDate, ParaProductId, ParaProcessId, ParaQty, ParaRemark, ParaDimension1Id, ParaDimension2Id, ParaUserName, ParamStockProcessId);
                                int StockProcessId = (int)ParamStockProcessId.Value;

                                FGetParameters(ref ParaStockHeaderId, ref ParaDocDate, ref ParaProductId, ref ParaFProcessId, ref ParaProcessId,
                                        ref ParaQty, ref ParaRemark, ref ParaStockProcessId,
                                        ref ParaDimension1Id, ref ParaDimension2Id, ref ParaDimension3Id, ref ParaDimension4Id, ref ParaUserName,
                                        StockHeaderId, Item, PostedViewModel[0].Remark, StockProcessId);

                                context.Database.ExecuteSqlCommand("Web.InsertInJobStockLines @StockHeaderId,@ProductId,@FromProcessId, @Qty, @Remark,@StockProcessId,@Dimension1Id,@Dimension2Id,@Dimension3Id,@Dimension4Id,@UserName ", ParaStockHeaderId, ParaProductId, ParaFProcessId, ParaQty, ParaRemark, ParaStockProcessId, ParaDimension1Id, ParaDimension2Id, ParaDimension3Id, ParaDimension4Id, ParaUserName);
                            }
                            dbTran.Commit();
                            return StockHeaderId;

                        }
                        catch (SqlException ex) { dbTran.Rollback(); throw ex; }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void FGetParameters(ref SqlParameter ParaStockHeaderId, ref SqlParameter ParaDocDate, ref SqlParameter ParaProductId, ref SqlParameter ParaFProcessId,
                            ref SqlParameter ParaProcessId,
                            ref SqlParameter ParaQty, ref SqlParameter ParaRemark, ref SqlParameter ParaStockProcessId,
                            ref SqlParameter ParaDimension1Id, ref SqlParameter ParaDimension2Id, ref SqlParameter ParaDimension3Id,
                            ref SqlParameter ParaDimension4Id, ref SqlParameter ParaUserName,
                            int StockHeaderId, StockLineAndProcessViewModel Item, string Remark, int StockProcessId)
        {
            ParaStockHeaderId = new SqlParameter("@StockHeaderId", StockHeaderId);
            ParaDocDate = new SqlParameter("@DocDate", !string.IsNullOrEmpty(Item.DocDate) ? Item.DocDate : (object)DBNull.Value);
            ParaProductId = new SqlParameter("@ProductId", Item.ProductId);
            ParaFProcessId = new SqlParameter("@FromProcessId", Item.ProcessId);
            ParaProcessId = new SqlParameter("@ProcessId", Item.ProcessId);
            ParaQty = new SqlParameter("@Qty", Item.ConsumeQty == 0 ? SqlDecimal.Null : (object)Item.ConsumeQty);
            ParaRemark = new SqlParameter("@Remark", !string.IsNullOrEmpty(Item.LineRemark) ? Item.LineRemark : (object)DBNull.Value);
            ParaStockProcessId = new SqlParameter("@StockProcessId", StockProcessId);

            if (Item.Dimension1Id == null || Item.Dimension1Id == 0)
                ParaDimension1Id = new SqlParameter("@Dimension1Id", DBNull.Value);
            else
                ParaDimension1Id = new SqlParameter("@Dimension1Id", Item.Dimension1Id);

            if (Item.Dimension2Id == null || Item.Dimension2Id == 0)
                ParaDimension2Id = new SqlParameter("@Dimension2Id", DBNull.Value);
            else
                ParaDimension2Id = new SqlParameter("@Dimension2Id", Item.Dimension2Id);

            if (Item.Dimension3Id == null || Item.Dimension3Id == 0)
                ParaDimension3Id = new SqlParameter("@Dimension3Id", DBNull.Value);
            else
                ParaDimension3Id = new SqlParameter("@Dimension3Id", Item.Dimension3Id);

            if (Item.Dimension4Id == null || Item.Dimension4Id == 0)
                ParaDimension4Id = new SqlParameter("@Dimension4Id", DBNull.Value);
            else
                ParaDimension4Id = new SqlParameter("@Dimension4Id", Item.Dimension4Id);

            ParaUserName = new SqlParameter("@UserName", LogVm.User);
        }
        
        public ReportHeaderCompanyDetailForJobConJumption GetReportHeaderCompanyDetail()
        {
            ReportHeaderCompanyDetailForJobConJumption ReportHeaderCompanyDetail = new ReportHeaderCompanyDetailForJobConJumption();

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var CompanyId = (int)System.Web.HttpContext.Current.Session["CompanyId"];

            Company Company = db.Company.Find(CompanyId);
            Division Division = db.Divisions.Find(DivisionId);


            ReportHeaderCompanyDetail.CompanyName = Company.CompanyName.Replace(System.Environment.NewLine, " ");
            ReportHeaderCompanyDetail.Address = Company.Address.Replace(System.Environment.NewLine, " ");
            
            if (Company.CityId != null)
                ReportHeaderCompanyDetail.CityName = db.City.Find(Company.CityId).CityName;
            else
                ReportHeaderCompanyDetail.CityName = "";
            ReportHeaderCompanyDetail.Phone = Company.Phone;
            ReportHeaderCompanyDetail.LogoBlob = Division.LogoBlob;

            return ReportHeaderCompanyDetail;
        }
        public IQueryable<ComboBoxResult> GetRecieveNoProcessAndPerson(int PersonId, int ProcessId, string term)
        {


            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var list = (from H in db.JobReceiveHeader
                        join P in db.Persons on H.JobWorkerId equals P.PersonID
                        join PP in db.Process on H.ProcessId equals PP.ProcessId into MainTable
                        from MainTab in MainTable.DefaultIfEmpty()
                        where MainTab.ProcessId == ProcessId && H.JobWorkerId == PersonId && H.SiteId == SiteId && H.DivisionId == DivisionId
                        orderby H.DocNo
                        select new ComboBoxResult
                        {
                            id = H.JobReceiveHeaderId.ToString(),
                            //text = b.Name + " | " + b.Code
                            text = H.DocNo
                        }
                      );
            return list;
        }
    }

    public class PersonWiseBalance
    {
        public string BalanceType { get; set; }
        public decimal Balance { get; set; }
    }
    public class PersonInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Location { get; set; }
    }
    public class PersonValue
    {
        public string Particulars { get; set; }
        public string Qty { get; set; }
        public int PersonID { get; set; }
        public string UnitName { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
    }
    public class MaterialBalanceUpDateViewModel
    {
        public string ReportType { get; set; }
        public string JobWorker { get; set; }
        public string AsOnDate { get; set; }
        public string Process { get; set; }
        public string CostCenter { get; set; }
        public string Product { get; set; }
        public string Dimension1Name { get; set; }// Size
        public string Dimension2Name { get; set; }// Style 
        public string Dimension3Name { get; set; } // Shade
        public string Dimension4Name { get; set; } // Fabric
        public string TextHidden { get; set; }
        public Boolean NegativeBalance { get; set; }
        // public ReportHeaderCompanyDetailForMaterial ReportHeaderCompanyDetail { get; set; }
        public ReportHeaderCompanyDetailForJobConJumption ReportHeaderCompanyDetail { get; set; }
    }
    public class StockHeadJobConjumptionViewModel 
    {
        public int StockHeaderId { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryNo { get; set; }
        public int? JobReceiveHeaderId { get; set; }
        public string Remark { get; set; }
        public int DocTypeId { get; set; }
        public int? StockProcessId { get; set; }
        public string ProcessName { get; set; }
        public int PersonId { get; set; }
        public int ProcessId { get; set; }
        public int? ProductId { get; set; }
        public int? Dimension1Id { get; set; }
        public int? Dimension2Id { get; set; }
        public int? Dimension3Id { get; set; }

        
    }
    public class StockLineAndProcessViewModel
    {
        public int StockHeaderId { get; set; }
        public string DocDate { get; set; }
        public string LineRemark { get; set; }
        public string DocNo { get; set; }
        public int? DocType { get; set; }
        public int StockProcessId { get; set; }
        public string ProcessName { get; set; }
        public int PersonId { get; set; }
        public int ProcessId { get; set; }
        public int ProductId { get; set; }
        public int? Dimension1Id { get; set; }
        public int? Dimension2Id { get; set; }
        public int? Dimension3Id { get; set; }
        public int? Dimension4Id { get; set; }
        public int CostCenterId { get; set; }
        public string JobWorker { get; set; }
        public string ProductName { get; set; }
        public string Dimension1Name { get; set; }// Size
        public string Dimension2Name { get; set; }// Style 
        public string Dimension3Name { get; set; } // Shade
        public string Dimension4Name { get; set; } // Fabric
        public string CostCenter { get; set; }
        public decimal BalQty { get; set; }
        public string UnitName { get; set; }
        public decimal ConsumeQty { get; set; }
        //  public StockHeadJobConjumptionViewModel Head { get; set; }
        public int DocTypeId { get; set; }
        public string EntryNo { get; set; }
        public string EntryDate { get; set; }
        public string Remark { get; set; }
        public int? JobReceiveHeaderId { get; set; }

    }
 

    public class MatereialBalanceJobWorkerViewModel
    {
        public int StockHeaderId { get; set; }
        public string DocDate { get; set; }
        public string Remark { get; set; }
        public string DocNo { get; set; }
        public int? DocType { get; set; }
        public int? StockProcessId { get; set; }
        public string ProcessName { get; set; }
        public int? PersonId { get; set; }
        public int? ProcessId { get; set; }
        public int? ProductId { get; set; }
        public int? Dimension1Id { get; set; }
        public int? Dimension2Id { get; set; }
        public int? Dimension3Id { get; set; }
        public int? Dimension4Id { get; set; }
        public int? CostCenterId { get; set; }
        public string JobWorker { get; set; }
        public string ProductName { get; set; }
        public string Dimension1Name { get; set; }// Size
        public string Dimension2Name { get; set; }// Style 
        public string Dimension3Name { get; set; } // Shade
        public string Dimension4Name { get; set; } // Fabric
        public string CostCenter { get; set; }
        public decimal BalQty { get; set; }
        public decimal ConsumeQty { get; set; }
        public string UnitName { get; set; }
        public string LotNo { get; set; }
        public string PlanNo { get; set; }
    }
    public class ReportHeaderCompanyDetailForJobConJumption 
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string Phone { get; set; }
        public string LogoBlob { get; set; }
    }
}
