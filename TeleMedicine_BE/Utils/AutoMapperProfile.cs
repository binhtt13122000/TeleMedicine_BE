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

            #region AutoMapper DrugViewModel
            CreateMap<Drug, DrugVM>();
            CreateMap<DrugVM, Drug>();
            CreateMap<DrugCM, Drug>();
            CreateMap<Drug, DrugCM>();
            CreateMap<DrugUM, Drug>();
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

            #region AutoMapper DiseaseGroupViewModel
            CreateMap<DiseaseGroup, DiseaseGroupVM>();
            CreateMap<DiseaseGroupVM, DiseaseGroup>();
            CreateMap<DiseaseGroupCM, DiseaseGroup>();
            CreateMap<DiseaseGroupUM, DiseaseGroup>();
            #endregion

            #region AutoMapper DiseaseViewModel
            CreateMap<Disease, DiseaseVM>();
            CreateMap<DiseaseVM, Disease>();
            CreateMap<DiseaseCM, Disease>();
            CreateMap<DiseaseUM, Disease>();
            #endregion

            #region AutoMapper TimeFrameViewModel
            CreateMap<TimeFrame, TimeFrameVM>();
            CreateMap<TimeFrameVM, TimeFrame>();
            CreateMap<TimeFrameCM, TimeFrame>();
            CreateMap<TimeFrameUM, TimeFrame>();
            #endregion

            #region AutoMapper NotificationViewModel
            CreateMap<Notification, NotificationVM>();
            CreateMap<NotificationVM, Notification>();
            CreateMap<NotificationCM, Notification>();
            CreateMap<NotificationUM, Notification>();
            #endregion

            #region AutoMapper CertificationViewModel
            CreateMap<Certification, CertificationVM>();
            CreateMap<CertificationVM, Certification>();
            CreateMap<CertificationCM, Certification>();
            CreateMap<CertificationUM, Certification>();
            #endregion

            #region AutoMapper AccountViewModel
            CreateMap<Account, AccountManageVM>();
            CreateMap<AccountProfileVM, Account>();
            CreateMap<Account, AccountProfileVM>();
            CreateMap<AccountProfileUM, Account>();
            #endregion

            #region AutoMapper DoctorViewModel
            CreateMap<Doctor, DoctorVM>();
            CreateMap<DoctorVM, Doctor>();
            CreateMap<DoctorCM, Doctor>();
            CreateMap<DoctorUM, Doctor>();
            CreateMap<DoctorSM, Doctor>();
            CreateMap<Doctor, DoctorSM>();
            #endregion

            #region AutoMapper HospitalDoctorViewModel
            CreateMap<HospitalDoctor, HospitalDoctorVM>();
            CreateMap<HospitalDoctorVM, HospitalDoctor>();
            CreateMap<HospitalDoctorCM, HospitalDoctor>();
            CreateMap<HospitalDoctorWithRegisterCM, HospitalDoctor>();
            CreateMap<HospitalDoctorUM, HospitalDoctor>();
            #endregion

            #region AutoMapper CertificationDoctorViewModel
            CreateMap<CertificationDoctor, CertificationDoctorVM>();
            CreateMap<CertificationDoctorVM, CertificationDoctor>();
            CreateMap<CertificationDoctorWithRegisterCM, CertificationDoctor>();
            CreateMap<CertificationDoctorCM, CertificationDoctor>();
            CreateMap<CertificationDoctorUM, CertificationDoctor>();
            #endregion

            #region AutoMapper MajorDoctorViewModel
            CreateMap<MajorDoctor, MajorDoctorVM>();
            CreateMap<MajorDoctorVM, MajorDoctor>();
            CreateMap<MajorDoctorCM, MajorDoctor>();
            CreateMap<MajorDoctorWithRegisterCM, MajorDoctor>();
            CreateMap<MajorDoctorUM, MajorDoctor>();
            #endregion
        }
    }
}
