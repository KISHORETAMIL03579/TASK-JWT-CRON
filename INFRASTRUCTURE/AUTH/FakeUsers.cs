using DOMAIN.ENITITES;
using DOMAIN.ENUM;

namespace INFRASTRUCTURE.AUTH
{
    public static class FakeUsers
    {
        public static List<AppUser> Users = new()
        {
            new AppUser { Username = "admin", Password = "admin123", Role = EmployeeRole.Admin },

            new AppUser { Username = "manager", Password = "manager123", Role = EmployeeRole.Manager },

            new AppUser { Username = "employee", Password = "emp123", Role = EmployeeRole.Employee }
        };

    }

}
