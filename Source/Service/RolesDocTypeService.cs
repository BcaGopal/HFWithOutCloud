using System.Collections.Generic;
using System.Linq;
using System;
using Model;
using System.Threading.Tasks;
//using AdminSetup.Models.Models;
//using AdminSetup.Models.ViewModels;
//using Infrastructure.IO;
using AutoMapper;
using Model.Models;
using Data.Infrastructure;
using Model.ViewModel;

namespace Service
{
    public interface IRolesDocTypeService : IDisposable
    {
        RolesDocType Create(RolesDocType pt);
        void Delete(int id);
        void Delete(RolesDocType pt);
        RolesDocType Find(int ptId);
        void Update(RolesDocType pt);
        RolesDocType Add(RolesDocType pt);
        IEnumerable<RolesDocType> GetRolesDocTypeList();
    }

    public class RolesDocTypeService : IRolesDocTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RolesDocType> _RolesDocTypeRepository;

        public RolesDocTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public RolesDocTypeService(IUnitOfWork unitOfWork, IRepository<RolesDocType> RolesDocTypeRepo)
        {
            _unitOfWork = unitOfWork;
            _RolesDocTypeRepository = RolesDocTypeRepo;
        }

        public RolesDocType Find(int pt)
        {
            return _unitOfWork.Repository<RolesDocType>().Find(pt);
        }

        public RolesDocType Create(RolesDocType pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<RolesDocType>().Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<RolesDocType>().Delete(id);
        }

        public void Delete(RolesDocType pt)
        {
            _unitOfWork.Repository<RolesDocType>().Delete(pt);
        }

        public void Update(RolesDocType pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<RolesDocType>().Update(pt);
        }

        public IEnumerable<RolesDocType> GetRolesDocTypeList()
        {
            var pt = _unitOfWork.Repository<RolesDocType>().Query().Get();

            return pt;
        }


        public RolesDocType Add(RolesDocType pt)
        {
            _unitOfWork.Repository<RolesDocType>().Insert(pt);
            return pt;
        }



        public void Save()
        {
            _unitOfWork.Save();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
