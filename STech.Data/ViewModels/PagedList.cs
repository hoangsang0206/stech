namespace STech.Data.ViewModels;

public class PagedList<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int RemaingItems { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();
}