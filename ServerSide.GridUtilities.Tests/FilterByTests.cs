using Microsoft.EntityFrameworkCore;
using ServerSide.GridUtilities.Extensions;
using ServerSide.GridUtilities.Grid;
using ServerSide.GridUtilities.Grid.Constants;
using ServerSide.GridUtilities.Tests.DatabaseContext;
using ServerSide.GridUtilities.Tests.SeedData;

namespace ServerSide.GridUtilities.Tests
{
    public class FilterByTests
    {
        private DbContextOptions<StudentsContext> _options;
        private StudentsSeedFixtures studentsFixture;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            studentsFixture = new StudentsSeedFixtures(_options);
        }

        [Test]
        public void FilterBy_TextEquals_Success()
        {
            var model = new FilterModel
            {
                FieldName = "Name",
                FilterType = FilterType.Text,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.Equals,
                            Values = ["Student 1"]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.Count == 1);
            Assert.That(results[0].Name == "Student 1");
        }

        [Test]
        public void FilterBy_TextContains_Success()
        {
            var model = new FilterModel
            {
                FieldName = "Name",
                FilterType = FilterType.Text,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.Contains,
                            Values = ["Student"]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var query = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel);
            var results = query.ToList();

            Assert.That(results.Count == 1000);
            Assert.That(results.All(r => r.Name.Contains("Student")));
        }

        [Test]
        public void FilterBy_TextStartsWith_Success()
        {
            var model = new FilterModel
            {
                FieldName = "Name",
                FilterType = FilterType.Text,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.StartsWith,
                            Values = ["Student 1"]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.All(t => t.Name.StartsWith("Student 1")));
        }

        [Test]
        public void FilterBy_TextEndsWith_Success()
        {
            var model = new FilterModel
            {
                FieldName = "Name",
                FilterType = FilterType.Text,
                Conditions =
                    [
                        new Condition
                                    {
                                        FilterMethod = FilterMethod.EndsWith,
                                        Values = ["dent 1"]
                                    }
                    ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.Count == 1);
            Assert.That(results[0].Name.EndsWith("dent 1"));
        }

        [Test]
        public void FilterBy_TextNotEqual_Success()
        {
            var model = new FilterModel
            {
                FieldName = "Name",
                FilterType = FilterType.Text,
                Conditions =
                    [
                        new Condition
                            {
                                FilterMethod = FilterMethod.NotEqual,
                                Values = ["Student 2"]
                            }
                    ]
            };

            var filterModel = new List<FilterModel> { model };

            var query = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel);
            var results = query.ToList();

            Assert.That(results.Count == 999);
            Assert.That(results.All(r => r.Name != "Student 2"));
        }

        [Test]
        public void FilterBy_DateEquals_Success()
        {
            var enrollmentDate = new DateTime(2019, 1, 1);
            var model = new FilterModel
            {
                FieldName = "EnrollmentDate",
                FilterType = FilterType.Date,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.Equals,
                            Values = [enrollmentDate.ToString()]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.Count == 1);
            Assert.That(results[0].EnrollmentDate.Equals(enrollmentDate));
        }

        [Test]
        public void FilterBy_DateGreaterThan_Success()
        { 
            var enrollmentDate = new DateTime(2019, 12, 31);

            var model = new FilterModel
            {
                FieldName = "EnrollmentDate",
                FilterType = FilterType.Date,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.GreaterThan,
                            Values = [enrollmentDate.ToString()]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.Count == 998);
            Assert.That(results.All(r => r.EnrollmentDate > enrollmentDate));
        }

        [Test]
        public void FilterBy_DateLessThan_Success()
        {
            var enrollmentDate = new DateTime(2019, 12, 31);
            var model = new FilterModel
            {
                FieldName = "EnrollmentDate",
                FilterType = FilterType.Date,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.LessThan,
                            Values = [enrollmentDate.ToString()]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.Count == 2);
            Assert.That(results.All(r => r.EnrollmentDate < enrollmentDate));
        }

        [Test]
        public void FilterBy_DateNotEqual_Success()
        {
            var enrollmentDate = new DateTime(2019, 1, 1);

            var model = new FilterModel
            {
                FieldName = "EnrollmentDate",
                FilterType = FilterType.Date,
                Conditions =
                [
                    new Condition
                        {
                            FilterMethod = FilterMethod.NotEqual,
                            Values = [enrollmentDate.ToString()]
                        }
                ]
            };

            var filterModel = new List<FilterModel> { model };

            var results = studentsFixture.StudentsContext.Students
                .FilterBy(filterModel)
                .ToList();

            Assert.That(results.Count == 999);
            Assert.That(results.All(r => !r.EnrollmentDate.Equals(enrollmentDate)));
        }

        [TearDown]
        public void FinishTest()
        {
            studentsFixture.Dispose();
        }
    }
}