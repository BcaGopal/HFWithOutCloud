using System.Collections.Generic;
using System.Linq;
using Data.Infrastructure;
using Model.ViewModel;
using System;
using Data.Models;
using System.Data.SqlClient;
using Model.ViewModels;

namespace Service
{
    public interface IRolePermissionService : IDisposable
    {
        IEnumerable<RolePermissionViewModel> RolePermissionDetail(string RoleId);
        IEnumerable<RoleProcessPermissionViewModel> RoleProcessPermissionDetail(string RoleId, int DocTypeId);
    }

    public class RolePermissionService : IRolePermissionService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

      

        public RolePermissionService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }
        public IEnumerable<RolePermissionViewModel> RolePermissionDetail(string RoleId)
        {
            SqlParameter SqlParameterRoleId = new SqlParameter("@RoleId", RoleId);

            string mQry = "";

//            mQry = @"SELECT D.DocumentTypeId, D.DocumentTypeName, IsNull(Max(VModule.ModuleName),'Others') AS ModuleName, IsNull(Max(VModule.SubModuleName),'Others') AS SubModuleName, 
//                    Max(VModule.ModuleSr) AS ModuleSr, Max(VModule.MenuSr) AS MenuSr,
//                    Max(Ca.ControllerName) AS ControllerName, 
//                    Max(CASE WHEN Ca.DisplayName = 'Add' THEN Ca.ActionName END) AS AddActionName, 
//                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Add' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Add],
//                    Max(CASE WHEN Ca.DisplayName = 'Edit' THEN Ca.ActionName END) AS EditActionName, 
//                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Edit' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Edit],
//                    Max(CASE WHEN Ca.DisplayName = 'Delete' THEN Ca.ActionName END) AS DeleteActionName, 
//                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Delete' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Delete],
//                    Max(CASE WHEN Ca.DisplayName = 'Print' THEN Ca.ActionName END) AS PrintActionName, 
//                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Print' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Print],
//                    Max(CASE WHEN Ca.DisplayName = 'Submit' THEN Ca.ActionName END) AS SubmitActionName, 
//                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Submit' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Submit],
//                    Case When Max(IsNull(VDocumentTypeProcesses.Cnt,0)) > 1 Then 'True' Else 'False' End IsVisibleProcess, 'Document' As EntryType
//                    FROM Web.ControllerActions Ca
//                    LEFT JOIN Web.DocumentTypes D ON Ca.ControllerName = D.ControllerName
//                    LEFT JOIN (
//                            Select Dp.DocumentTypeId, Count(*) As Cnt
//                            From Web.DocumentTypeProcesses Dp
//                            Group By Dp.DocumentTypeId
//                    ) As VDocumentTypeProcesses On D.DocumentTypeId = VDocumentTypeProcesses.DocumentTypeId
//                    LEFT JOIN (SELECT 1 AS IsPermissionGranted, Rd.DocTypeId, Rd.ControllerName, Rd.ActionName
//			                    FROM Web.RolesDocTypes Rd
//			                    WHERE Rd.RoleId = @RoleId
//                    ) AS VRolesDocTypes ON D.DocumentTypeId = VRolesDocTypes.DocTypeId
//		                    AND Ca.ControllerName = VRolesDocTypes.ControllerName
//		                    AND Ca.ActionName = VRolesDocTypes.ActionName
//                    LEFT JOIN (
//		                    SELECT Dt.DocumentTypeId, Max(Mm.ModuleName) AS ModuleName, Max(Sm.SubModuleName) AS SubModuleName,
//                            Max(Mm.Srl) AS ModuleSr, Max(M.Srl) AS MenuSr
//		                    FROM Web.Menus M 
//                            LEFT JOIN Web.MenuSubModules Sm On M.SubModuleId = Sm.SubModuleId
//		                    LEFT JOIN Web.MenuModules Mm ON M.ModuleId = Mm.ModuleId
//		                    LEFT JOIN Web.DocumentCategories Dc ON M.DocumentCategoryId = Dc.DocumentCategoryId
//		                    LEFT JOIN Web.DocumentTypes Dt ON Dc.DocumentCategoryId = Dt.DocumentCategoryId
//		                    WHERE Dt.DocumentTypeId IS NOT NULL
//		                    GROUP BY Dt.DocumentTypeId
//                    ) AS VModule ON D.DocumentTypeId = VModule.DocumentTypeId
//                    WHERE D.DocumentTypeId IS NOT NULL
//                    And IsNull(D.IsActive,0) <> 0
//                    AND Ca.DisplayName IN ('Add', 'Edit', 'Delete', 'Print', 'Submit')
//                    GROUP BY D.DocumentTypeId, D.DocumentTypeName ";

            mQry = @"SELECT Dt.DocumentTypeId As DocumentTypeId, Dt.DocumentTypeName As DocumentTypeName, IsNull(Max(Mm.ModuleName),'Others') As ModuleName, 
                    IsNull(Max(Sm.SubModuleName),'Others') As SubModuleName, 
                    Max(Mm.Srl) AS ModuleSr, Max(M.Srl) AS MenuSr,
                    Max(Ca.ControllerName) AS ControllerName, 
                    Max(CASE WHEN Ca.DisplayName = 'Add' THEN Ca.ActionName END) AS AddActionName, 
                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Add' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Add],
                    Max(CASE WHEN Ca.DisplayName = 'Edit' THEN Ca.ActionName END) AS EditActionName, 
                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Edit' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Edit],
                    Max(CASE WHEN Ca.DisplayName = 'Delete' THEN Ca.ActionName END) AS DeleteActionName, 
                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Delete' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Delete],
                    Max(CASE WHEN Ca.DisplayName = 'Print' THEN Ca.ActionName END) AS PrintActionName, 
                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Print' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Print],
                    Max(CASE WHEN Ca.DisplayName = 'Submit' THEN Ca.ActionName END) AS SubmitActionName, 
                    Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Submit' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Submit],
                    Case When Max(IsNull(VDocumentTypeProcesses.Cnt,0)) > 1 Then 'True' Else 'False' End  AS IsVisibleProcess, 'Document' As EntryType 
                    FROM Web.Menus M 
                    LEFT JOIN Web.DocumentTypes Dt ON M.DocumentCategoryId = Dt.DocumentCategoryId
                    LEFT JOIN Web.MenuModules Mm ON M.ModuleId = Mm.ModuleId
                    LEFT JOIN Web.MenuSubModules Sm On M.SubModuleId = Sm.SubModuleId
                    LEFT JOIN Web.ControllerActions Ca ON M.ControllerName = Ca.ControllerName
                    LEFT JOIN (
                            Select Dp.DocumentTypeId, Count(*) As Cnt
                            From Web.DocumentTypeProcesses Dp
                            Group By Dp.DocumentTypeId
                    ) As VDocumentTypeProcesses On Dt.DocumentTypeId = VDocumentTypeProcesses.DocumentTypeId
                    LEFT JOIN (SELECT 1 AS IsPermissionGranted, Rd.DocTypeId, Rd.ControllerName, Rd.ActionName
                                FROM Web.RolesDocTypes Rd
                                WHERE Rd.RoleId =  @RoleId
                    ) AS VRolesDocTypes ON Dt.DocumentTypeId = VRolesDocTypes.DocTypeId
                            AND Ca.ControllerName = VRolesDocTypes.ControllerName
                            AND Ca.ActionName = VRolesDocTypes.ActionName
                    WHERE 1=1 
                    AND Dt.DocumentTypeId IS NOT NULL
                    AND Ca.DisplayName IN ('Add', 'Edit', 'Delete', 'Print', 'Submit')
                    AND IsNull(M.IsVisible,0) <> 0
                    GROUP BY Dt.DocumentTypeId, Dt.DocumentTypeName ";

            mQry = mQry + " UNION ALL ";

            mQry = mQry + @"SELECT Pt.ProductTypeId As DocumentTypeId, Pt.ProductTypeName As DocumentTypeName, IsNull(Max(Mm.ModuleName),'Others') As ModuleName, 
                        IsNull(Max(Sm.SubModuleName),'Others') As SubModuleName, 
                        Max(Mm.Srl) AS ModuleSr, Max(M.Srl) AS MenuSr,
                        Max(Ca.ControllerName) AS ControllerName, 
                        Max(CASE WHEN Ca.DisplayName = 'Add' THEN Ca.ActionName END) AS AddActionName, 
                        Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Add' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Add],
                        Max(CASE WHEN Ca.DisplayName = 'Edit' THEN Ca.ActionName END) AS EditActionName, 
                        Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Edit' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Edit],
                        Max(CASE WHEN Ca.DisplayName = 'Delete' THEN Ca.ActionName END) AS DeleteActionName, 
                        Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Delete' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Delete],
                        Max(CASE WHEN Ca.DisplayName = 'Print' THEN Ca.ActionName END) AS PrintActionName, 
                        Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Print' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Print],
                        Max(CASE WHEN Ca.DisplayName = 'Submit' THEN Ca.ActionName END) AS SubmitActionName, 
                        Convert(BIT,Sum(CASE WHEN Ca.DisplayName = 'Submit' THEN IsNull(VRolesDocTypes.IsPermissionGranted,0) ELSE 0 END)) AS [Submit],
                        'False' AS IsVisibleProcess, 'Product' As EntryType 
                        FROM Web.Menus M 
                        LEFT JOIN Web.ProductTypes Pt ON M.ProductNatureId = Pt.ProductNatureId
                        LEFT JOIN Web.MenuModules Mm ON M.ModuleId = Mm.ModuleId
                        LEFT JOIN Web.MenuSubModules Sm On M.SubModuleId = Sm.SubModuleId
                        LEFT JOIN Web.ControllerActions Ca ON M.ControllerName = Ca.ControllerName
                        LEFT JOIN (SELECT 1 AS IsPermissionGranted, Rd.ProductTypeId, Rd.ControllerName, Rd.ActionName
                                    FROM Web.RolesDocTypes Rd
                                    WHERE Rd.RoleId = @RoleId
                        ) AS VRolesDocTypes ON Pt.ProductTypeId = VRolesDocTypes.ProductTypeId
                                AND Ca.ControllerName = VRolesDocTypes.ControllerName
                                AND Ca.ActionName = VRolesDocTypes.ActionName
                        WHERE 1=1 
                        AND Pt.ProductTypeId IS NOT NULL
                        AND Ca.DisplayName IN ('Add', 'Edit', 'Delete', 'Print', 'Submit')
                        AND IsNull(M.IsVisible,0) <> 0
                        GROUP BY Pt.ProductTypeId, Pt.ProductTypeName ";

            mQry = mQry + " UNION ALL ";

            mQry = mQry + @"SELECT M.MenuId As DocumentTypeId, M.MenuName As DocumentTypeName, IsNull(Mm.ModuleName,'Others') As ModuleName, IsNull(Sm.SubModuleName,'Others') As SubModuleName, 
                    Mm.Srl AS ModuleSr, M.Srl AS MenuSr,
                    Ca.ControllerName AS ControllerName, 
                    NULL AS AddActionName, Convert(BIT,0) AS [Add],
                    NULL AS EditActionName, Convert(BIT,0) AS [Edit],
                    NULL AS DeleteActionName, Convert(BIT,0) AS [Delete],
                    Ca.ActionName AS PrintActionName, Convert(BIT,IsNull(VRolesDocTypes.IsPermissionGranted,0)) AS [Print],
                    NULL AS SubmitActionName, Convert(BIT,0) AS [Submit],
                    'False' AS IsVisibleProcess, 'Report' As EntryType 
                    FROM Web.Menus M
                    LEFT JOIN Web.MenuModules Mm ON M.ModuleId = Mm.ModuleId
                    LEFT JOIN Web.MenuSubModules Sm On M.SubModuleId = Sm.SubModuleId
                    LEFT JOIN Web.ControllerActions Ca ON M.ControllerActionId = Ca.ControllerActionId 
                    LEFT JOIN (SELECT 1 AS IsPermissionGranted, Rd.MenuId, Rd.ControllerName, Rd.ActionName
                                FROM Web.RolesDocTypes Rd
                                WHERE Rd.RoleId = @RoleId
                    ) AS VRolesDocTypes ON M.MenuId = VRolesDocTypes.MenuId
                            AND Ca.ControllerName = VRolesDocTypes.ControllerName
                            AND Ca.ActionName = VRolesDocTypes.ActionName
                    WHERE 1=1 
                    And IsNull(M.IsVisible,0) <> 0
                    AND M.Description = 'Reports' 
                    ORDER BY ModuleSr, MenuSr ";


            IEnumerable<RolePermissionViewModel> RolePermissionViewModel = db.Database.SqlQuery<RolePermissionViewModel>(mQry, SqlParameterRoleId).ToList();


            return RolePermissionViewModel;

        }

        public IEnumerable<RoleProcessPermissionViewModel> RoleProcessPermissionDetail(string RoleId, int DocTypeId)
        {
            SqlParameter SqlParameterRoleId = new SqlParameter("@RoleId", RoleId);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", DocTypeId);

            string mQry = "";

            mQry = @"SELECT D.DocumentTypeId, P.ProcessId, P.ProcessName,
                        Convert(BIT,IsNull(VRolesDocTypeProcess.IsPermissionGranted,0)) AS IsActive
                        FROM Web.DocumentTypes D
                        LEFT JOIN Web.AspNetRoles R ON 1=1
                        LEFT JOIN Web.DocumentTypeProcesses Dp On D.DocumentTypeId = Dp.DocumentTypeId
                        LEFT JOIN Web.Processes P ON Dp.ProcessId = P.ProcessId
                        LEFT JOIN (SELECT 1 AS IsPermissionGranted, Rdp.RoleId, Rdp.DocTypeId, Rdp.ProcessId
                                    FROM Web.RolesDocTypeProcesses Rdp
                                    WHERE Rdp.RoleId = @RoleId AND Rdp.DocTypeId = @DocTypeId
                        ) AS VRolesDocTypeProcess ON R.Id = VRolesDocTypeProcess.RoleId
                                AND D.DocumentTypeId = VRolesDocTypeProcess.DocTypeId
                                AND P.ProcessId = VRolesDocTypeProcess.ProcessId
                        WHERE R.Id = @RoleId AND D.DocumentTypeId = @DocTypeId And P.ProcessId Is Not Null ";

            IEnumerable<RoleProcessPermissionViewModel> RoleProcessPermissionViewModel = db.Database.SqlQuery<RoleProcessPermissionViewModel>(mQry, SqlParameterRoleId, SqlParameterDocTypeId).ToList();

            return RoleProcessPermissionViewModel;
        }

        //public bool IsActionAllowed(List<string> UserRoles, int DocTypeId, int? ProcessId, string ControllerName, string ActionName)
        //{
        //    bool IsAllowed = true;

        //    var RolesDocType = (from L in db.RolesDocType 
        //                        join R in db.Roles on L.RoleId equals R.Id
        //                        where UserRoles.Contains(R.Name) && L.DocTypeId == DocTypeId 
        //                            && L.ControllerName == ControllerName && L.ActionName == ActionName 
        //                        select L).FirstOrDefault();

        //    if (RolesDocType == null)
        //    {
        //        IsAllowed = false;
        //    }
        //    else
        //    {
        //        if (ProcessId != null)
        //        {
        //            var RolesDocTypeProcess = (from L in db.RolesDocTypeProcess
        //                                       join R in db.AspNetRole on L.RoleId equals R.Id
        //                                       where UserRoles.Contains(R.Name) && L.DocTypeId == DocTypeId 
        //                                            && L.ProcessId == ProcessId 
        //                                       select L).FirstOrDefault();
        //            if (RolesDocTypeProcess == null)
        //                IsAllowed = false;
        //        }
        //    }

        //    return IsAllowed;
        //}


        public bool IsActionAllowed(List<string> UserRoles, int DocTypeId, int? ProcessId, string ControllerName, string ActionName)
        {
            bool IsAllowed = true;
            bool IsAllowedForPreviousRole = false;

            var ExistingData = (from L in db.RolesDocType select L).FirstOrDefault();
            if (ExistingData == null)
                return true;

            if (UserRoles.Contains("Admin"))
                return true;

            foreach(string RoleName in UserRoles)
            {
                if (IsAllowedForPreviousRole == false)
                {
                    var RolesDocType = (from L in db.RolesDocType
                                        join R in db.Roles on L.RoleId equals R.Id
                                        where R.Name == RoleName && L.DocTypeId == DocTypeId
                                            && L.ControllerName == ControllerName && L.ActionName == ActionName
                                        select L).FirstOrDefault();

                    if (RolesDocType == null)
                    {
                        IsAllowed = false;
                    }
                    else
                    {
                        if (ProcessId != null && ProcessId != 0)
                        {
                            var RolesDocTypeProcess_Any = (from L in db.RolesDocTypeProcess
                                                           join R in db.Roles on L.RoleId equals R.Id
                                                       where R.Name == RoleName && L.DocTypeId == DocTypeId
                                                       select L).FirstOrDefault();
                            if (RolesDocTypeProcess_Any != null)
                            {
                                var RolesDocTypeProcess = (from L in db.RolesDocTypeProcess
                                                           join R in db.Roles on L.RoleId equals R.Id
                                                           where R.Name == RoleName && L.DocTypeId == DocTypeId
                                                                && L.ProcessId == ProcessId
                                                           select L).FirstOrDefault();
                                if (RolesDocTypeProcess == null)
                                    IsAllowed = false;
                                else
                                {
                                    IsAllowed = true;
                                    IsAllowedForPreviousRole = true;
                                }
                            }
                            else
                            {
                                IsAllowed = true;
                                IsAllowedForPreviousRole = true;
                            }
                        }
                        else
                        {
                            IsAllowed = true;
                            IsAllowedForPreviousRole = true;
                        }
                    }
                }
            }

            return IsAllowed;
        }

        public void Dispose()
        {
        }
    }


    public class RolePermissionViewModel
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string IsVisibleProcess { get; set; }
        public string EntryType { get; set; }
        public string ModuleName { get; set; }
        public string SubModuleName { get; set; }
        public string ControllerName { get; set; }
        public string AddActionName { get; set; }
        public bool Add { get; set; }
        public string EditActionName { get; set; }
        public bool Edit { get; set; }
        public string DeleteActionName { get; set; }
        public bool Delete { get; set; }
        public string PrintActionName { get; set; }
        public bool Print { get; set; }
        public string SubmitActionName { get; set; }
        public bool Submit { get; set; }

    }


    public class RoleProcessPermissionViewModel
    {
        public int DocumentTypeId { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public bool IsActive { get; set; }
    }
}

