namespace DataAccessLayer.DataTransferObjects
{
    public class UserPreferenceDTO
    {
        public Guid PreferenceID { get; set; }
        public Guid UserID { get; set; }
        public bool DarkMode { get; set; } = false;
        public bool EmailNotifications { get; set; } = true;
        public string Currency { get; set; } = "EUR";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
    }
}
