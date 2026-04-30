using Catalog.Products.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Data.Seed;

public static class InitialData
{
    public static IEnumerable<Category> Categories =>
        new List<Category>
        {
            new Category(Guid.NewGuid(),"اغذية","Diary Food",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com",""),
            new Category(Guid.Parse( "9a4b92e6-7c2d-4722-9be6-5f2107bb5b3d"),"مشروبات","Drinks",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com",""),
            new Category(Guid.NewGuid(),"معجنات","Pastery", Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com",""),
            new Category(Guid.Parse("d0e96aa8-77cd-4e98-8a08-ff2a8f53abfc"),"أجبان وألبان","Milk and Cheese",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com", "")
        };

    public static IEnumerable<Brand> Brands =>
        new List<Brand>
        {
            new Brand(Guid.Parse("8af67c4b-fd17-4728-8ae6-f8cefc65da18"),"المراعي","Almaraee",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com",""),
            new Brand(Guid.Parse("5b580de5-80b2-4955-8d82-0d0c725e44a2"),"الصافي","Alsafy",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com",""),
            new Brand(Guid.NewGuid(),"الكبير","Alkabeer",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com","")
            //new Brand(Guid.NewGuid(),"الكبير")
        };

    public static IEnumerable< Unit> Units =>
        new List<Unit>
        {
            Unit.Create(Guid.Parse("9a4b92e6-7c2d-4722-9be6-5f2107bb5b3d"),"مل","ML",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),"baryce@gmail.com"),
            Unit.Create(Guid.NewGuid(),"لتر","Ltr",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com"),
            Unit.Create(Guid.Parse("a64630c2-d13d-42f0-9416-4f8aac39a8e6"), "جم", "MG",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com"),
            Unit.Create(Guid.NewGuid(), "كجم", "KG",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com"),
            Unit.Create(Guid.NewGuid(), "سم", "CM",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com"),
            Unit.Create(Guid.NewGuid(), "م", "M",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com")
        };
    public static IEnumerable<Product> Products =>
        new List<Product>
        {
            Product.Create(
                Guid.Parse("e0f16e7c-e899-4a95-a908-78072e693a25")
                ,"عصير منقة",
                "Mango Juice"
               
                ,Guid.Parse("8af67c4b-fd17-4728-8ae6-f8cefc65da18")
                ,Guid.Parse("9a4b92e6-7c2d-4722-9be6-5f2107bb5b3d")
                ,Guid.Parse("c33623f7-75ac-4358-9168-cec4ffc10cd2")
                
                ,"baryce@gmail.com"),

            Product.Create(
                Guid.Parse("5c8b8a8a-4128-445a-914f-389107ed7363")
                ,"عصير مشكل"
                ,"Mixed Juice"
                
                ,Guid.Parse("5b580de5-80b2-4955-8d82-0d0c725e44a2")
                ,Guid.Parse("9a4b92e6-7c2d-4722-9be6-5f2107bb5b3d")
                ,Guid.Parse("c33623f7-75ac-4358-9168-cec4ffc10cd2")
                
                ,"baryce@gmail.com"),
            
            Product.Create(Guid.NewGuid()
                ,"جبنة مثلثات"
                ,"Creamy Cheese"
                
                ,Guid.Parse("8af67c4b-fd17-4728-8ae6-f8cefc65da18")
                ,Guid.Parse("d0e96aa8-77cd-4e98-8a08-ff2a8f53abfc")
                ,Guid.Parse("a64630c2-d13d-42f0-9416-4f8aac39a8e6")
                
                ,"baryce@gmail.com"),
        };

    public static IEnumerable<ProductPackage> ProductPackages =>
        new List<ProductPackage>
        {
            
            new ProductPackage(
                Guid.Parse("6535b510-4ac8-498a-bdfb-9e61870fd359"),
                "حبة",
                "Single", 
                1,
                Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),
                Guid.Parse("a64630c2-d13d-42f0-9416-4f8aac39a8e6"),
                "baryce@gmail.com"),
            new ProductPackage(
                Guid.Parse("e789ae8d-b85e-45ed-a70e-6d3a52a5c3e5"),
                "كرتون ابو 10",
                "Pack of 10", 
                10,
                Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"),
                Guid.Parse("a64630c2-d13d-42f0-9416-4f8aac39a8e6"),
                "baryce@gmail.com"),
        };
    public static IEnumerable<Variant> Variants =>
        new List<Variant>
        {
            //new Variant(
            //    Guid.Parse("b01de546-9b32-4180-88fd-fad74cd3a0eb"),"الحجم","Size",Guid.Parse("4C3D205F-7E2B-42C2-A081-1700B229D91E"), "baryce@gmail.com",""),
            //new Variant(Guid.NewGuid(),"عصير منقة 500 مل", ""),
            //new Variant(Guid.NewGuid(),"عصير مشكل 250 مل", ""),
        };
    public static IEnumerable<ProductSku> ProductSkus =>
        new List<ProductSku>
        {
            //new ProductSku(
            //    Guid.Parse("2e18ef43-3713-4507-816c-b9a37331433d"),
            //    Guid.Parse("5c8b8a8a-4128-445a-914f-389107ed7363"),
            //    Guid.Parse("b01de546-9b32-4180-88fd-fad74cd3a0eb"),
            //    Guid.Parse("6535b510-4ac8-498a-bdfb-9e61870fd359"),
            //    "250",
            //    1,
            //    GenerateSKU.Generate("عصير مشكل", "الصافي","الحجم", "250","مل","Single"),
            //    GenerateSKU.Generate("MixedJuice", "Alsafi","Size", "250","ML","Single"),
            //    true,
            //    "baryce@gmail.com"),

            //new ProductSku(Guid.Parse("11ef79b6-25db-4768-b224-fd651428c8d3"),
            //    Guid.Parse("e0f16e7c-e899-4a95-a908-78072e693a25"),
            //    Guid.Parse("b01de546-9b32-4180-88fd-fad74cd3a0eb"),
            //    Guid.Parse("e789ae8d-b85e-45ed-a70e-6d3a52a5c3e5"),
            //    "500",
            //    2,
            //    GenerateSKU.Generate("عصير منقة","المراعي","الحجم","500","مل","Pack of 10"),
            //    GenerateSKU.Generate("Mango Juice","Almaraee","Size","500","ML","Pack of 10"),
            //    true,
            //    "baryce@gmail.com"),
        };


}
