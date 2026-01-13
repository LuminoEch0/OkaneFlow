namespace OkaneFlow.ViewModels;

    public class ContactVM
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Initials => Username.Length >= 2 ? Username[..2].ToUpper() : "??";
        public bool IsActive { get; set; } // Used for highlighting the selected contact
    }


