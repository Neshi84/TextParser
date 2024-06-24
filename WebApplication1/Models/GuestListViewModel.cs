namespace WebApplication1.Models
{
    public class GuestListViewModel
    {
        public DateTime VisitFrom { get; set; }
        public DateTime VisitTo { get; set; }
        public string ProjectFirm { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? EnterGuestList { get; set; }
        public string? GuestListText { get; set; }
        public List<Guest>? Guests { get; set; }
    }
}
