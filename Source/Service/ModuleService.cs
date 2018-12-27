using System.Collections.Generic;
using System.Linq;
using System;
using Model;
//using AdminSetup.Models.Models;
//using AdminSetup.Models.ViewModels;
using Model.Models;
using Model.ViewModel;
using Data.Infrastructure;
//using Infrastructure.IO;

namespace Service
{
    public interface IModuleService : IDisposable
    {
        MenuModule Create(MenuModule pt);
        void Delete(int id);
        void Delete(MenuModule pt);
        MenuModule Find(string Name);
        MenuModule Find(int id);
        void Update(MenuModule pt);
        MenuModule Add(MenuModule pt);
        IEnumerable<MenuModouleViewModel> GetModuleList();
        IEnumerable<MenuModouleViewModel> GetModuleListForUser(List<string> Roles, int SiteId, int DivisionId);
        MenuModule GetModuleByName(string terms);
        MenuModuleViewModelList GetModules();
        MenuModuleViewModelList GetUserPermission(string RoleId);
    }

    public class ModuleService : IModuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRolesService _userRolesService;
        private readonly IRepository<MenuModule> _ModuleRepository;
        public ModuleService(IUnitOfWork unitOfWork, IRepository<MenuModule> moduleRepo, IUserRolesService userRolesService)
        {
            _unitOfWork = unitOfWork;
            _ModuleRepository = moduleRepo;
            _userRolesService = userRolesService;
        }
        public MenuModule GetModuleByName(string terms)
        {
            return (from p in _ModuleRepository.Instance
                    where p.ModuleName == terms
                    select p).FirstOrDefault();
        }

        public MenuModule Find(string Name)
        {
            return _ModuleRepository.Query().Get().Where(i => i.ModuleName == Name).FirstOrDefault();
        }

        public MenuModule Find(int id)
        {
            return _ModuleRepository.Find(id);
        }

        public MenuModule Create(MenuModule pt)
        {
            pt.ObjectState = ObjectState.Added;
            _ModuleRepository.Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _ModuleRepository.Delete(id);
        }

        public void Delete(MenuModule pt)
        {
            _ModuleRepository.Delete(pt);
        }

        public void Update(MenuModule pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _ModuleRepository.Update(pt);
        }

        public IEnumerable<MenuModouleViewModel> GetModuleList()
        {
            var pt = (from p in _ModuleRepository.Instance
                      where p.IsActive == true
                      orderby p.Srl
                      select new MenuModouleViewModel
                      {
                          IconName = p.IconName,
                          IsActive = p.IsActive,
                          ModuleId = p.ModuleId,
                          ModuleName = p.ModuleName,
                          Srl = p.Srl,
                          URL = p.URL,
                      }
                          );

            return pt;
        }

