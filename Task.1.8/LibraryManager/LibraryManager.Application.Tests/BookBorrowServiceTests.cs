
using FluentAssertions;
using LibraryManager.Application.Interfaces;
using LibraryManager.Application.Services;
using LibraryManager.Domain.Models;
using Moq;

namespace LibraryManager.Application.Tests
{
    public class BookBorrowServiceTests
    {
        private readonly Mock<IBaseRepository<Book>> _bookRepositoryMock;
        private readonly Mock<IBaseRepository<Reader>> _readerRepositoryMock;
        private readonly Mock<IBorrowingRepository> _borrowRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly BookBorrowService _service;

        private const string ValidIsbn = "978-5-699-12014-7";

        public BookBorrowServiceTests()
        {
            _bookRepositoryMock = new Mock<IBaseRepository<Book>>();
            _readerRepositoryMock = new Mock<IBaseRepository<Reader>>();
            _borrowRepositoryMock = new Mock<IBorrowingRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _service = new BookBorrowService(
            _bookRepositoryMock.Object,
            _readerRepositoryMock.Object,
            _borrowRepositoryMock.Object,
            _unitOfWorkMock.Object
            );
        }

        #region BorrowBook Tests

        [Fact]
        public async Task BorrowBook_ShouldThrowException_WhenBookNotFound()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null!);

            // Act
            Func<Task> act = async () => await _service.BorrowBook(1, 1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Book not found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task BorrowBook_ShouldThrowException_WhenReaderNotFound()
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(book);
            _readerRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reader)null!);

            // Act
            Func<Task> act = async () => await _service.BorrowBook(1, 1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Reader not found");
        }

        [Fact]
        public async Task BorrowBook_ShouldThrowException_WhenBookIsNotAvailable()
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;
            book.IsAvailable = false;

            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(book);
            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            Func<Task> act = async () => await _service.BorrowBook(1, 1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Book is not available");
        }

        [Theory]
        [InlineData(ReaderState.Blocked)]
        [InlineData(ReaderState.DeletedByUser)]
        public async Task BorrowBook_ShouldThrowException_WhenReaderIsNotActive(ReaderState nonActiveState)
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;

            var reader = new Reader
            {
                Id = 1, Status = nonActiveState
            };

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(book);
            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);

            // Act
            Func<Task> act = async () => await _service.BorrowBook(1, 1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("User is not Allowed To This Action");
        }

        [Fact]
        public async Task BorrowBook_ShouldThrowException_WhenBookIsAlreadyBorrowedByThisReader()
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;

            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(book);
            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);
            _borrowRepositoryMock
                .Setup(x => x.IsBorrowed(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.BorrowBook(1, 1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("This Book is already Borrowed");
        }

        [Fact]
        public async Task BorrowBook_ShouldSaveBorrowing_WhenDataIsValid()
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;

            var reader = new Reader
            {
                Id = 1, Status = ReaderState.Active
            };

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(book);
            _readerRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reader);
            _borrowRepositoryMock.Setup(x => x.IsBorrowed(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            await _service.BorrowBook(1, 1, CancellationToken.None);

            // Assert
            book.IsAvailable.Should().BeFalse();

            _borrowRepositoryMock.Verify(x => x.Add(It.Is<BookBorrowing>(b =>
                b.BookId == 1 &&
                b.ReaderId == 1 &&
                !b.IsReturned &&
                b.ReturnedAt == null)), Times.Once);

            _bookRepositoryMock.Verify(x => x.Update(book), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region ReturnBook Tests

        [Fact]
        public async Task ReturnBook_ShouldThrowException_WhenBorrowingNotFound()
        {
            // Arrange
            _borrowRepositoryMock
                .Setup(x => x.GetBorrowingWithBookByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((BookBorrowing)null!);

            // Act
            Func<Task> act = async () => await _service.ReturnBook(1, DateTime.Now, null, BookCondition.Good, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Borrowing not found");
        }

        [Fact]
        public async Task ReturnBook_ShouldUpdateStatusToReturned_WhenReturnedOnTimeWithGoodCondition()
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;
            book.IsAvailable = false;
            book.Condition = BookCondition.New;

            var borrowing = new BookBorrowing
            {
                Id = 5,
                Book = book,
                BorrowedAt = DateTime.Now.AddDays(-5), // 5 дней из 14 допустимых
                IsReturned = false
            };

            _borrowRepositoryMock.Setup(x => x.GetBorrowingWithBookByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(borrowing);
            var returnDate = DateTime.Now;

            // Act
            await _service.ReturnBook(5, returnDate, null, BookCondition.Good, CancellationToken.None);

            // Assert
            borrowing.IsReturned.Should().BeTrue();
            borrowing.ReturnedAt.Should().Be(returnDate);
            borrowing.Fines.Should().BeEmpty();

            book.Condition.Should().Be(BookCondition.Good);
            book.IsAvailable.Should().BeTrue();

            _borrowRepositoryMock.Verify(x => x.Update(borrowing), Times.Once);
            _bookRepositoryMock.Verify(x => x.Update(book), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReturnBook_ShouldMakeBookUnavailableAndAddFine_WhenConditionIsWellWorn()
        {
            // Arrange
            var book = Book.Create("Test Title", ValidIsbn, "Author", 2023, 1);
            book.Id = 1;
            book.IsAvailable = false;
            book.Condition = BookCondition.Good;

            var borrowing = new BookBorrowing
            {
                Id = 10, Book = book, BorrowedAt = DateTime.Now.AddDays(-5), IsReturned = false
            };

            _borrowRepositoryMock.Setup(x => x.GetBorrowingWithBookByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(borrowing);

            // Act
            await _service.ReturnBook(10, DateTime.Now, "Torn cover", BookCondition.WellWorn, CancellationToken.None);

            // Assert
            book.Condition.Should().Be(BookCondition.WellWorn);
            book.IsAvailable.Should().BeFalse();
        }
        #endregion 
    };
}
