namespace TsrmWebApi.Models.DataModels
{
    public class ConnectionStringResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> ConnectionStrings { get; set; } = new List<string>();
        public string ConnectionString => ConnectionStrings?.FirstOrDefault();
        public string LocalDirectoryPath { get; set; }
    }
}
