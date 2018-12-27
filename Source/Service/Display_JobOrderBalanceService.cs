using System.Collections.Generic;
using System.Linq;
using Data.Infrastructure;
using Model.ViewModel;
using System;
using Data.Models;
using System.Data.SqlClient;
using Model.ViewModels;
using System.Text.RegularExpressions;

namespace Service
{
    public interface IDisplay_JobOrderBalanceService : IDisposable
    {
        IQueryable<ComboBoxResult> GetFilterFormat(string term, int? filter);

        IEnumerable<JobOrderBalancelOrderNoWiseViewModel> JobOrderBalanceDetail(DisplayFilterSettings Settings);
    }

    public class Display_JobOrderBalanceService : IDisplay_JobOrderBalanceService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;



        public Display_JobOrderBalanceService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<ComboBoxResult> GetFilterFormat(string term, int? filter)
        {
            List<ComboBoxResult> ResultList = new List<ComboBoxResult>();
            ResultList.Add(new ComboBoxResult { id = ReportFormat.JobWorkerWise, text = ReportFormat.JobWorkerWise });
            ResultList.Add(new ComboBoxResult { id = ReportFormat.MonthWise, text = ReportFormat.MonthWise });
            ResultList.Add(new ComboBoxResult { id = ReportFormat.ProdTypeWise, text = ReportFormat.ProdTypeWise });
            ResultList.Add(new ComboBoxResult { id = ReportFormat.ProductNatureWiseSummary, text = ReportFormat.ProductNatureWiseSummary });
            ResultList.Add(new ComboBoxResult { id = ReportFormat.OrderNoWise, text = ReportFormat.OrderNoWise });
            ResultList.Add(new ComboBoxResult { id = ReportFormat.ProcessWise, text = ReportFormat.ProcessWise });

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





        public IEnumerable<JobOrderBalancelOrderNoWiseViewModel> JobOrderBalanceDetail(DisplayFilterSettings Settings)
        {

            var FormatSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Format" select H).FirstOrDefault();
            var SiteSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Site" select H).FirstOrDefault();
            var DivisionSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Division" select H).FirstOrDefault();
            var FromDateSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var ProductNatureSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ProductNature" select H).FirstOrDefault();
            var ProductTypeSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "ProductType" select H).FirstOrDefault();
            var JobWorkerSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "JobWorker" select H).FirstOrDefault();
            var ProcessSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "Process" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in Settings.DisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();



            string Format = FormatSetting.Value;
            string SiteId = SiteSetting.Value;
            string DivisionId = DivisionSetting.Value;
            string FromDate = FromDateSetting.Value;
            string ToDate = ToDateSetting.Value;
            string ProductNature = ProductNatureSetting.Value;
            string ProductType = ProductTypeSetting.Value;
            string JobWorker = JobWorkerSetting.Value;
            string Process = ProcessSetting.Value;
            string TextHidden = TextHiddenSetting.Value;
            string FormatParaMeter = "";
            FormatParaMeter = Regex.Replace(Format, @"\s", "");
            SqlParameter SqlParameterFormat = new SqlParameter("@Formate", !string.IsNullOrEmpty(FormatParaMeter) ? FormatParaMeter : (object)DBNull.Value);
            SqlParameter SqlParameterSiteId = new SqlParameter("@Site", !string.IsNullOrEmpty(SiteId) ? SiteId : (object)DBNull.Value);
            SqlParameter SqlParameterDivisionId = new SqlParameter("@Division", !string.IsNullOrEmpty(DivisionId) ? DivisionId : (object)DBNull.Value);
            SqlParameter SqlParameterFromDate = new SqlParameter("@FromDate", FromDate);
            SqlParameter SqlParameterToDate = new SqlParameter("@ToDate", ToDate);
            SqlParameter SqlParameterProductNature = new SqlParameter("@ProductNature", !string.IsNullOrEmpty(ProductNature) ? ProductNature : (object)DBNull.Value);
            SqlParameter SqlParameterProductType = new SqlParameter("@ProductType", !string.IsNullOrEmpty(ProductType) ? ProductType : (object)DBNull.Value);
            SqlParameter SqlParameterJobWorker = new SqlParameter("@JobWorker", !string.IsNullOrEmpty(JobWorker) ? JobWorker : (object)DBNull.Value);
            SqlParameter SqlParameterProcess = new SqlParameter("@Process", !string.IsNullOrEmpty(Process) ? Process : (object)DBNull.Value);
            IEnumerable<JobOrderBalancelOrderNoWiseViewModel> TrialBalanceSummaryList = db.Database.SqlQuery<JobOrderBalancelOrderNoWiseViewModel>("Web.SpDisplayJobOrderBalance @Formate,@FromDate,@ToDate,@ProductNature,@ProductType,@Jobworker,@Site,@Division,@Process", SqlParameterFormat, SqlParameterFromDate, SqlParameterToDate, SqlParameterProductNature, SqlParameterProductType, SqlParameterJobWorker, SqlParameterSiteId, SqlParameterDivisionId, SqlParameterProcess).ToList();
            return TrialBalanceSummaryList;

        }


        public void Dispose()
        {
        }
    }

    public class Display_JobOrderBalanceViewModel
    {
        public string Format { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ProductNature { get; set; }
        public string ProductType { get; set; }
        public string JobWorker { get; set; }
        public string SiteIds { get; set; }
        public string DivisionIds { get; set; }
        public string Process { get; set; }
        public string TextHidden { get; set; }
        public Boolean Opening { get; set; }
        public ReportHeaderCompanyDetail ReportHeaderCompanyDetail { get; set; }


    }

    [Serializable()]
    public class DisplayFilterSettings
    {
        public string Format { get; set; }
        public List<DisplayFilterParameters> DisplayFilterParameters { get; set; }
    }

    [Serializable()]
    public class DisplayFilterParameters
    {
        public string ParameterName { get; set; }
        public bool IsApplicable { get; set; }
        public string Value { get; set; }
    }

    public class ReportFormat
    {
        public const string JobWorkerWise = "Job Worker Wise Summary";
        public const string MonthWise = "Month Wise Summary";
        public const string ProdTypeWise = "Product Type Wise Summary";
        public const string ProductNatureWiseSummary = "Product Nature Wise Summary";
        public const string OrderNoWise = "Order No Wise";
        public const string ProcessWise = "Process Wise Summary";
    }
    public class JobOrderBalancelOrderNoWiseViewModel
    {
        public int? JobOrderHeaderId { get; set; }
        public int? DocTypeId { get; set; }
        public string DocNo { get; set; }
        public string DocDate { get; set; }

        public string DueDate { get; set; }

        public string SupplierName { get; set; }

        public string ProductName { get; set; }

        public int? ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }

        public int? ProductNatureId { get; set; }
        public string ProductNatureName { get; set; }
        public string Month { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? JobWorkerId { get; set; }
        public string Specification { get; set; }

        public int? ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string UnitName { get; set; }
        public string DealUnitName { get; set; }

        public decimal? OrderQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? BalanceDealQty { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string Format { get; set; }
        public string Dimension1 { get; set; }
        public string Dimension2 { get; set; }
        public string Dimension3 { get; set; }
        public string Dimension4 { get; set; }
        public string ProdOrderNo { get; set; }
        public string LotNo { get; set; }

        public decimal? Rate { get; set; }


    }

}

