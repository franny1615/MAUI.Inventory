using Inventory.API.Models;

namespace Inventory.API.Repositories;

public interface ICrudRepository<T>
{
    public Task<RepoResult<int>> Insert(T item, int companyId);
    public Task<RepoResult<bool>> Update(T item, int companyId);
    public Task<RepoResult<DeleteResult>> Delete(int itemId, int companyId);
    public Task<RepoResult<T>> Get(int id, int companyId);
    public Task<RepoResult<SearchResult<T>>> Get(SearchRequest request, int companyId);
}

public class SearchResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Total { get; set; } = 0;
}

public class SearchRequest
{
    public string Search { get; set; } = "";
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 20;
    public int InventoryItemID { get; set; } = -1;
    public int SortBy { get; set; } = 0;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string QuantityType { get; set; } = string.Empty;
}

public enum InventorySortBy
{
    None = 0,
    Quantity = 1,
    QuantityType = 2,
    Barcode = 3,
    Location = 4,
    Status = 5,
    CreatedDate = 6,
    LastEditedDate = 7
}