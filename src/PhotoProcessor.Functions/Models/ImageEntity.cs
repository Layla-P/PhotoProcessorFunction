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
            string id,
            string fileName,
            ProcessStatusEnum processStatusEnum,
            string processedUrl)
        {
            PartitionKey = partitionKey;
            RowKey = id;
            FileName = fileName;
            ProcessStatusEnum = processStatusEnum.EnumValue();
            ProcessedUrl = processedUrl;
        }

        public string FileName { get; set; }
        public string ProcessStatusEnum { get; set; }
        public string ProcessedUrl { get; set; }
    }

}