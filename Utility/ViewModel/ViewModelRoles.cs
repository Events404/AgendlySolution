﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class ViewModelRoles
    {
        [Required ,StringLength(100)]
        public String Name { get; set; }
    }
}
