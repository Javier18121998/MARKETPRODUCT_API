﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.DataModels.EFModels
{
    public class CustomerRegistration
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
