using System.Data.Entity;

namespace TicketSystem.Models
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("TicketDatabase")
        {
           
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Error> Errors { get; set; }

    }
}