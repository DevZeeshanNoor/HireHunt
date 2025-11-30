namespace HireHuntBackend.Common
{
    public class RemoteOkJob
    {
        public int id { get; set; }
        public string company { get; set; }
        public string position { get; set; }
        public string location { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public List<string> tags { get; set; }
    }

}
