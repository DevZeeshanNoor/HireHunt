namespace HireHuntBackend.Common
{
    public class ArbeitnowResponse
    {
        public List<ArbeitnowJob> Data { get; set; }
    }

    public class ArbeitnowJob
    {
        public string Title { get; set; }
        public string Company_Name { get; set; }
        public string Location { get; set; }
        public string Slug { get; set; }
        public string Created_At { get; set; }
        public string Url { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
    }

}
