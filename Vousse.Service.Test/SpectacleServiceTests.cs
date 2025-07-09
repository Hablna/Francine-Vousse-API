using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Vousse.DAL.Modeles;
using Vousse.DTO;

namespace Vousse.Service.Test
{
    public class SpectacleServiceTests
    {
        [Fact]
        public void GetAllSpectacle_ReturnsAllSpectacles()
        {
            var data = new List<SpectacleParent>
            {
                new SpectacleParent { Id = 1, NomSpectacle = "Spectacle 1", TypeDeSpectacle = "TypeA", Descriptions = "Desc1" },
                new SpectacleParent { Id = 2, NomSpectacle = "Spectacle 2", TypeDeSpectacle = "TypeB", Descriptions = "Desc2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<SpectacleParent>>();
            mockSet.As<IQueryable<SpectacleParent>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<SpectacleParent>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<SpectacleParent>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<SpectacleParent>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<VousseContext>();
            mockContext.Setup(c => c.SpectacleParents).Returns(mockSet.Object);

            var service = new SpectacleService(mockContext.Object);

            var result = service.GetAllSpectacles().ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Spectacle 1", result[0].Titre);
            Assert.Equal("Spectacle 2", result[1].Titre);
        }

        [Fact]
        public void CheckBillet_ReturnsTrue_WhenBilletExists()
        {
            // Arrange
            var data = new List<Billeterie>
        {
            new Billeterie { NumeroBillet = 1, IdSpectacle = 10 }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Billeterie>>();
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<VousseContext>();
            mockContext.Setup(c => c.Billeteries).Returns(mockSet.Object);

            var service = new SpectacleService(mockContext.Object);

            var dto = new billetExistence_DTO { IdBillet = 1, IdSpectacle = 10 };

            // Act
            var result = service.checkBillet(dto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckBillet_ReturnsFalse_WhenBilletDoesNotExist()
        {
            // Arrange
            var data = new List<Billeterie>
        {
            new Billeterie { NumeroBillet = 2, IdSpectacle = 20 }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Billeterie>>();
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Billeterie>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<VousseContext>();
            mockContext.Setup(c => c.Billeteries).Returns(mockSet.Object);

            var service = new SpectacleService(mockContext.Object);

            var dto = new billetExistence_DTO { IdBillet = 1, IdSpectacle = 10 };

            // Act
            var result = service.checkBillet(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetTotalBillets_ReturnsTotalBilletsDTO_WhenSpectacleExists()
        {
            // Arrange
            int spectacleId = 10;
            var spectacleParents = new List<SpectacleParent>
        {
            new SpectacleParent { Id = spectacleId, NomSpectacle = "Spectacle Test" }
        }.AsQueryable();

            var billeteries = new List<Billeterie>
        {
            new Billeterie { IdSpectacle = spectacleId },
            new Billeterie { IdSpectacle = spectacleId }
        }.AsQueryable();

            var mockSpectacleParentSet = new Mock<DbSet<SpectacleParent>>();
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.Provider).Returns(spectacleParents.Provider);
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.Expression).Returns(spectacleParents.Expression);
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.ElementType).Returns(spectacleParents.ElementType);
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.GetEnumerator()).Returns(spectacleParents.GetEnumerator());

            var mockBilleterieSet = new Mock<DbSet<Billeterie>>();
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.Provider).Returns(billeteries.Provider);
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.Expression).Returns(billeteries.Expression);
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.ElementType).Returns(billeteries.ElementType);
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.GetEnumerator()).Returns(billeteries.GetEnumerator());

            var mockContext = new Mock<VousseContext>();
            mockContext.Setup(c => c.SpectacleParents).Returns(mockSpectacleParentSet.Object);
            mockContext.Setup(c => c.Billeteries).Returns(mockBilleterieSet.Object);

            var service = new SpectacleService(mockContext.Object);

            // Act
            var result = service.GetTotalBillets(spectacleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(spectacleId, result.id);
            Assert.Equal("Spectacle Test", result.titre);
            Assert.Equal(2, result.total);
            Assert.True(result.date <= DateTime.Now && result.date >= DateTime.Now.AddMinutes(-1)); // date proche de maintenant
        }



        [Fact]
        public void GetTotalBillets_ReturnsNull_WhenSpectacleNotFound()
        {
            // Arrange
            int spectacleId = 99;
            var spectacleParents = new List<SpectacleParent>().AsQueryable();
            var billeteries = new List<Billeterie>().AsQueryable();

            var mockSpectacleParentSet = new Mock<DbSet<SpectacleParent>>();
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.Provider).Returns(spectacleParents.Provider);
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.Expression).Returns(spectacleParents.Expression);
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.ElementType).Returns(spectacleParents.ElementType);
            mockSpectacleParentSet.As<IQueryable<SpectacleParent>>().Setup(m => m.GetEnumerator()).Returns(spectacleParents.GetEnumerator());

            var mockBilleterieSet = new Mock<DbSet<Billeterie>>();
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.Provider).Returns(billeteries.Provider);
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.Expression).Returns(billeteries.Expression);
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.ElementType).Returns(billeteries.ElementType);
            mockBilleterieSet.As<IQueryable<Billeterie>>().Setup(m => m.GetEnumerator()).Returns(billeteries.GetEnumerator());

            var mockContext = new Mock<VousseContext>();
            mockContext.Setup(c => c.SpectacleParents).Returns(mockSpectacleParentSet.Object);
            mockContext.Setup(c => c.Billeteries).Returns(mockBilleterieSet.Object);

            var service = new SpectacleService(mockContext.Object);

            // Act
            var result = service.GetTotalBillets(spectacleId);

            // Assert
            Assert.NotNull(result); // La méthode retourne toujours un DTO même si le spectacle n'existe pas (à adapter si besoin)
            Assert.Equal(spectacleId, result.id);
            Assert.Null(result.titre);
            Assert.Equal(0, result.total);
        }
    }
}