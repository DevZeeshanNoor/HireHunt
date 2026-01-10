
namespace HireHuntBackend.Model
{
    public class JobPost
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public string? Source { get; set; }
        public string? Url { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }      
        public DateTime? PostedDate { get; set; }
        public DateTime CreationDateTime { get; set; }= DateTime.UtcNow;
    }

}
