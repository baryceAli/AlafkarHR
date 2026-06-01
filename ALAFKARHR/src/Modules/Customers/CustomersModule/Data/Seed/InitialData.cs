using CustomersModule.Customers.Models;

namespace CustomersModule.Data.Seed;

public static class InitialData
{
    public static List<CustomerGroup> CustomerGroups => new List<CustomerGroup>
    {
        CustomerGroup.Create(Guid.Parse("37430971-6927-4457-b94f-c4fc367120ff"),"جمعيات","NPOs","",0,null,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),Guid.Parse("2243b966-e7c2-43f5-9e00-21f6315bcb22").ToString()),
        CustomerGroup.Create(Guid.Parse("f7352089-15a2-4ab9-8ff5-87745027608b"),"جملة","Wholesale","",0,null,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),Guid.Parse("2243b966-e7c2-43f5-9e00-21f6315bcb22").ToString()),
        CustomerGroup.Create(Guid.Parse("e96d6454-91d7-4c95-bf79-4d124494c026"),"كبار العملاء","VIP","",0,null,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),Guid.Parse("2243b966-e7c2-43f5-9e00-21f6315bcb22").ToString()),
        CustomerGroup.Create(Guid.Parse("fafb31b1-de3c-4be7-b570-0ab1f5295ceb"),"تجزئة","Retail","",0,null,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),Guid.Parse("2243b966-e7c2-43f5-9e00-21f6315bcb22").ToString()),
        CustomerGroup.Create(Guid.Parse("60cd1cb1-a719-43d7-825e-0c3f993f6b98"),"موزع","Distributor","",0,null,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),Guid.Parse("2243b966-e7c2-43f5-9e00-21f6315bcb22").ToString()),
        CustomerGroup.Create(Guid.Parse("b164cca7-2e5b-4f0e-bc76-065ae98cb115"),"حكومة","Government","",0,null,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),Guid.Parse("2243b966-e7c2-43f5-9e00-21f6315bcb22").ToString()),
    };
}
//
//
//
//
//
