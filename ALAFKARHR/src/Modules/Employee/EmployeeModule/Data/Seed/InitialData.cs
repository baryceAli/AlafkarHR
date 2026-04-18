using EmployeeModule.Employees.Models;

namespace EmployeeModule.Data.Seed;

public static class InitialData
{
    public static Position Position => 
        Position.Create(
            Guid.Parse("c981ca2a-df9b-42d1-94b1-183e9cacdd6a"), 
            "موظف", 
            "Employee", 
            "E001", 
            2000, 
            Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"), 
            "baryce@gmail.com");

    

}
