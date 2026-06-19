using System.Reflection;
using FluentAssertions;
using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Application.Interfaces;
using LibraryManager.Application.Services;
using LibraryManager.Domain.Models;
using Moq;

namespace LibraryManager.Application.Tests
{
    public class ReaderServiceTests
    {
        private readonly Mock<IBaseRepository<Reader>> _readerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReaderService _service;

        public ReaderServiceTests()
        {
            _readerRepositoryMock = new Mock<IBaseRepository<Reader>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _service = new ReaderService(
            _readerRepositoryMock.Object,
            _unitOfWorkMock.Object
            );
        }

        private void AddBorrowingToReader(Reader reader, BookBorrowing borrowing)
        {
            var field = typeof(Reader).GetField("_bookBorrowings", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<BookBorrowing>)field!.GetValue(reader)!;
            list.Add(borrowing);
        }

        #region UpdateReaderProfile Tests

        [Fact]
        public async Task UpdateReaderProfile_ShouldThrowException_WhenReaderNotFound()
        {
            // Arrange
            var request = new UpdateReaderRequest
            {
                Id = 1
            };
            _readerRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reader)null!);

            // Act
            Func<Task> act = async () => await _service.UpdateReaderProfile(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Reader not found");
        }

        [Theory]
        [InlineData(ReaderState.Blocked)]
        [InlineData(ReaderState.DeletedByUser)]
        public async Task UpdateReaderProfile_ShouldThrowException_WhenReaderIsNotActive(ReaderState nonActiveState)
        {
            // Arrange
            var request = new UpdateReaderRequest
            {
                Id = 1
            };
            var existingReader = new Reader
            {
                Id = 1, Status = nonActiveState
            };

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingReader);

            // Act
            Func<Task> act = async () => await _service.UpdateReaderProfile(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Reader is not active");
        }

        [Fact]
        public async Task UpdateReaderProfile_ShouldAdaptChangesAndSave_WhenReaderIsActive()
        {
            // Arrange
            var request = new UpdateReaderRequest
            {
                Id = 1, Name = "Updated Name"
            };
            var existingReader = new Reader
            {
                Id = 1, Name = "Old Name", Status = ReaderState.Active
            };

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingReader);

            // Act
            await _service.UpdateReaderProfile(request, CancellationToken.None);

            // Assert
            existingReader.Name.Should().Be("Updated Name");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteReader Tests

        [Fact]
        public async Task DeleteReader_ShouldThrowException_WhenReaderNotFound()
        {
            // Arrange
            _readerRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reader)null!);

            // Act
            Func<Task> act = async () => await _service.DeleteReader(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Reader not found");
        }

        [Fact]
        public async Task DeleteReader_ShouldThrowException_WhenReaderHasUnreturnedBooks()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };
            var activeLoan = new BookBorrowing
            {
                IsReturned = false
            };
            AddBorrowingToReader(reader, activeLoan);

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            Func<Task> act = async () => await _service.DeleteReader(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cannot delete reader while they have unreturned books.");
        }

        [Fact]
        public async Task DeleteReader_ShouldThrowException_WhenReaderHasUnpaidFines()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };
            var borrowing = new BookBorrowing
            {
                IsReturned = true
            };
            borrowing.AddFine(1, "Late return", 15.50m);
            AddBorrowingToReader(reader, borrowing);

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            Func<Task> act = async () => await _service.DeleteReader(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cannot delete reader. Unpaid fines: 15*");
        }

        [Fact]
        public async Task DeleteReader_ShouldSetStatusToDeleted_WhenReaderHasNoLoansOrFines()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };
            var borrowing = new BookBorrowing
            {
                IsReturned = true
            };
            AddBorrowingToReader(reader, borrowing);

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            await _service.DeleteReader(1, CancellationToken.None);

            // Assert
            reader.Status.Should().Be(ReaderState.DeletedByUser);
            _readerRepositoryMock.Verify(x => x.Update(reader), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region BlockReader Tests

        [Fact]
        public async Task BlockReader_ShouldThrowException_WhenReaderNotFound()
        {
            // Arrange
            _readerRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reader)null!);

            // Act
            Func<Task> act = async () => await _service.BlockReader(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Reader not found");
        }

        [Theory]
        [InlineData(ReaderState.Blocked)]
        [InlineData(ReaderState.DeletedByUser)]
        public async Task BlockReader_ShouldThrowException_WhenReaderIsNotActive(ReaderState nonActiveState)
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1, Status = nonActiveState
            };
            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            Func<Task> act = async () => await _service.BlockReader(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Reader is not active");
        }

        [Fact]
        public async Task BlockReader_ShouldThrowException_WhenReaderHasUnreturnedBooks()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };
            var activeLoan = new BookBorrowing
            {
                IsReturned = false
            };
            AddBorrowingToReader(reader, activeLoan);

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            Func<Task> act = async () => await _service.BlockReader(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cannot delete reader while they have unreturned books.");
        }

        [Fact]
        public async Task BlockReader_ShouldThrowException_WhenReaderHasUnpaidFines()
        {
            // Arrange
            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };
            var borrowing = new BookBorrowing
            {
                IsReturned = true
            };
            borrowing.AddFine(1, "Damage", 50.00m);
            AddBorrowingToReader(reader, borrowing);

            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

        }
    };
    #endregion
}