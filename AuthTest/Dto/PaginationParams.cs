namespace AuthTest.Dto;

public class PaginationParams
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    
    public PaginationParams()
    {
        Page = 1;
        PageSize = 10;
    }
    
    public PaginationParams(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize == 0 ? 10 : pageSize;
    }
}