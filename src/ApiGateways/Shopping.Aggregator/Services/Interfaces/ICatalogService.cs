using Shopping.Aggregator.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModel>> GetCatalog();
        Task<CatalogModel> GetCatalog(string id);
        Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);

    }
}
