namespace Organization.Data.Seed;

public class InitialData
{
    public static Company Company => Company.Create(
                        Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"),
                        "ALAFKAR",
                        "ALAFKAR",
                        "logo",
                        "MAKKA",
                        10.1,
                        10.2,
                        "111111111111111",
                        "Afkr",
                        "SAR",
                        "info@alafkar.com",
                        "0511111111",
                        "ksa",
                        "Baryce@gmail.com");
    public static IEnumerable<Branch> Branches => new List<Branch>()
    {
        Branch.Create(
                        Guid.Parse("4f825206-5179-413a-bbff-02bb047bab64"),
                        "Main Branch",
                        "Main Branch",
                        "Makka",
                        12.1,
                        12.1,
                        "br",
                        "05123456789",
                        "info@mainbranch.com",
                        true,
                        Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"),
                        "baryce@gmail.com"
        ),
        Branch.Create(
                        Guid.Parse("3fe659f3-7f1c-4f02-b198-1c9cde4b86de"),
                        "Second Branch",
                        "Second Branch",
                        "Makka",
                        12.1,
                        12.1,
                        "br",
                        "05123456789",
                        "info@secondbranch.com",
                        false,
                        Guid.Parse("4c3d205f-7e2b-42c2-a081-1700b229d91e"),
                        "baryce@gmail.com"
        )
};
}
