using DOMAIN.ENUM;

namespace DOMAIN.ENITITES
{
    public class AppUser
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public EmployeeRole Role { get; set; }
    }
}
