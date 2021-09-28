﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class AccountManageVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public DateTime Dob { get; set; }
        public bool? IsMale { get; set; }
        public bool? Active { get; set; }
        public DateTime? RegisterTime { get; set; }

        public virtual RoleVM Role { get; set; }
    }

    public class AccountProfileVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public DateTime Dob { get; set; }
        public bool? IsMale { get; set; }
    }

    public class AccountProfileUM
    {
        [Required]
        public int Id { get; set; }
        [Required(AllowEmptyStrings =false)]
        [StringLength(128)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string StreetAddress { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Locality { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string PostalCode { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Phone { get; set; }
        public string Avatar { get; set; }
        [Required]
        public DateTime Dob { get; set; }
        public bool? IsMale { get; set; }
    }

}