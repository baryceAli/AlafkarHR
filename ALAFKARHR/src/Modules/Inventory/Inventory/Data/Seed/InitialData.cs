namespace Inventory.Data.Seed;

public class InitialData
{
    public static IEnumerable<Warehouse> Warehouses =>
       new List<Warehouse>
       {
            Warehouse.Create(Guid.Parse("8a04f770-054c-4ca5-9daf-131769260025"),"مخزن 1","WH01","Riyadh","Address 1",10.1,13.2,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com"),
            Warehouse.Create(Guid.Parse("301840c7-f355-44b2-acf2-6ee1b9085021"),"مخزن 2","WH02","Jedda","Address 2",11.1,13.2,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com"),
            Warehouse.Create(Guid.Parse("fd8f6600-a6ca-43f3-974c-9e8e49b272d1"),"مخزن 3","WH03","Madina","Address 3",12.1,13.2,Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com"),
       };

    //public static IEnumerable<InventoryItem> InventoryItems =>
    //    new List<InventoryItem>
    //    {
    //        InventoryItem.Create(
    //            Guid.Parse("50c9666d-8172-4bb5-a3ec-a4376c73187b"),
    //            Guid.Parse("8a04f770-054c-4ca5-9daf-131769260025"),
    //            Guid.Parse("2e18ef43-3713-4507-816c-b9a37331433d"),
    //            100,0,"baryce@gmail.com"),

    //        InventoryItem.Create(
    //            Guid.Parse("339c78a2-3817-4c10-93a4-cb096049744b"),
    //            Guid.Parse("8a04f770-054c-4ca5-9daf-131769260025"),
    //            Guid.Parse("11ef79b6-25db-4768-b224-fd651428c8d3"),
    //            70,0,"baryce@gmail.com"),
    //    };


}
