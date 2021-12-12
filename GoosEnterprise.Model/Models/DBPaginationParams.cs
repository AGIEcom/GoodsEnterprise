using System;
using System.Collections.Generic;
using System.Text;

namespace GoodsEnterprise.Model.Models
{
    public class DBPaginationParams
    {
        public string sortColumn { get; set; }
        public string sortOrder { get; set; }

        public int OffsetValue { get; set; }
        public int PagingSize { get; set; }
        public string SearchText { get; set; }

        public string StoredProcuder { get; set; }
    }
}
