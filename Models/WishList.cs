﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class WishList
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<WishListEvent> WishListEvents { get; set; }

    }
}
