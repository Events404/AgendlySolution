﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<RoleViewModel> Roles { get; set; }

    }
}
