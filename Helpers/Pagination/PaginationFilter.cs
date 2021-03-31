using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Classes
{
    public class PaginationFilter
    {
        public int CurrPage { get; set; }
        public int ItemsPerPage { get; set; }
        public PaginationFilter()
        {
            this.CurrPage = 1;
            this.ItemsPerPage = 4;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.CurrPage = pageNumber < 1 ? 1 : pageNumber;
            this.ItemsPerPage = pageSize > 4 ? 4 : pageSize;
        }
    }
}
