using Inventory.API.Models;

namespace Inventory.API.Repositories;

public class LocationRepository : BaseRepository, ICrudRepository<Location>
{
    public async Task<RepoResult<bool>> Delete(int itemId, int companyId)
    {
        var result = new RepoResult<bool>();
        try 
        {
            string query = $@"
declare 
@locId int = {itemId},
@companyId int = {companyId};

delete from location
where CompanyId = @companyId
and Id = @locId;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<Location>> Get(int id, int companyId)
    {
        var result = new RepoResult<Location>();
        try 
        {
            string query = $@"
declare 
@locId int = {id},
@companyId int = {companyId};

select 
    Id,
    CompanyId,
    Description,
    Barcode
from location
where Id = @locId
and CompanyId = @companyId";
            result.Data = (await QueryAsync<Location>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<Location>>> Get(
        SearchRequest request, 
        int companyId)
    {
        var result = new RepoResult<SearchResult<Location>>();
        try 
        {
            string query = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};
select 
    Id,
    CompanyId,
    Description,
    Barcode
from location 
where CompanyId = @companyId
and [Description] LIKE @search+'%'
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<Location>(query)).ToList();
            string totalQuery = $@"select COUNT(*) from location;";
            var total = (await QueryAsync<int>(totalQuery)).First();

            result.Data = new()
            {
                Items = items,
                Total = total
            };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<int>> Insert(Location item, int companyId)
    {
        var result = new RepoResult<int>();
        try 
        {
            string query = $@"
declare 
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}',
@barcode nvarchar(max) = '{item.Barcode}';

insert into location
(
    CompanyId,
    [Description],
    Barcode
)
values 
(
    @companyId,
    @description,
    @barcode
);

select SCOPE_IDENTITY();";
            result.Data = (await QueryAsync<int>(query)).First();
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> Update(Location item, int companyId)
    {
        var result = new RepoResult<bool>();
        try 
        {
            string query = $@"
declare 
@locId int = {item.Id},
@companyId int = {companyId},
@description nvarchar(max) = '{item.Description}',
@barcode nvarchar(max) = '{item.Barcode}';

update location set 
    [Description] = @description,
    Barcode = @barcode
where CompanyId = @companyId
and Id = @locId;";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }
}