        public IEnumerable<MenuModouleViewModel> GetModuleListForUser(List<string> Role, int SiteId, int DivisionId)
        {
            List<MenuModouleViewModel> ModuleList = new List<MenuModouleViewModel>();
            //Testing Block

            var Roles = _userRolesService.GetRolesList().ToList();

            var RoleId = string.Join(",", from p in Roles
                                          where Role.Contains(p.Name)
                                          select p.Id.ToString());
            //End


            var ExistingData = (from L in _unitOfWork.Repository<RolesDocType>().Instance select L).FirstOrDefault();
            if (ExistingData == null)
            {
                var pt = (from p in _unitOfWork.Repository<RolesMenu>().Instance
                            join t in _unitOfWork.Repository<Menu>().Instance on p.MenuId equals t.MenuId
                            join t2 in _unitOfWork.Repository<MenuModule>().Instance on t.ModuleId equals t2.ModuleId
                            where p.SiteId == SiteId && p.DivisionId == DivisionId && RoleId.Contains(p.RoleId)
                            group t2 by t2.ModuleId into g
                            select g.FirstOrDefault()
                                );

                if (pt != null)
                    ModuleList = (from p in pt
                                  select new MenuModouleViewModel
                                  {
                                      IconName = p.IconName,
                                      IsActive = p.IsActive,
                                      ModuleId = p.ModuleId,
                                      ModuleName = p.ModuleName,
                                      Srl = p.Srl,
                                      URL = p.URL,
                                  }).ToList();
            }
            else
            {
                //var pt1 = (from R in _unitOfWork.Repository<RolesDocType>().Instance.Where(m => m.DocTypeId != null)
                //           join D in _unitOfWork.Repository<DocumentType>().Instance on R.DocTypeId equals D.DocumentTypeId into DocumentTypeTable
                //           from DocumentTypeTab in DocumentTypeTable.DefaultIfEmpty()
                //           join C in _unitOfWork.Repository<ControllerAction>().Instance on R.ControllerName equals C.ControllerName into ControllerActionTable
                //           from ControllerActionTab in ControllerActionTable.DefaultIfEmpty()
                //           join M in _unitOfWork.Repository<Menu>().Instance on new { B1 = ControllerActionTab.ControllerActionId, B2 = DocumentTypeTab.DocumentCategoryId } equals new { B1 = M.ControllerActionId ?? 0, B2 = M.DocumentCategoryId ?? 0 } into MenuTable
                //           from MenuTab in MenuTable.DefaultIfEmpty()
                //           join Md in _unitOfWork.Repository<MenuModule>().Instance on MenuTab.ModuleId equals Md.ModuleId into ModuleTable
                //           from ModuleTab in ModuleTable.DefaultIfEmpty()
                //           where RoleId.Contains(R.RoleId) && ModuleTab.ModuleId != null
                //           group ModuleTab by ModuleTab.ModuleId into Result
                //           select Result.FirstOrDefault());

                var pt1 = (from R in _unitOfWork.Repository<RolesDocType>().Instance.Where(m => m.DocTypeId != null)
                           join D in _unitOfWork.Repository<DocumentType>().Instance on R.DocTypeId equals D.DocumentTypeId into DocumentTypeTable
                           from DocumentTypeTab in DocumentTypeTable.DefaultIfEmpty()
                           join M in _unitOfWork.Repository<Menu>().Instance on DocumentTypeTab.DocumentCategoryId equals M.DocumentCategoryId into MenuTable
                           from MenuTab in MenuTable.DefaultIfEmpty()
                           join Md in _unitOfWork.Repository<MenuModule>().Instance on MenuTab.ModuleId equals Md.ModuleId into ModuleTable
                           from ModuleTab in ModuleTable.DefaultIfEmpty()
                           where RoleId.Contains(R.RoleId) && ModuleTab.ModuleId != null && ModuleTab.IsActive == true
                           group ModuleTab by ModuleTab.ModuleId into Result
                           select Result.FirstOrDefault());

                var pt2 = (from R in _unitOfWork.Repository<RolesDocType>().Instance.Where(m => m.ProductTypeId != null)
                           join D in _unitOfWork.Repository<ProductType>().Instance on R.ProductTypeId equals D.ProductTypeId into ProductTypeTable
                           from ProductTypeTab in ProductTypeTable.DefaultIfEmpty()
                           join M in _unitOfWork.Repository<Menu>().Instance on ProductTypeTab.ProductNatureId equals M.ProductNatureId into MenuTable
                           from MenuTab in MenuTable.DefaultIfEmpty()
                           join Md in _unitOfWork.Repository<MenuModule>().Instance on MenuTab.ModuleId equals Md.ModuleId into ModuleTable
                           from ModuleTab in ModuleTable.DefaultIfEmpty()
                           where RoleId.Contains(R.RoleId) && ModuleTab.ModuleId != null && ModuleTab.IsActive == true
                           group ModuleTab by ModuleTab.ModuleId into Result
                           select Result.FirstOrDefault());

                var pt3 = (from R in _unitOfWork.Repository<RolesDocType>().Instance.Where(m => m.MenuId != null)
                           join M in _unitOfWork.Repository<Menu>().Instance on R.MenuId equals M.MenuId into MenuTable
                           from MenuTab in MenuTable.DefaultIfEmpty()
                           join Md in _unitOfWork.Repository<MenuModule>().Instance on MenuTab.ModuleId equals Md.ModuleId into ModuleTable
                           from ModuleTab in ModuleTable.DefaultIfEmpty()
                           where RoleId.Contains(R.RoleId) && ModuleTab.ModuleId != null && ModuleTab.IsActive == true
                           group ModuleTab by ModuleTab.ModuleId into Result
                           select Result.FirstOrDefault());

                var pt = pt1.Union(pt2).Union(pt3);

                if (pt != null)
                    ModuleList = (from p in pt
                                  select new MenuModouleViewModel
                                  {
                                      IconName = p.IconName,
                                      IsActive = p.IsActive,
                                      ModuleId = p.ModuleId,
                                      ModuleName = p.ModuleName,
                                      Srl = p.Srl,
                                      URL = p.URL,
                                  }).ToList();
            }

            return ModuleList;
        }

