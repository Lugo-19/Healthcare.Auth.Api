namespace Healthcare.Auth.Api.Shared.Commons
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        public static PaginatedResponse<T> Create(IEnumerable<T> data, int page, int pageSize, int totalRecords) => new()
        {
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }
}
