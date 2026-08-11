namespace ClinicFlow.Application.Common.Helper
{
    public class PagedResponse<T>
    {

        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PagedResponse(IEnumerable<T> data, int totalRecords, int currentPage, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

    }
}
