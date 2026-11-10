   using Microsoft.EntityFrameworkCore;
   using MonPremierProjet.Models;

   namespace MonPremierProjet.Data;

   public class AppDbContext(DbContextOptions<AppDbContext> options)
       : DbContext(options)
   {
       public DbSet<Todo> Todos => Set<Todo>();
   }