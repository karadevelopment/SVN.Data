namespace SVN.Data.EntityFramework
{
    public class ConnectionModel
    {
        public string Metadata { get; set; }
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
    }
}