using System;
using System.Linq;
using Xunit;
using lab12v9;

namespace lab12v9.Tests
{
    public class ProjectRepositoryTests
    {
        [Fact]
        public void Add_ShouldAddProjectSuccessfully()
        {
            // Arrange
            IRepository<Project> repository = new InMemoryRepository<Project>();
            var project = new Project(1, "Test Project", DateTime.Now, DateTime.Now.AddMonths(1));

            // Act
            repository.Add(project);
            var result = repository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Project", result.Name);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            IRepository<Project> repository = new InMemoryRepository<Project>();

            // Act
            var result = repository.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProjects()
        {
            // Arrange
            IRepository<Project> repository = new InMemoryRepository<Project>();
            repository.Add(new Project(1, "Proj 1", DateTime.Now, DateTime.Now.AddDays(10)));
            repository.Add(new Project(2, "Proj 2", DateTime.Now, DateTime.Now.AddDays(20)));

            // Act
            var results = repository.GetAll().ToList();

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Update_ShouldModifyExistingProject()
        {
            // Arrange
            IRepository<Project> repository = new InMemoryRepository<Project>();
            repository.Add(new Project(1, "Old Name", DateTime.Now, DateTime.Now.AddDays(10)));

            // Act
            var project = repository.GetById(1);
            if (project != null)
            {
                project.Name = "New Name";
                repository.Update(project);
            }

            var updated = repository.GetById(1);

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveProject_WhenIdExists()
        {
            // Arrange
            IRepository<Project> repository = new InMemoryRepository<Project>();
            repository.Add(new Project(1, "To Delete", DateTime.Now, DateTime.Now.AddDays(10)));

            // Act
            repository.Delete(1);
            var result = repository.GetById(1);

            // Assert
            Assert.Null(result);
        }
    }
}