using Microsoft.AspNetCore.Mvc;

namespace MovieTime.Models
{
    public class Pagination
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }

        public int StartPage { get; private set; }

        public int EndPage { get; private set;}
        public string SortOrder { get; private set; }

        public Pagination()
        {

        }
        public Pagination(int totalItems, int page, int pageSize = 10, string sortOrder = "")
        {
            int totalPages=(int)Math.Ceiling((decimal)totalItems/(decimal)pageSize);
            int currentPage = page;

            int startPage = currentPage - 5;
            int endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage=endPage-(startPage-1);
                startPage = 1;
            }
            if(endPage>totalPages)
            {
                endPage= totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            TotalItems = totalItems;
            CurrentPage = currentPage;
            StartPage=startPage;
            EndPage=endPage;
            TotalPages = totalPages;
            PageSize = pageSize;
            SortOrder = SortOrder;
        }
    }
}
