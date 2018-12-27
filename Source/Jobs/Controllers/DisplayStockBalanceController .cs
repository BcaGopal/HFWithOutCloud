using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using System.Data;
using Model.ViewModels;
using Jobs.Helpers;
using Core.Common;
using System.Text;

namespace Jobs.Controllers
{

    [Authorize]
    public class DisplayStockBalanceController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IDisplay_StockBalanceService _StockBalanceService;
        //  IStockInHandSettingService _StockInHandSettingService;
        IStockInHandService _StockInHandService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        IStockInHandSettingService _StockInHandSettingService;
        public DisplayStockBalanceController(IDisplay_StockBalanceService Display_StockBalanceService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _StockBalanceService = Display_StockBalanceService;
            _exception = exec;
            _unitOfWork = unitOfWork;
        }
        //Add for GroupOn List Bind
        //private void PrepareViewBag(int MenuId) //string Routeid
        //{

        //    List<SelectListItem> temp = new List<SelectListItem>();
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Dimension1Caption, Value = StockInHandGroupOnConstants.Dimension1 });
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Dimension2Caption, Value = StockInHandGroupOnConstants.Dimension2 });
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Dimension3Caption, Value = StockInHandGroupOnConstants.Dimension3 });
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Dimension4Caption, Value = StockInHandGroupOnConstants.Dimension4 });
        //  /*  if (Routeid == "Stock")
        //    {
        //        temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Godown, Value = StockInHandGroupOnConstants.Godown });
        //    }
        //    else
        //    {
        //        temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Person, Value = StockInHandGroupOnConstants.Person });
        //    }*/
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.LotNo, Value = StockInHandGroupOnConstants.LotNo });
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Process, Value = StockInHandGroupOnConstants.Process });
        //    temp.Add(new SelectListItem { Text = StockInHandGroupOnConstants.Product, Value = StockInHandGroupOnConstants.Product });
        //    ViewBag.GroupOnList = new SelectList(temp, "Value", "Text");

        //    List<SelectListItem> shwBal = new List<SelectListItem>();
        //    shwBal.Add(new SelectListItem { Text = StockInHandShowBalanceConstants.GreaterThanZero, Value = StockInHandShowBalanceConstants.GreaterThanZero });
        //    shwBal.Add(new SelectListItem { Text = StockInHandShowBalanceConstants.LessThanZero, Value = StockInHandShowBalanceConstants.LessThanZero });
        //    shwBal.Add(new SelectListItem { Text = StockInHandShowBalanceConstants.NotZero, Value = StockInHandShowBalanceConstants.NotZero });
        //    shwBal.Add(new SelectListItem { Text = StockInHandShowBalanceConstants.PeriodNegative, Value = StockInHandShowBalanceConstants.PeriodNegative });
        //    shwBal.Add(new SelectListItem { Text = StockInHandShowBalanceConstants.Zero, Value = StockInHandShowBalanceConstants.Zero });
        //    shwBal.Add(new SelectListItem { Text = StockInHandShowBalanceConstants.All, Value = StockInHandShowBalanceConstants.All });
        //    ViewBag.ShowBalanceList = new SelectList(shwBal, "Value", "Text");
        //}
        //

        [HttpGet]
        public ActionResult DisplayStockBalance(int MenuId)//int? ProductTypeId, string Route,string ControllerName
        {

            Menu Menu = new MenuService(_unitOfWork).Find(MenuId);
            Display_StockBalanceViewModel vm = new Display_StockBalanceViewModel();
            // StockInHandSetting vn = new StockInHandSetting();
            //    string RoutId=Menu.RouteId;
            //     ViewBag.RId = RoutId;



            // ViewBag.id = ProductTypeId;
            //  System.Web.HttpContext.Current.Session["ProductTypeId"] = ProductTypeId;
            //  System.Web.HttpContext.Current.Session["Route"] = Route;
            List<string> GrList = new List<string> { "Product", "Dimension1", "Dimension2", "Dimension3", "Dimension4", "Process", "Godown" };
            // IEnumerable<GroupOnViewModel> GroupOnResultlist = db.Database.SqlQuery<GroupOnViewModel>("Web.GetGroupOnList").ToList();
            //    PrepareViewBag(MenuId);
            System.Web.HttpContext.Current.Session["SettingList"] = new List<DisplayFilterSettings_StockBaance>();
            System.Web.HttpContext.Current.Session["CurrentSetting"] = new DisplayFilterSettings_StockBaance();
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            vm.FromDate = "01/Apr/" + DateTime.Now.Date.Year.ToString();
            vm.ToDate = DateTime.Now.Date.ToString("dd/MMM/yyyy");
            vm.SiteIds = SiteId.ToString();
            vm.DivisionIds = DivisionId.ToString();
            vm.GroupOn = String.Join(",", (from p in GrList select p).Distinct()).ToString();
            vm.ShowBalance = DisplayStockShowBalanceConstants.All;
            vm.Provision = Provision.Stock;
            vm.ShowOpening = true;
            // vm.ReportType = ReportFormat_StockBaance.ProcessWise;
            vm.ReportHeaderCompanyDetail = new GridReportService().GetReportHeaderCompanyDetail();
            string UserName = User.Identity.Name;
            return View("DisplayStockBalance", vm);

        }


        [HandleError()]
        public ActionResult DisplayStockBalanceFill(Display_StockBalanceViewModel vm)
        {


            // List<Display_StockBalanceViewModel> DisplayType = new List<Display_StockBalanceViewModel>();
            string UserName = User.Identity.Name;
            //  int  id =Convert.ToInt32(vm.ProductType);

            //  var settings = new StockInHandSettingService(_unitOfWork).GetTrailBalanceSettingforDSB(User.Identity.Name, id);
            // //  string RouteId = settings.TableName;
            // vm.GroupOn = settings.GroupOn;
            // vm.ShowOpening = Convert.ToBoolean(settings.ShowOpening == true ? 1 : 0);
            // vm.ShowOpening = ShowOpening;
            //    List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            DisplayFilterSettings_StockBaance SettingParameter = SetCurrentParameterSettings(vm);
            IEnumerable<StockBalancelOrderNoWiseViewModel> StockBalanceJobWorkerWise = _StockBalanceService.StockBalanceDetail(SettingParameter);
            if (StockBalanceJobWorkerWise != null)
            {
                JsonResult json = Json(new { Success = true, Data = StockBalanceJobWorkerWise.ToList() }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        public DisplayFilterSettings_StockBaance SetCurrentParameterSettings(Display_StockBalanceViewModel vm)
        {
            DisplayFilterSettings_StockBaance DisplayFilterSettings = new DisplayFilterSettings_StockBaance();


            DisplayFilterParameters_StockBaance FromDateParameter = new DisplayFilterParameters_StockBaance();
            FromDateParameter.ParameterName = "FromDate";
            FromDateParameter.Value = vm.FromDate;
            FromDateParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ToDateParameter = new DisplayFilterParameters_StockBaance();
            ToDateParameter.ParameterName = "ToDate";
            ToDateParameter.Value = vm.ToDate;
            ToDateParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ProductNatureParameter = new DisplayFilterParameters_StockBaance();
            ProductNatureParameter.ParameterName = "ProductNature";
            ProductNatureParameter.Value = vm.ProductNature;
            ProductNatureParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ProductTypeParameter = new DisplayFilterParameters_StockBaance();
            ProductTypeParameter.ParameterName = "ProductType";
            ProductTypeParameter.Value = vm.ProductType;
            ProductTypeParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ProductCategoryParameter = new DisplayFilterParameters_StockBaance();
            ProductCategoryParameter.ParameterName = "ProductCategory";
            ProductCategoryParameter.Value = vm.ProductCategory;
            ProductCategoryParameter.IsApplicable = true;



            DisplayFilterParameters_StockBaance ProductGorupParameter = new DisplayFilterParameters_StockBaance();
            ProductGorupParameter.ParameterName = "ProductGroup";
            ProductGorupParameter.Value = vm.ProductGroup;
            ProductGorupParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance GodownParameter = new DisplayFilterParameters_StockBaance();
            GodownParameter.ParameterName = "Godown";
            GodownParameter.Value = vm.Godown;
            GodownParameter.IsApplicable = true;
            DisplayFilterParameters_StockBaance PersonParameter = new DisplayFilterParameters_StockBaance();
            PersonParameter.ParameterName = "Name";
            PersonParameter.Value = vm.Name;
            PersonParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance SiteParameter = new DisplayFilterParameters_StockBaance();
            SiteParameter.ParameterName = "SiteIds";
            SiteParameter.Value = vm.SiteIds;
            SiteParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance DivisionParameter = new DisplayFilterParameters_StockBaance();
            DivisionParameter.ParameterName = "DivisionIds";
            DivisionParameter.Value = vm.DivisionIds;
            DivisionParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ProductParameter = new DisplayFilterParameters_StockBaance();
            ProductParameter.ParameterName = "Product";
            ProductParameter.Value = vm.Product;
            ProductParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ShowBalanceParameter = new DisplayFilterParameters_StockBaance();
            ShowBalanceParameter.ParameterName = "ShowBalance";
            ShowBalanceParameter.Value = vm.ShowBalance;
            ShowBalanceParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance GroupOnParameter = new DisplayFilterParameters_StockBaance();
            GroupOnParameter.ParameterName = "GroupOn";
            GroupOnParameter.Value = vm.GroupOn;
            GroupOnParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ShowOpeningParameter = new DisplayFilterParameters_StockBaance();
            ShowOpeningParameter.ParameterName = "ShowOpening";
            ShowOpeningParameter.Value = vm.ShowOpening.ToString();
            ShowOpeningParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance TextHiddenParameter = new DisplayFilterParameters_StockBaance();
            TextHiddenParameter.ParameterName = "TextHidden";
            TextHiddenParameter.Value = vm.TextHidden;
            TextHiddenParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance SizeParameter = new DisplayFilterParameters_StockBaance();
            SizeParameter.ParameterName = "Dimension1TypeName";
            SizeParameter.Value = vm.Dimension1Name;
            SizeParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance StyleParameter = new DisplayFilterParameters_StockBaance();
            StyleParameter.ParameterName = "Dimension2TypeName";
            StyleParameter.Value = vm.Dimension2Name;
            StyleParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ShadeParameter = new DisplayFilterParameters_StockBaance();
            ShadeParameter.ParameterName = "Dimension3TypeName";
            ShadeParameter.Value = vm.Dimension3Name;
            ShadeParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance FabricParameter = new DisplayFilterParameters_StockBaance();
            FabricParameter.ParameterName = "Dimension4TypeName";
            FabricParameter.Value = vm.Dimension4Name;
            FabricParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ProcessParameter = new DisplayFilterParameters_StockBaance();
            ProcessParameter.ParameterName = "Process";
            ProcessParameter.Value = vm.Process;
            ProcessParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance LotNoParameter = new DisplayFilterParameters_StockBaance();
            LotNoParameter.ParameterName = "LotNo";
            LotNoParameter.Value = vm.LotNo;
            LotNoParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance PlanNoParameter = new DisplayFilterParameters_StockBaance();
            PlanNoParameter.ParameterName = "PlanNo";
            PlanNoParameter.Value = vm.PlanNo;
            PlanNoParameter.IsApplicable = true;

            DisplayFilterParameters_StockBaance ProvisionParameter = new DisplayFilterParameters_StockBaance();
            ProvisionParameter.ParameterName = "Provision";
            ProvisionParameter.Value = vm.Provision;
            ProvisionParameter.IsApplicable = true;


            DisplayFilterSettings.DisplayFilterParameters = new List<DisplayFilterParameters_StockBaance>();
            DisplayFilterSettings.DisplayFilterParameters.Add(FromDateParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ToDateParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ProductNatureParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ProductTypeParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ShowBalanceParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ProductCategoryParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ProductGorupParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(SiteParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(DivisionParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ProductParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ShowOpeningParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(GroupOnParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(TextHiddenParameter);

            DisplayFilterSettings.DisplayFilterParameters.Add(ProcessParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(SizeParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(StyleParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ShadeParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(FabricParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(GodownParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(LotNoParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(PlanNoParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(ProvisionParameter);
            DisplayFilterSettings.DisplayFilterParameters.Add(PersonParameter);
            System.Web.HttpContext.Current.Session["CurrentSetting"] = DisplayFilterSettings;
            return DisplayFilterSettings;
        }

        public void SaveCurrentSetting()
        {
            ((List<DisplayFilterSettings_StockBaance>)System.Web.HttpContext.Current.Session["SettingList"]).Add((DisplayFilterSettings_StockBaance)System.Web.HttpContext.Current.Session["CurrentSetting"]);

        }
        public JsonResult GetParameterSettingsForLastDisplay()
        {
            var SettingList = (List<DisplayFilterSettings_StockBaance>)System.Web.HttpContext.Current.Session["SettingList"];

            var LastSetting = (from H in SettingList select H).LastOrDefault();
            System.Web.HttpContext.Current.Session["CurrentSetting"] = LastSetting;
            var FromDateSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "FromDate" select H).FirstOrDefault();
            var ToDateSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ToDate" select H).FirstOrDefault();
            var ProductNatureSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ProductNature" select H).FirstOrDefault();
            var ProductTypeSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ProductType" select H).FirstOrDefault();
            var GodownSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Godown" select H).FirstOrDefault();
            var PersonSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Name" select H).FirstOrDefault();
            var ShowBalanceSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ShowBalance" select H).FirstOrDefault();
            var ProductCategorySetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ProductCategory" select H).FirstOrDefault();
            var ProductGroupSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ProductGroup" select H).FirstOrDefault();
            var SiteSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "SiteIds" select H).FirstOrDefault();
            var DivisionSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "DivisionIds" select H).FirstOrDefault();
            var ProcessSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Process" select H).FirstOrDefault();
            var ShowOpeningSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "ShowOpening" select H).FirstOrDefault();
            var ProductSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Product" select H).FirstOrDefault();
            var GroupOnSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "GroupOn" select H).FirstOrDefault();
            var Dimension3Setting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Dimension3TypeName" select H).FirstOrDefault();
            var Dimension2Setting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Dimension2TypeName" select H).FirstOrDefault();
            var Dimension1Setting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Dimension1TypeName" select H).FirstOrDefault();
            var Dimension4Setting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Dimension4TypeName" select H).FirstOrDefault();
            //  var ReceiveSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Receive" select H).FirstOrDefault();
            // var IssueSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Issue" select H).FirstOrDefault();
            var ProvisionSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "Provision" select H).FirstOrDefault();
            var TextHiddenSetting = (from H in LastSetting.DisplayFilterParameters where H.ParameterName == "TextHidden" select H).FirstOrDefault();
            //  if(SettingListPre.re)
            SettingList.Remove(LastSetting);

            return Json(new
            {


                FromDate = FromDateSetting.Value,
                ToDate = ToDateSetting.Value,
                ProductNature = ProductNatureSetting.Value,
                ProductType = ProductTypeSetting.Value,
                ProductCategory = ProductCategorySetting.Value,
                Product = ProductSetting.Value,
                Godown = GodownSetting.Value,
                Name = PersonSetting.Value,
                Process = ProcessSetting.Value,
                SiteId = SiteSetting.Value,
                DivisionId = DivisionSetting.Value,
                ShowBalance = ShowBalanceSetting.Value,
                GroupOn = GroupOnSetting.Value,
                ShowOpening = ShowOpeningSetting.Value,
                ProductGroup = ProductGroupSetting.Value,
                Size = Dimension3Setting.Value,
                Style = Dimension2Setting.Value,
                Shade = Dimension1Setting.Value,
                Fabric = Dimension4Setting.Value,
                Provision = ProvisionSetting.Value,
                TextHidden = TextHiddenSetting.Value,

            });
        }

        //public string ErrorMessage()
        //{
        //    return 
        //}

        public ActionResult DocumentMenu(int DocTypeId, int DocId)
        {

            if (DocTypeId == 0 || DocId == 0)
            {
                return View("Error");
            }
            //if (new Display_StockBalanceService(_unitOfWork).IsValidData(DocTypeId, DocId) == false)
            //{

            //    return View("~/Views/Shared/DataNotFound.cshtml").Warning("Some record are missing , can't move entry point!!!");
            //}
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            var DocumentType = new DocumentTypeService(_unitOfWork).Find(DocTypeId);
            if (DocumentType.ControllerActionId.HasValue && DocumentType.ControllerActionId.Value > 0)
            {
                ControllerAction CA = db.ControllerAction.Find(DocumentType.ControllerActionId.Value);
                if (CA == null)
                {
                    return View("~/Views/Shared/UnderImplementation.cshtml");
                }
                else if (!string.IsNullOrEmpty(DocumentType.DomainName))
                {
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings[DocumentType.DomainName] + "/" + CA.ControllerName + "/" + CA.ActionName + "/" + DocId);
                }
                else
                {
                    return RedirectToAction(CA.ActionName, CA.ControllerName, new { id = DocId });
                }
            }

            ViewBag.RetUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
            HandleErrorInfo Excp = new HandleErrorInfo(new Exception("Document Settings not Configured"), "TrialBalance", "DocumentMenu");
            return View("Error", Excp);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public JsonResult GetGroupOn(string searchTerm, int pageSize, int pageNum, int? filter)
        {
            var Query = _StockBalanceService.GetFilterGroupOnFormat(searchTerm, filter);
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
        public JsonResult SetGroupOn(string Ids)
        {
            //ComboBoxResult ProductJson = new ComboBoxResult();
            //ProductJson.id = Ids;
            //ProductJson.text = Ids;
            //return Json(ProductJson);
            string[] subStr = Ids.Split(',');
            List<ComboBoxResult> ProductJson = new List<ComboBoxResult>();
            for (int i = 0; i < subStr.Length; i++)
            {
                string temp = (subStr[i]);

                //  IEnumerable<ProductGroup> prod = from p in db.ProductGroups
                //                                 where p.ProductGroupId == temp
                //                                 select p;
                IEnumerable<GroupOnViewModel> GroupOnResultlist = db.Database.SqlQuery<GroupOnViewModel>("Web.GetGroupOnList").ToList();

                var list = (from D in GroupOnResultlist
                            where D.Dimension1Id == temp
                            select new ComboBoxResult()
                            {
                                id = D.Dimension1Id,
                                text = D.Dimension1TypeName
                            });
                //  List<ComboBoxList> prodLst = new List<ComboBoxList>();
                //List < ComboBoxResult > ResultList = new List<ComboBoxResult>();
                //ResultList.Add(new ComboBoxResult(){ id = ResultList., text = "Order Date" });
                //prodLst.Add(new ComboBoxList() { Id = 2, PropFirst = "Amendment Date" });

                ProductJson.Add(new ComboBoxResult()
                {
                    id = list.FirstOrDefault().id.ToString(),
                    text = list.FirstOrDefault().text.ToString()
                });
            }
            return Json(ProductJson);
        }
        public JsonResult GetShowBalence(string searchTerm, int pageSize, int pageNum, int? filter)
        {
            var Query = _StockBalanceService.GetFilterShowBalanceFormat(searchTerm, filter);
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
        public JsonResult SetShowBalence(String Ids)
        {
            ComboBoxResult ProductJson = new ComboBoxResult();
            ProductJson.id = Ids;
            ProductJson.text = Ids;
            return Json(ProductJson);
        }
        public JsonResult GetProvision(string searchTerm, int pageSize, int pageNum, int? filter)
        {
            var Query = _StockBalanceService.GetFilterProvision(searchTerm, filter);
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
        public JsonResult SetProvision(String Ids)
        {
            ComboBoxResult ProductJson = new ComboBoxResult();
            ProductJson.id = Ids;
            ProductJson.text = Ids;
            return Json(ProductJson);
        }

        public JsonResult GetDivisionSettings()
        {
            GridReportService _GridReportService = new GridReportService();
            _GridReportService.GetDivisionSettings();

            return Json(new
            {
                Dimension1Caption = _GridReportService.Dimension1Caption,
                Dimension2Caption = _GridReportService.Dimension2Caption,
                Dimension3Caption = _GridReportService.Dimension3Caption,
                Dimension4Caption = _GridReportService.Dimension4Caption,
            });
        }


    }
}
