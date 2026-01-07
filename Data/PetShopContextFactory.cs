using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PetShop.Data;

namespace PetShop;

public class PetShopContextFactory : IDesignTimeDbContextFactory<PetShopContext>
{
    public PetShopContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PetShopContext>();
        optionsBuilder.UseSqlite("Data Source=PetShop.db");
        
        return new PetShopContext(optionsBuilder.Options);
    }
}