        public MenuModuleViewModelList GetModules()
        {
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            int SiteID = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            MenuModuleViewModelList Vm = new MenuModuleViewModelList();
            List<MenuModouleViewModel> ma = new List<MenuModouleViewModel>();

            if ((List<MenuModouleViewModel>)System.Web.HttpContext.Current.Session["UserModuleList"] != null)
                ma = (List<MenuModouleViewModel>)System.Web.HttpContext.Current.Session["UserModuleList"];
            else if (UserRoles.Contains("Admin"))
            {
                var pt = (from p in _ModuleRepository.Instance
                          where p.IsActive == true
                          orderby p.Srl
                          select new MenuModouleViewModel
                          {
                              IconName = p.IconName,
                              IsActive = p.IsActive,
                              ModuleId = p.ModuleId,
                              ModuleName = p.ModuleName,
                              Srl = p.Srl,
                              URL = p.URL,
                          }
                          );

                Vm.MenuModule = pt.ToList();

                return Vm;
            }
            else
            {
                List<MenuModouleViewModel> ModuleList = new List<MenuModouleViewModel>();
                //Testing Block

                var Roles = _userRolesService.GetRolesList().ToList();

                var RoleId = string.Join(",", from p in Roles
                                              where UserRoles.Contains(p.Name)
                                              select p.Id.ToString());
                //End

                var pt = (from p in _unitOfWork.Repository<RolesMenu>().Instance
                          join t in _unitOfWork.Repository<Menu>().Instance on p.MenuId equals t.MenuId
                          join t2 in _unitOfWork.Repository<MenuModule>().Instance on t.ModuleId equals t2.ModuleId
                          where p.SiteId == SiteID && p.DivisionId == DivisionId && RoleId.Contains(p.RoleId)
                          group t2 by t2.ModuleId into g
                          select g.FirstOrDefault()
                              );

                if (pt != null)
                    ModuleList = (from p in pt
                                  select new MenuModouleViewModel
                                  {
                                      IconName = p.IconName,
                                      IsActive = p.IsActive,
                                      ModuleId = p.ModuleId,
                                      ModuleName = p.ModuleName,
                                      Srl = p.Srl,
                                      URL = p.URL,
                                  }).ToList();

                Vm.MenuModule = ModuleList;
            }
            Vm.MenuModule = ma;
            return Vm;
        }


        //public MenuModuleViewModelList GetModules()
        //{
        //    List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        //    int SiteID = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //    int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
        //    MenuModuleViewModelList Vm = new MenuModuleViewModelList();
        //    List<MenuModouleViewModel> ma = new List<MenuModouleViewModel>();

        //    if ((List<MenuModouleViewModel>)System.Web.HttpContext.Current.Session["UserModuleList"] != null)
        //        ma = (List<MenuModouleViewModel>)System.Web.HttpContext.Current.Session["UserModuleList"];
        //    else if (UserRoles.Contains("Admin"))
        //    {
        //        var pt = (from p in _ModuleRepository.Instance
        //                  where p.IsActive == true
        //                  orderby p.Srl
        //                  select new MenuModouleViewModel
        //                  {
        //                      IconName = p.IconName,
        //                      IsActive = p.IsActive,
        //                      ModuleId = p.ModuleId,
        //                      ModuleName = p.ModuleName,
        //                      Srl = p.Srl,
        //                      URL = p.URL,
        //                  }
        //                  );

