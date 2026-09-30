using Conmo;
using Library.Models;

namespace Library.Models {
    public class Book {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int Year { get; set; }

        public override string ToString() => $"{BookId}. {Title} by {Author} ({Year})";
    }
}

namespace Library.Services {
    public class BookServices {
        private static readonly List<Book> Books = new();

        public bool Add(Book book) {
            book.BookId = Books.Count + 1;
            Books.Add(book);
            return true;
        }

        public List<Book> GetAll() => Books;

        public Book? Get(int id) => Books.FirstOrDefault(book => book.BookId == id);

        public bool Remove(int id) => Books.RemoveAll(book => book.BookId == id) > 0;
    }
}

namespace Library {
    public class LibraryMenu : Menu {
        public void CreateBook() { }

        public void ViewBook() { }

        public int RemoveBook() {
            Console.Write("Book id: ");
            return int.Parse(Console.ReadLine() ?? "0");
        }
    }

    public static class Program {
        public static void Main() => new LibraryMenu().Show();
    }
}
