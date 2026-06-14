using FluentAssertions;
using LibraryManager.Application.Interfaces;
using LibraryManager.Application.Services;
using LibraryManager.Domain.Models;
using Moq;

namespace LibraryManager.Application.Tests
{
    public class CatalogueServiceTests
    {
        private readonly Mock<IBaseRepository<Catalogue>> _catalogueRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CatalogueService _service;

        public CatalogueServiceTests()
        {
            _catalogueRepositoryMock = new Mock<IBaseRepository<Catalogue>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _service = new CatalogueService(
                _catalogueRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        #region CreateCatalogue Tests

        [Fact]
        public async Task CreateCatalogue_ShouldThrowException_WhenParentIdProvidedButParentNotFound()
        {
            // Arrange
            _catalogueRepositoryMock
                .Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Catalogue)null!);

            // Act
            Func<Task> act = async () => await _service.CreateCatalogue("Sci-Fi", 10, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Parent catalogue not found");
            _catalogueRepositoryMock.Verify(x => x.Add(It.IsAny<Catalogue>()), Times.Never);
        }

        [Fact]
        public async Task CreateCatalogue_ShouldSaveCatalogue_WhenNoParentIdProvided()
        {
            // Arrange
            _catalogueRepositoryMock
                .Setup(x => x.Add(It.IsAny<Catalogue>()))
                .Callback<Catalogue>(c => c.Id = 1);

            // Act
            var resultId = await _service.CreateCatalogue("Fiction", null, CancellationToken.None);

            // Assert
            resultId.Should().Be(1);
            _catalogueRepositoryMock.Verify(x => x.Add(It.Is<Catalogue>(c => c.Name == "Fiction" && c.ParentId == null)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCatalogue_ShouldSaveCatalogue_WhenValidParentIdProvided()
        {
            // Arrange
            var parent = new Catalogue { Id = 10, Name = "Books" };
            _catalogueRepositoryMock.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(parent);
            _catalogueRepositoryMock.Setup(x => x.Add(It.IsAny<Catalogue>())).Callback<Catalogue>(c => c.Id = 11);

            // Act
            var resultId = await _service.CreateCatalogue("Sci-Fi", 10, CancellationToken.None);

            // Assert
            resultId.Should().Be(11);
            _catalogueRepositoryMock.Verify(x => x.Add(It.Is<Catalogue>(c => c.Name == "Sci-Fi" && c.ParentId == 10)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetCatalogue Tests

        [Fact]
        public async Task GetCatalogue_ShouldThrowException_WhenCatalogueNotFound()
        {
            // Arrange
            _catalogueRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Catalogue)null!);

            // Act
            Func<Task> act = async () => await _service.GetCatalogue(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Catalogue not found");
        }

        [Fact]
        public async Task GetCatalogue_ShouldReturnCatalogue_WhenCatalogueExists()
        {
            // Arrange
            var catalogue = new Catalogue { Id = 1, Name = "History" };
            _catalogueRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(catalogue);

            // Act
            var result = await _service.GetCatalogue(1, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("History");
        }

        #endregion

        #region UpdateCatalogue Tests

        [Fact]
        public async Task UpdateCatalogue_ShouldThrowException_WhenCatalogueToUpdateNotFound()
        {
            // Arrange
            _catalogueRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Catalogue)null!);

            // Act
            Func<Task> act = async () => await _service.UpdateCatalogue(1, "New Name", null, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Catalogue not found");
        }

        [Fact]
        public async Task UpdateCatalogue_ShouldThrowException_WhenCatalogueIsItsOwnParent()
        {
            // Arrange
            var catalogue = new Catalogue { Id = 5, Name = "Drama" };
            _catalogueRepositoryMock.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(catalogue);

            // Act
            Func<Task> act = async () => await _service.UpdateCatalogue(5, "Updated Drama", 5, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Catalogue cannot be its own parent");
            _catalogueRepositoryMock.Verify(x => x.Update(It.IsAny<Catalogue>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCatalogue_ShouldUpdateFieldsAndSave_WhenDataIsValid()
        {
            // Arrange
            var catalogue = new Catalogue { Id = 5, Name = "Drama", ParentId = null };
            _catalogueRepositoryMock.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(catalogue);

            // Act
            await _service.UpdateCatalogue(5, "New Drama", 10, CancellationToken.None);

            // Assert
            catalogue.Name.Should().Be("New Drama");
            catalogue.ParentId.Should().Be(10);
            _catalogueRepositoryMock.Verify(x => x.Update(catalogue), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteCatalogue Tests

        [Fact]
        public async Task DeleteCatalogue_ShouldThrowException_WhenCatalogueToDeleteNotFound()
        {
            // Arrange
            _catalogueRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Catalogue)null!);

            // Act
            Func<Task> act = async () => await _service.DeleteCatalogue(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Catalogue not found");
        }

        [Fact]
        public async Task DeleteCatalogue_ShouldThrowException_WhenCatalogueHasBooks()
        {
            // Arrange
            var catalogue = new Catalogue { Id = 1, Name = "Novels" };
            var book = Book.Create("Valid Title", "978-5-699-12014-7", "Author", 2026, 1);
            
            var field = typeof(Catalogue).GetField("_bookCopies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var list = (System.Collections.Generic.List<Book>)field!.GetValue(catalogue)!;
            list.Add(book);

            _catalogueRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(catalogue);

            // Act
            Func<Task> act = async () => await _service.DeleteCatalogue(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cannot delete catalogue with books. Move or delete books first.");
            _catalogueRepositoryMock.Verify(x => x.Delete(It.IsAny<Catalogue>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCatalogue_ShouldDeleteAndSave_WhenCatalogueIsEmpty()
        {
            // Arrange
            var catalogue = new Catalogue { Id = 1, Name = "Empty Catalogue" };
            _catalogueRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(catalogue);

            // Act
            await _service.DeleteCatalogue(1, CancellationToken.None);

            // Assert
            _catalogueRepositoryMock.Verify(x => x.Delete(catalogue), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
