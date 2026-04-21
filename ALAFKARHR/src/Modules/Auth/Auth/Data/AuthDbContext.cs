
namespace Auth.Data;
//add-migration AuthInitial -Project Auth -StartupProject Api -OutputDir Data/Migrations -Context AuthDbContext
public class AuthDbContext:IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Auth");
        
         //✅ Ensure schema exists
        //builder.HasAnnotation("Relational:Schema", "Auth");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
