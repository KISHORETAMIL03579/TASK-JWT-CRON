using APPLICATION.DTO;

namespace APPLICATION.INTERFACE
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAll();
        Task<EmployeeDto?> GetById(Guid id);
        Task Create(CreateEmployeeDto dto);
        Task Update(Guid id, UpdateEmployeeDto dto);
        Task Delete(Guid id);
    }
}
