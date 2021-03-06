﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Alduin.Web.Models
{
    public class UdpModel : BaseFloodModel
    {
        [Required]
        [Display(Name = "Port")]
        public int Port { get; set; }
        [Required]
        [Display(Name = "Length")]
        public int Length { get; set; }
    }
}
