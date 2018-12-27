using System.Collections.Generic;
using System.Linq;
using Data.Infrastructure;
using Model.ViewModel;
using System;
using Data.Models;
using System.Data.SqlClient;
using Model.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service
{
    public interface IDisplay_StockBalanceService : IDisposable

    {
        IQueryable<ComboBoxResult> GetFilterGroupOnFormat(string term, int? filter);
        IQueryable<ComboBoxResult> GetFilterShowBalanceFormat(string term, int? filter);
        IQueryable<ComboBoxResult> GetFilterProvision(string term, int? filter);
        // string SelectGroupOn();
        IQueryable<ValidEntryPoint> GetValidEntry(int productId, string DocNo);
        // bool IsValidData(int? ProductId, string DocNo);
        IEnumerable<StockBalancelOrderNoWiseViewModel> StockBalanceDetail(DisplayFilterSettings_StockBaance Settings);
    }

    public class Display_StockBalanceService : IDisplay_StockBalanceService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;



        public Display_StockBalanceService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public string SelectGroupOn()
        //{
        //    IEnumerable<GroupOnViewModel> GroupOnResultlist = db.Database.SqlQuery<GroupOnViewModel>("Web.GetGroupOnList").ToList();

        //    List<ComboBoxResult> ResultList = new List<ComboBoxResult>();
        //    foreach (var item in GroupOnResultlist)
        //    {
        //        ResultList.Add(new ComboBoxResult()
        //        {
        //            id = item.Dimension1Id.ToString(),
        //            text = item.Dimension1TypeName.ToString()
        //        });

        //    }
        //    var list = (from D in ResultList
        //                where D.id == "Product" && D.text == "Product"
        //                orderby D.text
        //                select new ComboBoxResult
        //                {
        //                    id = D.id,
        //                    text = D.text
        //                }
        //     );



        //    return list.ToString();

        //}
        public IQueryable<ValidEntryPoint> GetValidEntry(int productId, string DocNo)
        {
            SqlParameter ParaFromDate = new SqlParameter("@ProductId", productId);
            SqlParameter ParaToDate = new SqlParameter("@DocNo", DocNo);
            IEnumerable<ValidEntryPoint> GroupOnResultlist = db.Database.SqlQuery<ValidEntryPoint>("Web.GetValidEntryPoint", productId, DocNo).ToList();
            return GroupOnResultlist.AsQueryable();
        }
        public bool IsValidData(int DocTypeId, int DocId)
        {
            bool IsAllow = false;
            var MachingList = (from S in db.Stock
                               join Sh in db.StockHeader on S.StockHeaderId equals Sh.StockHeaderId
                               where Sh.DocTypeId == DocTypeId && (Sh.DocHeaderId == DocId || Sh.StockHeaderId == DocId)
                               select new
                               {
                                   ProductId = S.ProductId,
                                   StockHeaderId = S.StockHeaderId,
                                   DocHeaderId = Sh.DocHeaderId,
                                   DocTyeId = Sh.DocTypeId,
                                   DocNo = Sh.DocNo
                               }).FirstOrDefault();

            if (MachingList.DocHeaderId != null && MachingList.StockHeaderId != 0 && MachingList.DocTyeId != 0 && MachingList.DocNo != null)
            {
                IsAllow = true;
            }
            else
            {
                IsAllow = false;
            }

            return IsAllow;
        }
        public IQueryable<ComboBoxResult> GetFilterGroupOnFormat(string term, int? filter)
        {
            IEnumerable<GroupOnViewModel> GroupOnResultlist = db.Database.SqlQuery<GroupOnViewModel>("Web.GetGroupOnList").ToList();

            List<ComboBoxResult> ResultList = new List<ComboBoxResult>();

            //  ResultList.Add(new ComboBoxResult { id = GroupOnResultlist, text = DisplayStockGroupOnConstants.Process});
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Product, text = DisplayStockGroupOnConstants.Product });
            ////  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.LotNo, text = DisplayStockGroupOnConstants.LotNo });
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Godown, text = DisplayStockGroupOnConstants.Godown });
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension1, text = DisplayStockGroupOnConstants.Dimension1 });
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension2, text = DisplayStockGroupOnConstants.Dimension2 });
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension3, text = DisplayStockGroupOnConstants.Dimension3 });
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension4, text = DisplayStockGroupOnConstants.Dimension4 });
            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension2, text = ReportFormat.ProcessWise });
            foreach (var item in GroupOnResultlist)
            {
                ResultList.Add(new ComboBoxResult()
                {
                    id = item.Dimension1Id.ToString(),
                    text = item.Dimension1TypeName.ToString()
                });
            }
            var list = (from D in ResultList
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : (D.text.ToLower().Contains(term.ToLower())))
                        orderby D.text
                        select new ComboBoxResult
                        {
                            id = D.id,
                            text = D.text
                        }
             );
            return list.AsQueryable();

            //   return ResultList.AsQueryable();
        }

        public IQueryable<ComboBoxResult> GetFilterShowBalanceFormat(string term, int? filter)
        {
            List<ComboBoxResult> ResultList = new List<ComboBoxResult>();
            ResultList.Add(new ComboBoxResult { id = DisplayStockShowBalanceConstants.All, text = DisplayStockShowBalanceConstants.All });
            ResultList.Add(new ComboBoxResult { id = DisplayStockShowBalanceConstants.Zero, text = DisplayStockShowBalanceConstants.Zero });
            ResultList.Add(new ComboBoxResult { id = DisplayStockShowBalanceConstants.GreaterThanZero, text = DisplayStockShowBalanceConstants.GreaterThanZero });
            ResultList.Add(new ComboBoxResult { id = DisplayStockShowBalanceConstants.LessThanZero, text = DisplayStockShowBalanceConstants.LessThanZero });
            ResultList.Add(new ComboBoxResult { id = DisplayStockShowBalanceConstants.NotZero, text = DisplayStockShowBalanceConstants.NotZero });
            ResultList.Add(new ComboBoxResult { id = DisplayStockShowBalanceConstants.PeriodNegative, text = DisplayStockShowBalanceConstants.PeriodNegative });

            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension2, text = ReportFormat.ProcessWise });

            var list = (from D in ResultList
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : (D.text.ToLower().Contains(term.ToLower())))
                        orderby D.text
                        select new ComboBoxResult
                        {
                            id = D.id,
                            text = D.text
                        }
             );
            return list.AsQueryable();
        }

        public IQueryable<ComboBoxResult> GetFilterProvision(string term, int? filter)
        {
            List<ComboBoxResult> ResultList = new List<ComboBoxResult>();
            ResultList.Add(new ComboBoxResult { id = Provision.Stock, text = Provision.Stock });
            ResultList.Add(new ComboBoxResult { id = Provision.StockProcess, text = Provision.StockProcess });
            ResultList.Add(new ComboBoxResult { id = Provision.StockandStockInProcess, text = Provision.StockandStockInProcess });

            //  ResultList.Add(new ComboBoxResult { id = DisplayStockGroupOnConstants.Dimension2, text = ReportFormat.ProcessWise });

            var list = (from D in ResultList
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : (D.text.ToLower().Contains(term.ToLower())))
                        orderby D.text
                        select new ComboBoxResult
                        {
                            id = D.id,
                            text = D.text
                        }
             );
            return list.AsQueryable();

        }

        public IEnumerable<StockBalancelOrderNoWiseViewModel> StockBalanceDetail(DisplayFilterSettings_StockBaance Settings)
        {
            var FromDateSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var ProductNatureSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ProductNature" select H).FirstOrDefault();
            var ProductTypeSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ProductType" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProcessSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Process" select H).FirstOrDefault();
            var GodownSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Godown" select H).FirstOrDefault();
            var PersonSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Name" select H).FirstOrDefault();
            var ProductSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "SiteIds" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "DivisionIds" select H).FirstOrDefault();
            var ShowBalanceSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ShowBalance" select H).FirstOrDefault();
            var GroupOnSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "GroupOn" select H).FirstOrDefault();
            var ShowOpeningSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ShowOpening" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var LotNoSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "LotNo" select H).FirstOrDefault();
            var PlanNoSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "PlanNo" select H).FirstOrDefault();
            var Dimension3Setting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Dimension3TypeName" select H).FirstOrDefault();
            var Dimension2Setting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Dimension2TypeName" select H).FirstOrDefault();
            var Dimension1Setting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Dimension1TypeName" select H).FirstOrDefault();
            var Dimension4Setting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Dimension4TypeName" select H).FirstOrDefault();
            var ProvisionSettings = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Provision" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string ProductNature = ProductNatureSetting.Value;
            string ProductType = ProductTypeSetting.Value;
            string ProductCategory = ProductCategorySetting.Value;
            string LotNo = LotNoSetting.Value;
            string PlanNo = PlanNoSetting.Value;
            string Godown = GodownSetting.Value;
            string Name = PersonSetting.Value;
            string Product = ProductSetting.Value;
            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string ShowBalance = ShowBalanceSetting.Value;
            string GroupOn = GroupOnSetting.Value;
            bool ShowOpening = Convert.ToBoolean(ShowOpeningSetting.Value);
            string ProductGroup = ProductGroupSetting.Value;
            string Dimension3 = Dimension3Setting.Value;
            string Dimension2 = Dimension2Setting.Value;
            string Dimension1 = Dimension1Setting.Value;
            string Dimension4 = Dimension4Setting.Value;
            string Process = ProcessSetting.Value;
            string Provision = ProvisionSettings.Value;
            string TextHidden = TextHiddenSetting.Value;
            int ShowO = 0;
            #region Test
            if (ShowOpening == true)
            {
                ShowO = 1;
            }
            else
            {
                ShowO = 0;
            }
            SqlParameter ParaProductType = new SqlParameter("@ProductType", !string.IsNullOrEmpty(ProductType) ? ProductType : (object)DBNull.Value);
            SqlParameter ParaSite = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter ParaFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter ParaToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter ParaGroupOn = new SqlParameter("@GroupOn", !string.IsNullOrEmpty(GroupOn) ? GroupOn : (object)DBNull.Value);
            SqlParameter ParaShowBalance = new SqlParameter("@ShowBalance", !string.IsNullOrEmpty(ShowBalance) ? ShowBalance : (object)DBNull.Value);
            SqlParameter ParaProduct = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
            SqlParameter ParaGodown = new SqlParameter("@Godown", !string.IsNullOrEmpty(Godown) ? Godown : (object)DBNull.Value);
            SqlParameter ParaPerson = new SqlParameter("@PersonId", !string.IsNullOrEmpty(Name) ? Name : (object)DBNull.Value);
            SqlParameter ParaProcess = new SqlParameter("@Process", !string.IsNullOrEmpty(Process) ? Process : (object)DBNull.Value);
            SqlParameter ParaDimension1 = new SqlParameter("@Dimension1", !string.IsNullOrEmpty(Dimension1) ? Dimension1 : (object)DBNull.Value);
            SqlParameter ParaDimension2 = new SqlParameter("@Dimension2", !string.IsNullOrEmpty(Dimension2) ? Dimension2 : (object)DBNull.Value);
            SqlParameter ParaProductNature = new SqlParameter("@ProductNature", !string.IsNullOrEmpty(ProductNature) ? ProductNature : (object)DBNull.Value);
            SqlParameter ParaProductGroup = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
            SqlParameter ParaProductCustomGroup = new SqlParameter("@ProductCustomGroup", DBNull.Value); //ProductCustomGroup
            SqlParameter ParaDimension3 = new SqlParameter("@Dimension3", !string.IsNullOrEmpty(Dimension3) ? Dimension3 : (object)DBNull.Value);
            SqlParameter ParaDimension4 = new SqlParameter("@Dimension4", !string.IsNullOrEmpty(Dimension4) ? Dimension4 : (object)DBNull.Value);
            SqlParameter ParaLotNo = new SqlParameter("@Lot", !string.IsNullOrEmpty(LotNo) ? LotNo : (object)DBNull.Value);
            SqlParameter ParaPlanNo = new SqlParameter("@PlanNo", !string.IsNullOrEmpty(PlanNo) ? PlanNo : (object)DBNull.Value);
            SqlParameter ParaShowOpening = new SqlParameter("@ShowOpening", Convert.ToString(ShowO));
            SqlParameter ParaTableName = new SqlParameter("@TableName", Provision);
            SqlParameter ParaDivision = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter ParaReporttype = new SqlParameter("@ReportType", DBNull.Value); //ProductCustomGroup
            SqlParameter ParaProductDivision = new SqlParameter("@ProductDivisionId", DBNull.Value); //ProductCustomGroup
            #endregion
            IEnumerable<StockBalancelOrderNoWiseViewModel> TrialBalanceSummaryList = null;
            if (Product != null || Process != null || Dimension2 != null || Dimension1 != null || Dimension4 != null || Godown != null || Dimension3 != null)
            {
                // TrialBalanceSummaryList = db.Database.SqlQuery<StockBalancelOrderNoWiseViewModel>("Web.spStockLedger_New  @Site,@Division,@FromDate,@ToDate,@GroupOn,@Product,@Godown,@Process,@Dimension1,@Dimension2,@Dimension3,@Dimension4,@PersonId,@ProductType,@ShowBalance,@ProductNature,@ProductGroup,@Lot ", ParaSite,ParaDivision,ParaFromDate,ParaToDate,ParaGroupOn,ParaProduct,ParaGodown,ParaProcess, ParaDimension1, ParaDimension2,ParaDimension3,ParaDimension4,ParaPerson,ParaProductType,ParaShowBalance,ParaProductNature,ParaProductGroup,ParaLotNo).ToList();
                TrialBalanceSummaryList = db.Database.SqlQuery<StockBalancelOrderNoWiseViewModel>("Web.spStockLedger_New  @Site,@Division, @FromDate, @ToDate, @GroupOn, @Product, @Godown, @Process, @Dimension1, @Dimension2, @Dimension3, @Dimension4,@ProductDivisionId, @PersonId, @ProductType, @ShowBalance, @ProductNature, @ProductGroup,@ProductCustomGroup,@ReportType, @Lot, @PlanNo", ParaSite, ParaDivision, ParaFromDate, ParaToDate, ParaGroupOn, ParaProduct, ParaGodown, ParaProcess, ParaDimension1, ParaDimension2, ParaDimension3, ParaDimension4, ParaProductDivision, ParaPerson, ParaProductType, ParaShowBalance, ParaProductNature, ParaProductGroup, ParaProductCustomGroup, ParaReporttype, ParaLotNo, ParaPlanNo).ToList();

                Decimal NetBalance = TrialBalanceSummaryList.LastOrDefault().Balance;
                foreach (var item in TrialBalanceSummaryList)
                    item.NetBalance = NetBalance;
            }
            else
            {
                if (Provision == "Stock and StockIn Process")
                {
                    IEnumerable<StockBalancelOrderNoWiseViewModel> TrialBalanceSummaryList1 = null;
                    IEnumerable<StockBalancelOrderNoWiseViewModel> TrialBalanceSummaryList2 = null;
                    SqlParameter ParaTableName1 = new SqlParameter("@TableName", "Stock");
                    TrialBalanceSummaryList1 = db.Database.SqlQuery<StockBalancelOrderNoWiseViewModel>("Web.spDisplayStockInHandAndStockProcessDisplay  @ProductType, @Site, @FromDate, @ToDate, @GroupOn, @ShowBalance, @Godown, @Process,@Product, @Dimension1, @Dimension2, @ProductNature, @ProductGroup, @ProductCustomGroup,@Dimension3, @Dimension4, @ShowOpening ,@Lot,@PlanNo,@TableName", ParaProductType, ParaSite, ParaFromDate, ParaToDate, ParaGroupOn, ParaShowBalance, ParaGodown, ParaProcess, ParaProduct, ParaDimension1, ParaDimension2, ParaProductNature, ParaProductGroup, ParaProductCustomGroup, ParaDimension3, ParaDimension4, ParaShowOpening, ParaLotNo, ParaPlanNo, ParaTableName1).ToList();
                    SqlParameter ParaTableName2 = new SqlParameter("@TableName", "Stock in Process");

                    SqlParameter ParaProductType1 = new SqlParameter("@ProductType", !string.IsNullOrEmpty(ProductType) ? ProductType : (object)DBNull.Value);
                    SqlParameter ParaSite1 = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
                    SqlParameter ParaFromDate1 = new SqlParameter("@FromDate", FromDate);
                    SqlParameter ParaToDate1 = new SqlParameter("@ToDate", ToDate);
                    SqlParameter ParaGroupOn1 = new SqlParameter("@GroupOn", !string.IsNullOrEmpty(GroupOn) ? GroupOn : (object)DBNull.Value);
                    SqlParameter ParaShowBalance1 = new SqlParameter("@ShowBalance", !string.IsNullOrEmpty(ShowBalance) ? ShowBalance : (object)DBNull.Value);
                    SqlParameter ParaProduct1 = new SqlParameter("@Product", !string.IsNullOrEmpty(Product) ? Product : (object)DBNull.Value);
                    SqlParameter ParaGodown1 = new SqlParameter("@Godown", !string.IsNullOrEmpty(Godown) ? Godown : (object)DBNull.Value);
                    SqlParameter ParaProcess1 = new SqlParameter("@Process", !string.IsNullOrEmpty(Process) ? Process : (object)DBNull.Value);
                    SqlParameter ParaDimension11 = new SqlParameter("@Dimension1", !string.IsNullOrEmpty(Dimension1) ? Dimension1 : (object)DBNull.Value);
                    SqlParameter ParaDimension21 = new SqlParameter("@Dimension2", !string.IsNullOrEmpty(Dimension2) ? Dimension2 : (object)DBNull.Value);
                    SqlParameter ParaProductNature1 = new SqlParameter("@ProductNature", !string.IsNullOrEmpty(ProductNature) ? ProductNature : (object)DBNull.Value);
                    SqlParameter ParaProductGroup1 = new SqlParameter("@ProductGroup", !string.IsNullOrEmpty(ProductGroup) ? ProductGroup : (object)DBNull.Value);
                    SqlParameter ParaProductCustomGroup1 = new SqlParameter("@ProductCustomGroup", DBNull.Value); //ProductCustomGroup
                    SqlParameter ParaDimension31 = new SqlParameter("@Dimension3", !string.IsNullOrEmpty(Dimension3) ? Dimension3 : (object)DBNull.Value);
                    SqlParameter ParaDimension41 = new SqlParameter("@Dimension4", !string.IsNullOrEmpty(Dimension4) ? Dimension4 : (object)DBNull.Value);
                    SqlParameter ParaLotNo1 = new SqlParameter("@Lot", !string.IsNullOrEmpty(LotNo) ? LotNo : (object)DBNull.Value);
                    SqlParameter ParaPlanNo1 = new SqlParameter("@PlanNo", !string.IsNullOrEmpty(PlanNo) ? PlanNo : (object)DBNull.Value);
                    SqlParameter ParaShowOpening1 = new SqlParameter("@ShowOpening", Convert.ToString(ShowO));
                    SqlParameter ParaDivision1 = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);



                    TrialBalanceSummaryList2 = db.Database.SqlQuery<StockBalancelOrderNoWiseViewModel>("Web.spDisplayStockInHandAndStockProcessDisplay  @ProductType, @Site, @FromDate, @ToDate, @GroupOn, @ShowBalance, @Godown, @Process,@Product, @Dimension1, @Dimension2, @ProductNature, @ProductGroup, @ProductCustomGroup,@Dimension3, @Dimension4, @ShowOpening ,@Lot,@PlanNo,@TableName", ParaProductType1, ParaSite1, ParaFromDate1, ParaToDate1, ParaGroupOn1, ParaShowBalance1, ParaGodown1, ParaProcess1, ParaProduct1, ParaDimension11, ParaDimension21, ParaProductNature1, ParaProductGroup1, ParaProductCustomGroup1, ParaDimension31, ParaDimension41, ParaShowOpening1, ParaLotNo1, ParaPlanNo1, ParaTableName2).ToList();
                    TrialBalanceSummaryList = TrialBalanceSummaryList1.Union(TrialBalanceSummaryList2);
                }
                else
                {
                    TrialBalanceSummaryList = db.Database.SqlQuery<StockBalancelOrderNoWiseViewModel>("Web.spDisplayStockInHandAndStockProcessDisplay  @ProductType, @Site, @FromDate, @ToDate, @GroupOn, @ShowBalance, @Godown, @Process,@Product, @Dimension1, @Dimension2, @ProductNature, @ProductGroup, @ProductCustomGroup,@Dimension3, @Dimension4, @ShowOpening ,@Lot,@PlanNo,@TableName", ParaProductType, ParaSite, ParaFromDate, ParaToDate, ParaGroupOn, ParaShowBalance, ParaGodown, ParaProcess, ParaProduct, ParaDimension1, ParaDimension2, ParaProductNature, ParaProductGroup, ParaProductCustomGroup, ParaDimension3, ParaDimension4, ParaShowOpening, ParaLotNo, ParaPlanNo, ParaTableName).ToList();
                }

            }
            return TrialBalanceSummaryList;
        }


        public void Dispose()
        {
        }
    }

    public class Display_StockBalanceViewModel
    {

        public string ReportType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ProductNature { get; set; }
        public string ProductType { get; set; }
        public string Product { get; set; }

        public string ProductCategory { get; set; }
        public string ProductGroup { get; set; }
        public string Godown { get; set; }
        public string Name { get; set; }
        public string SiteIds { get; set; }
        public string DivisionIds { get; set; }
        public string Process { get; set; }
        public string ShowBalance { get; set; }
        public string GroupOn { get; set; }

        [Display(Name = "Show Opening")]
        public Boolean ShowOpening { get; set; }
        public string Provision { get; set; }
        public string TextHidden { get; set; }
        public ReportHeaderCompanyDetail ReportHeaderCompanyDetail { get; set; }

        public string Dimension1Name { get; set; }
        public string Dimension2Name { get; set; }
        public string Dimension3Name { get; set; }
        public string Dimension4Name { get; set; }
        public string LotNo { get; set; }
        public string PlanNo { get; set; }
        public string ErrorMsg { get; set; }


    }


    [Serializable()]
    public class DisplayFilterSettings_StockBaance
    {
        public string ReportType { get; set; }
        public List<DisplayFilterParameters_StockBaance> DisplayFilterParameters { get; set; }
    }

    [Serializable()]
    public class DisplayFilterParameters_StockBaance
    {
        public string ParameterName { get; set; }
        public bool IsApplicable { get; set; }
        public string Value { get; set; }

    }

    public class DisplayStockGroupOnConstants
    {
        public const string Godown = "Godown";
        public const string Process = "Process";
        public const string Product = "Product";
        public const string Dimension1 = "Size";
        public const string Dimension2 = "Style";
        public const string Dimension3 = "Shade";
        public const string Dimension4 = "Fabric";
        public const string LotNo = "LotNo";
        public const string PlanNo = "PlanNo";
        public const string Person = "Person";
        public const string ErrorMsg = "Record not found in Database, so not move to entry page";
    }
    public class GroupOnViewModel
    {
        public string Dimension1Id { get; set; }
        public string Dimension2Id { get; set; }
        public string Dimension3Id { get; set; }
        public string Dimension4Id { get; set; }
        public string Godown { get; set; }
        public string Process { get; set; }
        public string Product { get; set; }
        public string LotNo { get; set; }
        public string PlanNo { get; set; }
        public string Dimension1TypeName { get; set; }
        public string Dimension2TypeName { get; set; }
        public string Dimension3TypeName { get; set; }
        public string Dimension4TypeName { get; set; }


    }
    public class DisplayStockShowBalanceConstants
    {
        public const string NotZero = "Not Zero";
        public const string Zero = "Zero";
        public const string GreaterThanZero = "Greater Than Zero";
        public const string LessThanZero = "Less Than Zero";
        public const string PeriodNegative = "Period Negative";
        public const string All = "All";
    }
    public class ReportFormat_StockBaance
    {
        public const string JobWorkerWise = "Job Worker Wise Summary";
        public const string MonthWise = "Month Wise Summary";
        public const string ProdTypeWise = "Product Type Wise Summary";
        public const string ProductNatureWiseSummary = "Product Nature Wise Summary";
        public const string OrderNoWise = "Order No Wise";
        public const string ProcessWise = "Process Wise Symmary";
        public const string Product = "Product";
        public const string Dimension1Caption = "Size";
        public const string Dimension2Caption = "Style";
        public const string Dimension3Caption = "Shade";
        public const string Dimension4Caption = "Fabric";
        public const string LotNo = "LotNo";
        public const string PlanNo = "PlanNo";
        public const string Person = "Person";
    }
    public class Provision
    {
        public const string Stock = "Stock";
        public const string StockProcess = "Stock in Process";
        public const string StockandStockInProcess = "Stock and StockIn Process";
    }
    public class StockBalancelOrderNoWiseViewModel
    {
        public Int64 Id { get; set; }
        public int SiteId { get; set; }
        public int DivisionId { get; set; }
        public string LotNo { get; set; }
        public string PlanNo { get; set; }
        public string ProductName { get; set; }
        public string ProductGroupName { get; set; }
        public int? ProductId { get; set; }
        public string UnitName { get; set; }
        public int? UnitDecimalPlaces { get; set; }
        //   public byte? UnitDecimalPlaces { get; set; }
        public string SiteName { get; set; }
        public string DivisionName { get; set; }
        public string GodownLocation { get; set; }
        public int? GodownId { get; set; }
        public int? ProcessId { get; set; }
        public int? Dimension1Id { get; set; }
        public string Dimension1Name { get; set; }
        public int? Dimension2Id { get; set; }
        public string Dimension2Name { get; set; }
        public int? Dimension3Id { get; set; }
        public string Dimension3Name { get; set; }
        public int? Dimension4Id { get; set; }
        public string Dimension4Name { get; set; }
        public string ProcessName { get; set; }
        public string Dimension1TypeName { get; set; }
        public string Dimension2TypeName { get; set; }
        public decimal Opening { get; set; }
        public decimal RecQty { get; set; }
        public string ShowOpening { get; set; }
        public decimal IssQty { get; set; }
        public decimal BalQty { get; set; }
        public decimal Balance { get; set; }
        public decimal NetBalance { get; set; }
        public string ReportName { get; set; }
        public string ReportTitle { get; set; }

        public string SubReportProcList { get; set; }

        public string Name { get; set; }

        public int? PersonId { get; set; }

        public int Receive { get; set; }

        public int ProductType { get; set; }

        public string GroupOn { get; set; }

        public string Dimension1Caption { get; set; }
        public string Dimension2Caption { get; set; }
        public string Dimension3Caption { get; set; }
        public string Dimension4Caption { get; set; }
        public string PartyName { get; set; }
        public string DocNo { get; set; }
        public string DocDate { get; set; }
        public int? DocTypeId { get; set; }
        public int? DocHeaderId { get; set; }
        public int? StockheaderId { get; set; }
        public int ProductNatureId { get; set; }
        public string ProductNatureName { get; set; }
        public string DocumentTypeName { get; set; }
    }
    public class ValidEntryPoint
    {
        public int? StockHeaderId { get; set; }
        public int DocHeaderId { get; set; }
        public int DocTypeId { get; set; }
        public string DocNo { get; set; }
        public int ProductId { get; set; }

    }

}

