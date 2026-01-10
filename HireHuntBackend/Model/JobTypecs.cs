namespace HireHuntBackend.Model
{
    public class JobTypecs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int JobPostId { get; set; }
        public JobPost JobPost { get; set; }
    }
}
