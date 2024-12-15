using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class WishListEvent
    {
        public int Id { get; set; }
        public int WishListId { get; set; }
        public WishList wishList { get; set; }
        
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
