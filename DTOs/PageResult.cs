namespace Api_entradas.DTOs
{
    public class PageResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page, PageSize, TotalCount;
    }
}
