namespace PhotoProcessor.Functions.Models
{
    public class TableConfiguration : ITableConfiguration
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }
}