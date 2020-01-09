using System;
using Microsoft.Azure.Cosmos.Table;
using PhotoProcessor.Functions.Models;

namespace Functions.Models
{
    public class ImageEntity : TableEntity
    {
        public ImageEntity() { }

        public ImageEntity(
            string partitionKey,
            Guid id,
            string fileName,
            ProcessStatusEnum processStatusEnum)
        {
            PartitionKey = partitionKey;
            RowKey = id.ToString();
            FileName = fileName;
            ProcessStatusEnum = processStatusEnum;
        }

        public string FileName { get; set; }
        public ProcessStatusEnum ProcessStatusEnum { get; set; }
    }

}