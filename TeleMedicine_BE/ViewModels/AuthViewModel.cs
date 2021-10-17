using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class AuthCM
    {
        public string TokenId { get; set;}

        public int LoginType { get; set; } 
    }

    public class ImageCM
    {
        public IFormFile file { get; set; }
    }
}
