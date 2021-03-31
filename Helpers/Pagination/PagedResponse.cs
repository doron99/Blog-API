using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Classes
{
    public class PagedResponse<T> : Response<T>
    {
        public int CurrPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public PagedResponse(T data, int currPage, int itemsPerPage)
        {
            this.CurrPage = currPage;
            this.ItemsPerPage = itemsPerPage;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
