namespace CarManagment.Models.ViewModels
{
    public class ProfileModel
    {
        public string UserId { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegTime { get; set; }
        public int TotalCars { get; set; }

    }
}
