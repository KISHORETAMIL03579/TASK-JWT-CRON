using DOMAIN.ENITITES;

namespace INFRASTRUCTURE
{
    public static class EmployeeRepository
    {
        // In-memory data store (simulating a database table)
        public static List<Employee> Employees { get; } = new List<Employee>();
    }
}
