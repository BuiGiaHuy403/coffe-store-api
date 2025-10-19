using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Responses
{
    public class PaginationServiceResponse<T>
    {
        public int TotalCurrent {  get; set; }
        public int TotalPage { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Results { get; set; } = [];
    }
}
