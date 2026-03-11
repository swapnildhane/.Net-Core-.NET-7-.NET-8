using Employee_Task_Manager.Model;
using EmployeeTaskManager.Data;

public class EmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public List<Employee> GetEmployees()
    {
        return _context.Employees.ToList();
    }

    public void AddEmployee(Employee emp)
    {
        emp.Id = 0;   // Important fix

        _context.Employees.Add(emp);
        _context.SaveChanges();
    }

    public void DeleteEmployee(int id)
    {
        var emp = _context.Employees.Find(id);

        if (emp != null)
        {
            _context.Employees.Remove(emp);
            _context.SaveChanges();
        }
    }
}