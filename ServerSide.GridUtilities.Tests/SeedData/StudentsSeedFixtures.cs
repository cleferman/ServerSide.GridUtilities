using Microsoft.EntityFrameworkCore;
using ServerSide.GridUtilities.Tests.DatabaseContext;

namespace ServerSide.GridUtilities.Tests.SeedData
{
    public class StudentsSeedFixtures : IDisposable
    {
        public readonly StudentsContext StudentsContext;
        public StudentsSeedFixtures(DbContextOptions options)
        {
            StudentsContext = new StudentsContext(options);
            StudentsContext.Database.EnsureDeleted();
            var students = GenerateStudents(1000);
            StudentsContext!.Students.AddRange(students);
            StudentsContext.SaveChanges();
        }

        public void Dispose()
        {
            StudentsContext.Dispose();
        }

        private List<Student> GenerateStudents(int count)
        {
            var students = new List<Student>();

            for (int i = 0; i < count; i++)
            {
                var student = new Student
                {
                    // Generate data for each student based on i
                    Id = i + 1,
                    Name = $"Student {i + 1}",
                    Grade = GetRandomGrade(),
                    Age = GetRandomAge(),
                    EnrollmentDate = GetRandomDateBetweenYears(2020, 2023)
                };

                students.Add(student);
            }

            return students;
        }

        private int GetRandomGrade()
        {
            // Generate a random grade for the student
            var grades = new int[] { 5, 6, 7, 8, 9, 10 };
            var random = new Random();
            var index = random.Next(grades.Length);

            return grades[index];
        }

        private int GetRandomAge()
        {
            // Generate a random age for the student between 18 and 25
            var random = new Random();
            return random.Next(18, 26);
        }

        private DateTime GetRandomDateBetweenYears(int startYear, int endYear)
        {
            var random = new Random();
            var start = new DateTime(startYear, 1, 1);
            var end = new DateTime(endYear, 12, 31);
            var range = (end - start).Days;

            return start.AddDays(random.Next(range));
        }
    }
}