namespace RN_International_Website.Models
{
    public class TeamMember
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Role { get; set; } 
        public string? PhotoUrl { get; set; } 

        // Social Media Links (optional)
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? InstagramUrl { get; set; }
    }


}
