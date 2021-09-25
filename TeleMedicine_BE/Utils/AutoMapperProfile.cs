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
            CreateMap<SymptomUM, Symptom>();
            #endregion

        }
    }
}
