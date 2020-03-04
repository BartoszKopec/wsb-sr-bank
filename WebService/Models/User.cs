﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class User //: IdentityRole
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}