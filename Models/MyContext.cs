using Microsoft.EntityFrameworkCore;
using exam.Models;
namespace exam.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<Users> users {get;set;}

        public DbSet<Event> Event { get; set; }
        public DbSet<RSVP> RSVP {get;set;}
    }
}
