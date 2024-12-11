using RN_International_Website.Models;

namespace RN_International_Website.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<TeamMember> TeamMembers { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
