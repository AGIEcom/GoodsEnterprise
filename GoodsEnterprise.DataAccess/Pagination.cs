using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise.DataAccess.Interface
{
    public class Pagination
    {
        public Pagination()
        {
      
        }
        public string SortBy { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 5;
        public int TotalRecords { get; set; } = 0;
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

    }
}
