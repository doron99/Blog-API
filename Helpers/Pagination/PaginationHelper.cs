using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Classes
{
    public static class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords)
        {
            var respose = new PagedResponse<List<T>>(pagedData, validFilter.CurrPage, validFilter.ItemsPerPage);
            var totalPages = ((double)totalRecords / (double)validFilter.ItemsPerPage);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            respose.CurrPage = validFilter.CurrPage;
            respose.TotalPages = roundedTotalPages;
            respose.TotalItems = totalRecords;
            return respose;
        }
    }
}
