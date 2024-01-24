using Microsoft.EntityFrameworkCore;

namespace ServerSide.GridUtilities.Tests.DatabaseContext
{
    public class StudentsContext : DbContext
    {
        public StudentsContext(DbContextOptions options)
            :base(options)
        {
        }
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public int Grade { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
