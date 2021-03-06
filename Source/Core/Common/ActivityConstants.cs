﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public class PurchaseOrderActivityTypeConstants
    {
        public const int ShipDateChange = 1;
        public const int ProgressChange = 2;
    }

    public class SessionNameConstants
    {
        public const string CompanyName = "CompanyName";
        public const string SiteAddress = "SiteAddress";
        public const string SiteCityName = "SiteCityName";
        public const string SiteName = "SiteName";
        public const string DivisionName = "DivisionName";
        public const string SiteShortName = "SiteShortName";
        public const string LoginSiteId = "LoginSiteId";
        public const string LoginDivisionId = "LoginDivisionId";
        public const string UserNotificationCount = "uNotifiCount";
        public const string LoginDomain = "_loginDomain";
        public const string MenuDomain = "_menuDomain";
        public const string TitleCase = "TitleCase";
    }

    public class DocumentStatusConstants
    {
        public const string PendingToSubmit = "Pending to submit";
        public const string PendingToApprove = "Pending to approve";
    }

    public enum ActivityTypeContants
    {
        UnKnown = 0,
        Submitted = 1,
        Approved = 2,
        Modified = 3,
        ModificationSumbitted = 4,
        Deleted = 5,
        Added = 6,
        Closed = 7,
        Report = 8,
        Reviewed = 9,
        Import = 10,
        WizardCreate = 11,
        MultipleCreate = 12,
        Print = 13,
        Detail=14,
        FileAdded=15,
        FileRemoved=16,
        SettingsAdded=17,
        SettingsModified=18,
    }

    public class InspectionRequestActivityTypeConstants
    {
        public const int InspectionDateChange = 1;
    }

    public static class RefreshData
    {
        public static bool RefreshProductData = false;
    }

    public enum DepartmentConstants
    {
        Production = 1,
    }

    public class ProductGroupConstants
    {
        public const string Bom = "Bom";
        public const string Trace = "Trace";
        public const string Map = "Map";
        public const string LedgerAccount = "Ledger Account";
    }

    public class TitleCaseConstants
    {
        public const string UpperCase = "UpperCase";
        public const string LowerCase = "LowerCase";
        public const string TitleCase = "TitleCase";
    }


    public class ProductTypeConstants
    {
        public const string Rug = "Rug";
        public const string FinishedMaterial = "Finished Material";
        public const string Bom = "Bom";
        public const string Stencil = "Stencil";
        public const string Trace = "Trace";
        public const string Map = "Map";
        public const string OtherMaterial = "Other Material";
    }

    public class ProductNatureConstants
    {
        public const string Bom = "Bom";
        public const string Rawmaterial = "Raw Material";
        public const string OtherMaterial = "Other Material";
        public const string FinishedMaterial = "Finished Material";
        public const string Machine = "Machine";
        public const string AdditionalCharges = "Addition/Deduction";
    }

    public class UnitConstants
    {
        public const string Pieces = "PCS";
        public const string Kgs = "KG";
        public const string BOX = "BOX";
        public const string SqCms = "CM2";
        public const string Feet = "FT";
        public const string Liter = "Lit";
        public const string METER = "MET";
        public const string SqMeter = "MT2";
        public const string Nos = "Nos";
        public const string PKT = "PKT";
        public const string Roll = "Rol";
        public const string Yard = "Yar";
        public const string SqYard = "YD2";
        public const string SqFeet = "FT2";
    }

    public class DisplayTypeConstants
    {
        public const string Balance = "Balance";
        public const string Summary = "Summary";
    }

    public class DrCrConstants
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";
        public const string Both = "Both";
    }

    public class StockInHandGroupOnConstants
    {
        public const string Godown = "Godown";
        public const string Process = "Process";
        public const string Product = "Product";
        public const string Dimension1 = "Dimension1";
        public const string Dimension2 = "Dimension2";
        public const string Dimension3 = "Dimension3";
        public const string Dimension4 = "Dimension4";
        public const string LotNo = "LotNo";
        public const string Person = "Person";
    }
    public class LedgerHeaderAdjustmentTypeConstants
    {
        public const string Payment = "Payment";
        public const string Advance = "Advance";
    }

    public class StockInHandShowBalanceConstants
    {
        public const string NotZero = "Not Zero";
        public const string Zero = "Zero";
        public const string GreaterThanZero = "Greater Than Zero";
        public const string LessThanZero = "Less Than Zero";
        public const string PeriodNegative = "Period Negative";
        public const string All = "All";
    }

    public class AddressTypeConstants
    {
        public const string Work = "Work";
        public const string Office = "Office";
        public const string Permanent = "Permanent";
        public const string Temporary = "Temporary";
        public const string Godown = "Godown";
    }
    public class LedgerAccountTypeConstants
    {
        public const string Real = "Real";
        public const string Personal = "Personal";
        public const string Nominal = "Nominal";
        public const string Bank = "Bank";
    }
    public class LedgerAccountNatureConstants
    {
        public const string Assets = "Asset";
        public const string Liabilities = "Liability";
        public const string Income = "Income";
        public const string Expenditure = "Expense";
        public const string Bank = "Bank";
    }

    public class LedgerAccountGroupConstants
    {
        public const string SundryCreditors = "Sundry Creditors";
    }

    public class LedgerAccountConstants
    {
        public const string Sale = "Sale";
        public const string Party = "|PARTY|";
        public const string Charge = "|CHARGE|";
    }

    public class ChargeTypeCategoryConstants
    {
        public const string SalesTax = "Sales Tax";
    }

    public class SizeTypeConstants
    {
        public const string Standard = "Standard";
    }

    public class PersonRegistrationType
    {
        public const string TinNo = "Tin No";
        public const string CstNo = "Cst No";
        public const string GstNo = "Gst No";
        public const string PANNo = "PAN No";
        public const string ServiceTaxNo = "Service Tax No";
        public const string KYCNo = "KYC No";
        public const string AadharNo = "Aadhar No";
    }

    public class ProcessConstants
    {
        //Systenm Define
        public const string Sales = "Sale";
        public const string Purchase = "Purchase";
        public const string Manufacturing = "Manufacturing";
        public const string Stock = "Stock";

        //Customize
        public const string OverTuft = "OverTuft";
        public const string Weaving = "Weaving";
        public const string Silai = "Silai";
        public const string Packing = "Packing";
        public const string FullFinishing = "Full Finishing";
        public const string ThirdBacking = "Third Backing";
        public const string Dyeing = "Dyeing";

    }

    public class StockStatusConstants
    {
        public const string Failed = "Failed";
    }

    public class PaymentTypeConstants
    {
        public const string ToPay = "To Pay";
        public const string Paid = "Paid";
        public const string TobeBilled = "To be Billed";
    }

    public class TransactionNatureConstants
    {
        public const string Return = "Return";
        public const string Debit = "Debit Note";
        public const string Credit = "Credit Note";
    }

    public class ProductUidStatusConstants
    {
        public const string Issue = "Issue";
        public const string Receive = "Receive";
        public const string Ship = "Ship";
        public const string Transfer = "Transfer";
        public const string Cancel = "Cancel";
        public const string Return = "Return";
        public const string Pack = "Pack";
        public const string Dispatch = "Dispatch";
        public const string Generated = "Gen";
    }
    public class MaterialPlanTypeConstants
    {
        public const string ProdOrder = "ProdOrder";
        public const string SaleOrder = "SaleOrder";
    }

    public class StockTransferConstants
    {
        public const string Issue = "Issue";
        public const string Receive = "Receive";
    }

    public class LedgerReferenceTypeConstants
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";
    }

    public class PrevNextConstants
    {
        public const string Prev = "Previous";
        public const string Next = "Next";
    }

    public class ForActionConstants
    {
        public const string PendingToSubmit = "Pending To Submit";
        public const string PendingToReview = "Pending To Review";
    }

    public class StockNatureConstants
    {
        public const string Issue = "I";
        public const string Receive = "R";
        public const string Transfer = "T";
    }

    public class PubConstants
    {
        public const int MainSiteId = 17;
    }

    public class JobStatusQtyConstants
    {
        public const string CancelQty = "Cancel Qty";
        public const string AmendmentQty = "Amendment Qty";
        public const string AmendmentRate = "Amendment Rate";
        public const string ReceiveQty = "Receive Qty";
        public const string InvoiceQty = "Invoice Qty";
        public const string InvoiceReceiveQty = "Invoice Receive Qty";
        public const string PaymentQty = "Payment Qty";
        public const string ReturnQty = "Return Qty";
    }

    public class PurchaseStatusQtyConstants
    {
        public const string CancelQty = "Cancel Qty";
        public const string AmendmentQty = "Amendment Qty";
        public const string AmendmentRate = "Amendment Rate";
        public const string ReceiveQty = "Receive Qty";
        public const string InvoiceQty = "Invoice Qty";
        public const string PaymentQty = "Payment Qty";
        public const string ReturnQty = "Return Qty";
    }

    public class RequisitionStatusQtyConstants
    {
        public const string CancelQty = "Cancel Qty";
        public const string AmendmentQty = "Amendment Qty";
        public const string IssueQty = "Issue Qty";
        public const string ReceiveQty = "Receive Qty";
    }

    public class SaleStatusQtyConstants
    {
        public const string CancelQty = "Cancel Qty";
        public const string AmendmentQty = "Amendment Qty";
        public const string ReceiveQty = "Receive Qty";
        public const string InvoiceQty = "Invoice Qty";
        public const string PaymentQty = "Payment Qty";
        public const string ReturnQty = "Return Qty";
    }

    public class ProdStatusQtyConstants
    {
        public const string CancelQty = "Cancel Qty";
        public const string AmendmentQty = "Amendment Qty";
        public const string JobOrderQty = "Job Order Qty";
    }

    public class ChargeCodeConstants
    {
        public const string Incentive = "INCENT";
        public const string Penalty = "PENALTY";
    }

    public class ProjectConstants
    {
        public const string Login = "Login";
        public const string Module = "Module";
        public const string Calculation = "Calculation";
        public const string Customize = "Customize";
        public const string Finance = "Finance";
        public const string HumanResource = "Human Resource";
        public const string Jobs = "Jobs";
        public const string Planning = "Planning";
        public const string Purchase = "Purchase";
        public const string Sales = "Sales";
        public const string Store = "Store";
        public const string Tasks = "Tasks";
    }

    public class CommonControllers
    {
        public static string[] Controllers = { "Account", "AdminSetup", "ComboHelpList", "DuplicateDocumentCheck", "Home", "ActivityLog", "Notification" };
    }

    public enum NotificationTypeConstants
    {
        Danger,
        Warning,
        Success,
        Info
    }
    public class JobReceiveTypeConstants
    {
        public const string ProductUid = "ProductUId";
        public const string ProductUIdHeaderId = "ProductUIdHeaderId";
        public const string NA = "NA";
    }

    public class RateListCalculateOnConstants
    {
        public const string Qty = "Qty";
        public const string DealQty = "Deal Qty";
        public const string Weight = "Weight";
    }

    public class DocumentTimePlanTypeConstants
    {
        public const string Create = "Create";
        public const string Submit = "Submit";
        public const string Modify = "Modify";
        public const string Delete = "Delete";
        public const string GatePassCreate = "GatePassCreate";
        public const string GatePassCancel = "GatePassCancel";
    }


    public class ReportFormatConstants
    {
        public const string JobWorkerWise = "Job Worker Wise Summary";
        public const string MonthWise = "Month Wise Summary";
        public const string ProductWise = "Product Wise Summary";
        public const string ProductGroupWise = "Product Group Wise Summary";
    }
    public class ReportTypeConstants
    {
        public const string JobInvoice = "Job Invoice";
        public const string JobInvoiceReturn = "Job Invoice Return/Debit Credit";
    }


}
