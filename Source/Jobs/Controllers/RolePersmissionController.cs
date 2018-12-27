﻿using System;
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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Core.Common;

namespace Jobs.Controllers
{
    [Authorize]
    public class RolePermissionController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        List<string> UserRoles = new List<string>();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        IRolePermissionService _RolePermissionService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public RoleManager<IdentityRole> RoleManager { get; private set; }
        public RolePermissionController(IRolePermissionService RolePermissionService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _RolePermissionService = RolePermissionService;
            _exception = exec;
            _unitOfWork = unitOfWork;

            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        }

        public ActionResult Index()
        {
            return View(RoleManager.Roles.OrderBy(m => m.Name));
        }

        public ActionResult Create()
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.UserRoles);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.UserRoles + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Delete") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First().ToString());
                    return View();
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", "RolePermission", new { Id = role.Id }).Success("Data saved successfully");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Edit(string id)
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.UserRoles);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.UserRoles + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            var role = RoleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View("Create", role);
        }


        public JsonResult RolePermissionFill(string id)
        {
            IEnumerable<RolePermissionViewModel> RolePermissionViewModel = _RolePermissionService.RolePermissionDetail(id);

            if (RolePermissionViewModel != null)
            {
                JsonResult json = Json(new { Success = true, Data = RolePermissionViewModel.ToList() }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RoleProcessPermissionFill(string RoleId, int DocTypeId)
        {
            IEnumerable<RoleProcessPermissionViewModel> RoleProcessPermissionViewModel = _RolePermissionService.RoleProcessPermissionDetail(RoleId, DocTypeId);

            if (RoleProcessPermissionViewModel != null)
            {
                JsonResult json = Json(new { Success = true, Data = RoleProcessPermissionViewModel.ToList() }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        public void SavePermission(string RoleId, int DocumentTypeId, string ControllerName, 
                  string AddActionName, bool Add,
                  string EditActionName, bool Edit,
                  string DeleteActionName, bool Delete,
                  string PrintActionName, bool Print,
                  string SubmitActionName, bool Submit, string EntryType)
        {
            int? DocTypeId = null;
            int? ProductTypeId = null;
            int? MenuId = null;

            if (EntryType == "Document")
                DocTypeId = DocumentTypeId;
            else if (EntryType == "Product")
                ProductTypeId = DocumentTypeId;
            else
                MenuId = DocumentTypeId;


            RolesDocType RolesDocTypeForAdd = new RolesDocType();

            if (EntryType == "Document")
                RolesDocTypeForAdd = (from L in db.RolesDocType where L.RoleId == RoleId && L.DocTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == AddActionName select L).FirstOrDefault();
            else if (EntryType == "Product")
                RolesDocTypeForAdd = (from L in db.RolesDocType where L.RoleId == RoleId && L.ProductTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == AddActionName select L).FirstOrDefault();
            else
                RolesDocTypeForAdd = (from L in db.RolesDocType where L.RoleId == RoleId && L.MenuId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == AddActionName select L).FirstOrDefault();

            if (Add == true)
            {
                if (RolesDocTypeForAdd == null)
                {
                    RolesDocType RolesDocType = new RolesDocType();
                    RolesDocType.RoleId = RoleId;
                    RolesDocType.DocTypeId = DocTypeId;
                    RolesDocType.ProductTypeId = ProductTypeId;
                    RolesDocType.MenuId = MenuId;
                    RolesDocType.ControllerName = ControllerName;
                    RolesDocType.ActionName = AddActionName;
                    RolesDocType.CreatedBy = User.Identity.Name;
                    RolesDocType.ModifiedBy = User.Identity.Name;
                    RolesDocType.CreatedDate = DateTime.Now;
                    RolesDocType.ModifiedDate = DateTime.Now;
                    RolesDocType.ObjectState = Model.ObjectState.Added;
                    db.RolesDocType.Add(RolesDocType);
                }
            }
            else
            {
                if (RolesDocTypeForAdd != null)
                {
                    RolesDocTypeForAdd.ObjectState = Model.ObjectState.Deleted;
                    db.RolesDocType.Remove(RolesDocTypeForAdd);
                }
            }


            RolesDocType RolesDocTypeForEdit = new RolesDocType();

            if (EntryType == "Document")
                RolesDocTypeForEdit = (from L in db.RolesDocType where L.RoleId == RoleId && L.DocTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == EditActionName select L).FirstOrDefault();
            else if (EntryType == "Product")
                RolesDocTypeForEdit = (from L in db.RolesDocType where L.RoleId == RoleId && L.ProductTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == EditActionName select L).FirstOrDefault();
            else
                RolesDocTypeForEdit = (from L in db.RolesDocType where L.RoleId == RoleId && L.MenuId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == AddActionName select L).FirstOrDefault();

            if (Edit == true)
            {
                if (RolesDocTypeForEdit == null)
                {
                    RolesDocType RolesDocType = new RolesDocType();
                    RolesDocType.RoleId = RoleId;
                    RolesDocType.DocTypeId = DocTypeId;
                    RolesDocType.ProductTypeId = ProductTypeId;
                    RolesDocType.MenuId = MenuId;
                    RolesDocType.ControllerName = ControllerName;
                    RolesDocType.ActionName = EditActionName;
                    RolesDocType.CreatedBy = User.Identity.Name;
                    RolesDocType.ModifiedBy = User.Identity.Name;
                    RolesDocType.CreatedDate = DateTime.Now;
                    RolesDocType.ModifiedDate = DateTime.Now;
                    RolesDocType.ObjectState = Model.ObjectState.Added;
                    db.RolesDocType.Add(RolesDocType);
                }
            }
            else
            {
                if (RolesDocTypeForEdit != null)
                {
                    RolesDocTypeForEdit.ObjectState = Model.ObjectState.Deleted;
                    db.RolesDocType.Remove(RolesDocTypeForEdit);
                }
            }


            RolesDocType RolesDocTypeForDelete = new RolesDocType();
            if (EntryType == "Document")
                RolesDocTypeForDelete = (from L in db.RolesDocType where L.RoleId == RoleId && L.DocTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == DeleteActionName select L).FirstOrDefault();
            else if (EntryType == "Product")
                RolesDocTypeForDelete = (from L in db.RolesDocType where L.RoleId == RoleId && L.ProductTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == DeleteActionName select L).FirstOrDefault();
            else
                RolesDocTypeForDelete = (from L in db.RolesDocType where L.RoleId == RoleId && L.MenuId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == DeleteActionName select L).FirstOrDefault();

            if (Delete == true)
            {
                if (RolesDocTypeForDelete == null)
                {
                    RolesDocType RolesDocType = new RolesDocType();
                    RolesDocType.RoleId = RoleId;
                    RolesDocType.DocTypeId = DocTypeId;
                    RolesDocType.ProductTypeId = ProductTypeId;
                    RolesDocType.MenuId = MenuId;
                    RolesDocType.ControllerName = ControllerName;
                    RolesDocType.ActionName = DeleteActionName;
                    RolesDocType.CreatedBy = User.Identity.Name;
                    RolesDocType.ModifiedBy = User.Identity.Name;
                    RolesDocType.CreatedDate = DateTime.Now;
                    RolesDocType.ModifiedDate = DateTime.Now;
                    RolesDocType.ObjectState = Model.ObjectState.Added;
                    db.RolesDocType.Add(RolesDocType);
                }
            }
            else
            {
                if (RolesDocTypeForDelete != null)
                {
                    RolesDocTypeForDelete.ObjectState = Model.ObjectState.Deleted;
                    db.RolesDocType.Remove(RolesDocTypeForDelete);
                }
            }


            RolesDocType RolesDocTypeForPrint = new RolesDocType();

            if (EntryType == "Document")
                RolesDocTypeForPrint = (from L in db.RolesDocType where L.RoleId == RoleId && L.DocTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == PrintActionName select L).FirstOrDefault();
            if (EntryType == "Product")
                RolesDocTypeForPrint = (from L in db.RolesDocType where L.RoleId == RoleId && L.ProductTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == PrintActionName select L).FirstOrDefault();
            else
                RolesDocTypeForPrint = (from L in db.RolesDocType where L.RoleId == RoleId && L.MenuId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == PrintActionName select L).FirstOrDefault();

            if (Print == true)
            {
                if (RolesDocTypeForPrint == null)
                {
                    RolesDocType RolesDocType = new RolesDocType();
                    RolesDocType.RoleId = RoleId;
                    RolesDocType.DocTypeId = DocTypeId;
                    RolesDocType.ProductTypeId = ProductTypeId;
                    RolesDocType.MenuId = MenuId;
                    RolesDocType.ControllerName = ControllerName;
                    RolesDocType.ActionName = PrintActionName;
                    RolesDocType.CreatedBy = User.Identity.Name;
                    RolesDocType.ModifiedBy = User.Identity.Name;
                    RolesDocType.CreatedDate = DateTime.Now;
                    RolesDocType.ModifiedDate = DateTime.Now;
                    RolesDocType.ObjectState = Model.ObjectState.Added;
                    db.RolesDocType.Add(RolesDocType);
                }
            }
            else
            {
                if (RolesDocTypeForPrint != null)
                {
                    RolesDocTypeForPrint.ObjectState = Model.ObjectState.Deleted;
                    db.RolesDocType.Remove(RolesDocTypeForPrint);
                }
            }

            RolesDocType RolesDocTypeForSubmit = new RolesDocType();

            if (EntryType == "Document")
                RolesDocTypeForSubmit = (from L in db.RolesDocType where L.RoleId == RoleId && L.DocTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == SubmitActionName select L).FirstOrDefault();
            else if (EntryType == "Product")
                RolesDocTypeForSubmit = (from L in db.RolesDocType where L.RoleId == RoleId && L.ProductTypeId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == SubmitActionName select L).FirstOrDefault();
            else
                RolesDocTypeForSubmit = (from L in db.RolesDocType where L.RoleId == RoleId && L.MenuId == DocumentTypeId && L.ControllerName == ControllerName && L.ActionName == SubmitActionName select L).FirstOrDefault();


            if (Submit == true)
            {
                if (RolesDocTypeForSubmit == null)
                {
                    RolesDocType RolesDocType = new RolesDocType();
                    RolesDocType.RoleId = RoleId;
                    RolesDocType.DocTypeId = DocTypeId;
                    RolesDocType.ProductTypeId = ProductTypeId;
                    RolesDocType.MenuId = MenuId;
                    RolesDocType.ControllerName = ControllerName;
                    RolesDocType.ActionName = SubmitActionName;
                    RolesDocType.CreatedBy = User.Identity.Name;
                    RolesDocType.ModifiedBy = User.Identity.Name;
                    RolesDocType.CreatedDate = DateTime.Now;
                    RolesDocType.ModifiedDate = DateTime.Now;
                    RolesDocType.ObjectState = Model.ObjectState.Added;
                    db.RolesDocType.Add(RolesDocType);
                }
            }
            else
            {
                if (RolesDocTypeForSubmit != null)
                {
                    RolesDocTypeForSubmit.ObjectState = Model.ObjectState.Deleted;
                    db.RolesDocType.Remove(RolesDocTypeForSubmit);
                }
            }



            try
            {
                db.SaveChanges();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
            }
        }




        public void SaveProcessPermission(string RoleId, int DocumentTypeId, int ProcessId, bool IsActive)
        {
            RolesDocTypeProcess RolesDocTypeProcess_Existing = (from L in db.RolesDocTypeProcess where L.RoleId == RoleId && L.DocTypeId == DocumentTypeId && L.ProcessId == ProcessId select L).FirstOrDefault();
            if (IsActive == true)
            {
                if (RolesDocTypeProcess_Existing == null)
                {
                    RolesDocTypeProcess RolesDocTypeProcess = new RolesDocTypeProcess();
                    RolesDocTypeProcess.RoleId = RoleId;
                    RolesDocTypeProcess.DocTypeId = DocumentTypeId;
                    RolesDocTypeProcess.ProcessId = ProcessId;
                    RolesDocTypeProcess.CreatedBy = User.Identity.Name;
                    RolesDocTypeProcess.ModifiedBy = User.Identity.Name;
                    RolesDocTypeProcess.CreatedDate = DateTime.Now;
                    RolesDocTypeProcess.ModifiedDate = DateTime.Now;
                    RolesDocTypeProcess.ObjectState = Model.ObjectState.Added;
                    db.RolesDocTypeProcess.Add(RolesDocTypeProcess);
                }
            }
            else
            {
                if (RolesDocTypeProcess_Existing != null)
                {
                    RolesDocTypeProcess_Existing.ObjectState = Model.ObjectState.Deleted;
                    db.RolesDocTypeProcess.Remove(RolesDocTypeProcess_Existing);
                }
            }

            try
            {
                db.SaveChanges();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
            }
        }



        public JsonResult RolePermissionPost()
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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
