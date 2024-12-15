using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Utility.ViewModel
{
    public class EventIndexViewModel
    {
      
            public List<Event> Events { get; set; } // قائمة الأحداث
            public int CurrentPage { get; set; } // الصفحة الحالية
            public int TotalPages { get; set; } // إجمالي الصفحات
            public string Filter { get; set; } // الفلتر الحالي (مثل: all, available, past, today)
            public string SearchQuery { get; set; } // نص البحث
        

    }
}