        //        Vm.MenuModule = pt.ToList();

        //        return Vm;
        //    }
        //    else
        //    {
        //        List<MenuModouleViewModel> ModuleList = new List<MenuModouleViewModel>();
        //        //Testing Block

        //        var Roles = _userRolesService.GetRolesList().ToList();

        //        var temp = from p in Roles
        //                   where UserRoles.Contains(p.Name)
        //                   select p.Id;

        //        var RoleId = string.Join(",", from p in Roles
        //                                      where UserRoles.Contains(p.Name)
        //                                      select p.Id.ToString());
        //        //End

        //        //var pt = (from p in _unitOfWork.Repository<RolesMenu>().Instance
        //        //          join t in _unitOfWork.Repository<Menu>().Instance on p.MenuId equals t.MenuId
        //        //          join t2 in _unitOfWork.Repository<MenuModule>().Instance on t.ModuleId equals t2.ModuleId
        //        //          where p.SiteId == SiteID && p.DivisionId == DivisionId && RoleId.Contains(p.RoleId)
        //        //          group t2 by t2.ModuleId into g
        //        //          select g.FirstOrDefault()
        //        //              );

        //        var pt = (from R in _unitOfWork.Repository<RolesDocType>().Instance
        //                 join D in _unitOfWork.Repository<DocumentType>().Instance on R.DocTypeId equals D.DocumentTypeId into DocumentTypeTable from DocumentTypeTab in DocumentTypeTable.DefaultIfEmpty()
        //                 join C in _unitOfWork.Repository<ControllerAction>().Instance on new { A1 = R.ControllerName, A2 = R.ActionName }  equals new { A1 = C.ControllerName, A2 = C.ActionName } into ControllerActionTable from ControllerActionTab in ControllerActionTable.DefaultIfEmpty()
        //                 join M in _unitOfWork.Repository<Menu>().Instance on new { B1 = ControllerActionTab.ControllerActionId, B2 = DocumentTypeTab.DocumentCategoryId } equals new { B1 = M.ControllerActionId ?? 0, B2 = M.DocumentCategoryId ?? 0 } into MenuTable from MenuTab in MenuTable.DefaultIfEmpty()
        //                 join Md in _unitOfWork.Repository<MenuModule>().Instance on MenuTab.ModuleId equals Md.ModuleId into ModuleTable from ModuleTab in ModuleTable.DefaultIfEmpty()
        //                 where RoleId.Contains(R.RoleId) && ModuleTab.ModuleId != null
        //                 group ModuleTab by ModuleTab.ModuleId into Result
        //                 select Result.FirstOrDefault());

        //        if (pt != null)
        //            ModuleList = (from p in pt
        //                          select new MenuModouleViewModel
        //                          {
        //                              IconName = p.IconName,
        //                              IsActive = p.IsActive,
        //                              ModuleId = p.ModuleId,
        //                              ModuleName = p.ModuleName,
        //                              Srl = p.Srl,
        //                              URL = p.URL,
        //                          }).ToList();

        //        Vm.MenuModule = ModuleList;
        //    }
        //    Vm.MenuModule = ma;
        //    return Vm;
        //}

        public MenuModule Add(MenuModule pt)
        {
            _unitOfWork.Repository<MenuModule>().Insert(pt);
            return pt;
        }


        public MenuModuleViewModelList GetUserPermission(string RoleId)
        {
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            MenuModuleViewModelList Vm = new MenuModuleViewModelList();
            System.Web.HttpContext.Current.Session["RoleUId"] = RoleId;
            List<MenuModouleViewModel> ma = new List<MenuModouleViewModel>();
            if (UserRoles.Contains("Admin"))
            {
                ma = GetModuleList().ToList();
                Vm.RoleId = RoleId; Vm.RoleModification = true;
            }
            
            Vm.MenuModule = ma;

            return Vm;
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

    }
}
