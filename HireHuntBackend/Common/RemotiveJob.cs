namespace HireHuntBackend.Common
{
    public class RemotiveJobResponse
    {
        public List<RemotiveJob> Jobs { get; set; }
    }
    public class RemotiveJob
    {
        public string Title { get; set; }
        public string Company_Name { get; set; }
        public string Publication_Date { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }
}
