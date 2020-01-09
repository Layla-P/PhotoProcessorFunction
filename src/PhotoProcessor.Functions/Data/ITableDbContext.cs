using System.Collections.Generic;
using System.Threading.Tasks;
using Functions.Models;
using Microsoft.Azure.Cosmos.Table;
using PhotoProcessor.Functions.Models;

namespace PhotoProcessor.Functions.Data
{
    public interface ITableDbContext
    {
        Task CreateTableAsync();
        Task<GeneralStatusEnum> InsertOrMergeEntityAsync(ImageEntity entity);
        
        Task<ImageEntity> GetEntityAsync(string partitionKey, string rowKey);

    }
    
}