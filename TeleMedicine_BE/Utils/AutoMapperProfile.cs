using AutoMapper;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.ViewModels;

namespace TeleMedicine_BE.Utils
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            #region AutoMapper SymptomViewModel
            CreateMap<Symptom, SymptomVM>();
            CreateMap<SymptomVM, Symptom>();
            CreateMap<SymptomCM, Symptom>();
            CreateMap<Symptom, SymptomCM>();
            CreateMap<SymptomUM, Symptom>();
            #endregion

            #region AutoMapper HospitalViewModels
            CreateMap<Hospital, HospitalVM>();
            CreateMap<Hospital, HospitalWithDoctorVM>();
            CreateMap<HospitalVM, Hospital>();
            CreateMap<HospitalCM, Hospital>();
            CreateMap<HospitalUM, Hospital>();
            #endregion

            #region AutoMapper DrugTypeViewModel
            CreateMap<DrugType, DrugTypeVM>();
            CreateMap<DrugTypeVM, DrugType>();
            CreateMap<DrugTypeCM, DrugType>();
            CreateMap<DrugType, DrugTypeCM>();
            CreateMap<DrugTypeUM, DrugType>();
            #endregion

            #region AutoMapper MajorViewModel
            CreateMap<Major, MajorVM>();
            CreateMap<MajorVM, Major>();
            CreateMap<MajorCM, Major>();
            CreateMap<MajorUM, Major>();
            #endregion

            #region AutoMapper RoleViewModel
            CreateMap<Role, RoleVM>();
            CreateMap<RoleVM, Role>();
            CreateMap<RoleCM, Role>();
            CreateMap<RoleUM, Role>();
            #endregion
        }
    }
}
