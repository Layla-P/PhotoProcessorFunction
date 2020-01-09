using System;
using Microsoft.Azure.Cosmos.Table;
using PhotoProcessor.Functions.Models;
using System.Reflection;
using System.ComponentModel;

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
            ProcessStatusEnum = processStatusEnum.EnumValue();
            ProcessedUrl = "unset";
        }

        public string FileName { get; set; }
        public string ProcessStatusEnum { get; set; }
        public string ProcessedUrl { get; set; }
    }

}