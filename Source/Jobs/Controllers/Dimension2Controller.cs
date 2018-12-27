﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModel;
using AutoMapper;
using System.Xml.Linq;
using Model.ViewModels;
using Jobs.Helpers;

namespace Jobs.Controllers
{
    [Authorize]
    public class Dimension2Controller : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        List<string> UserRoles = new List<string>();
        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IDimension2Service _Dimension2Service;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public Dimension2Controller(IDimension2Service Dimension2Service, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _Dimension2Service = Dimension2Service;
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }
        // GET: /ProductMaster/

        public ActionResult Index(int id)
        {
            var Dimension2 = _Dimension2Service.GetDimension2List(id);
            ViewBag.id = id;
            ViewBag.Name = new ProductTypeService(_unitOfWork).Find(id).ProductTypeName;
            return View(Dimension2);
            //return RedirectToAction("Create");
        }

        public ActionResult ProductTypeIndex()
        {
            var producttype = new ProductTypeService(_unitOfWork).GetProductTypeList().Where(m => m.IsActive != false).ToList();
            return View("ProductTypeIndex", producttype);
        }

        // GET: /ProductMaster/Create

        public ActionResult Create(int id)//ProductType Id
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.Dimension2 + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            Dimension2ViewModel vm = new Dimension2ViewModel();
            vm.IsActive = true;
            vm.ProductTypeId = id;
            ViewBag.id = id;
            ViewBag.Name = new ProductTypeService(_unitOfWork).Find(id).ProductTypeName;

            var settings = new ProductTypeSettingsService(_unitOfWork).GetProductTypeSettingsForDocument(id);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "ProductTypeSettings", new { id = id }).Warning("Please create Product Type Settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }
            vm.ProductTypeSettings = Mapper.Map<ProductTypeSettings, ProductTypeSettingsViewModel>(settings);

            return View("Create", vm);
        }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Dimension2ViewModel vm)
        {
            Dimension2 pt = Mapper.Map<Dimension2ViewModel, Dimension2>(vm);

            if (ModelState.IsValid)
            {


                if (vm.Dimension2Id <= 0)
                {
                    pt.CreatedDate = DateTime.Now;
                    pt.ModifiedDate = DateTime.Now;
                    pt.CreatedBy = User.Identity.Name;
                    pt.ModifiedBy = User.Identity.Name;
                    pt.ObjectState = Model.ObjectState.Added;
                    _Dimension2Service.Create(pt);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        ViewBag.id = vm.ProductTypeId;
                        ViewBag.Name = new ProductTypeService(_unitOfWork).Find(vm.ProductTypeId ?? 0).ProductTypeName;
                        return View("Create", vm);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2).DocumentTypeId,
                        DocId = pt.Dimension2Id,
                        ActivityType = (int)ActivityTypeContants.Added,
                    }));

                    return RedirectToAction("Create", new { id = vm.ProductTypeId }).Success("Data saved successfully");

                }
                else
                {
                    List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

                    Dimension2 temp = _Dimension2Service.Find(pt.Dimension2Id);

                    Dimension2 ExRec = Mapper.Map<Dimension2>(temp);

                    temp.Dimension2Name = pt.Dimension2Name;
                    temp.IsActive = pt.IsActive;
                    temp.Description = pt.Description;
                    temp.ModifiedDate = DateTime.Now;
                    temp.ModifiedBy = User.Identity.Name;
                    temp.ObjectState = Model.ObjectState.Modified;
                    _Dimension2Service.Update(temp);

                    LogList.Add(new LogTypeViewModel
                    {
                        ExObj = ExRec,
                        Obj = temp,
                    });
                    XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);

                    try
                    {
                        _unitOfWork.Save();
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        ViewBag.id = pt.ProductTypeId;
                        ViewBag.Name = new ProductTypeService(_unitOfWork).Find(pt.ProductTypeId ?? 0).ProductTypeName;
                        return View("Create", pt);
                    }

                    LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                    {
                        DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2).DocumentTypeId,
                        DocId = temp.Dimension2Id,
                        ActivityType = (int)ActivityTypeContants.Modified,
                        xEModifications = Modifications,
                    }));
                    
                    return RedirectToAction("Index", new { id = pt.ProductTypeId }).Success("Data saved successfully");

                }

            }
            ViewBag.id = vm.ProductTypeId;
            ViewBag.Name = new ProductTypeService(_unitOfWork).Find(vm.ProductTypeId ?? 0).ProductTypeName;
            return View("Create", vm);
        }


        // GET: /ProductMaster/Edit/5

        public ActionResult Edit(int id)
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.Dimension2 + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            Dimension2 D1 = _Dimension2Service.Find(id);
            Dimension2ViewModel pt = Mapper.Map<Dimension2, Dimension2ViewModel>(D1);

            if (pt == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = pt.ProductTypeId;
            ViewBag.Name = new ProductTypeService(_unitOfWork).Find(pt.ProductTypeId ?? 0).ProductTypeName;

            if (pt.ProductTypeId != null)
            {
                var settings = new ProductTypeSettingsService(_unitOfWork).GetProductTypeSettingsForDocument((int)pt.ProductTypeId);

                if (settings == null && UserRoles.Contains("SysAdmin"))
                {
                    return RedirectToAction("Create", "ProductTypeSettings", new { id = id }).Warning("Please create Product Type Settings");
                }
                else if (settings == null && !UserRoles.Contains("SysAdmin"))
                {
                    return View("~/Views/Shared/InValidSettings.cshtml");
                }
                pt.ProductTypeSettings = Mapper.Map<ProductTypeSettings, ProductTypeSettingsViewModel>(settings);
            }

            return View("Create", pt);
        }

        // GET: /ProductMaster/Delete/5

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.Dimension2 + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Delete") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            Dimension2 Dimension2 = _Dimension2Service.Find(id);

            if (Dimension2 == null)
            {
                return HttpNotFound();
            }

            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            if (ModelState.IsValid)
            {

                var temp = _Dimension2Service.Find(vm.id);
                LogList.Add(new LogTypeViewModel
                {
                    ExObj = temp,
                });

                _Dimension2Service.Delete(vm.id);
                XElement Modifications = new ModificationsCheckService().CheckChanges(LogList);
                try
                {
                    _unitOfWork.Save();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    ModelState.AddModelError("", message);
                    return PartialView("_Reason", vm);
                }

                LogActivity.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
                {
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2).DocumentTypeId,
                    DocId = vm.id,
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    UserRemark = vm.Reason,
                    xEModifications = Modifications,
                }));               

                
                return Json(new { success = true });

            }
            return PartialView("_Reason", vm);
        }


        [HttpGet]
        public ActionResult NextPage(int id, int ptypeid)//CurrentHeaderId
        {
            var nextId = _Dimension2Service.NextId(id, ptypeid);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, int ptypeid)//CurrentHeaderId
        {
            var nextId = _Dimension2Service.PrevId(id, ptypeid);
            return RedirectToAction("Edit", new { id = nextId });
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Print()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Report()
        {

            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Dimension2);

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

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
