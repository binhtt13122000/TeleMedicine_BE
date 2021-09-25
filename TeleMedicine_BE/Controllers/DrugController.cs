using AutoMapper;
using BusinessLogic.Services;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;

namespace TeleMedicine_BE.Controllers
{
    [Route("api/v1/drugs")]
    [ApiController]
    public class DrugController : ControllerBase
    {
        private readonly IDrugService _drugService;
        private readonly IMapper _mapper;
        private readonly IPagingSupport<Drug> _pagingSupport;
        public DrugController(IDrugService drugService, IMapper mapper, IPagingSupport<Drug> pagingSupport)
        {
            _drugService = drugService;
            _mapper = mapper;
            _pagingSupport = pagingSupport;
        }


    }
}
