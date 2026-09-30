# Conmo

A small C# library that turns plain classes into interactive console apps through reflection and naming conventions. You write a menu class whose method names say what they do; Conmo builds the numbered menu, prompts for input, and routes create, view and remove actions to your services.

## How it works

- **Menus.** A class that extends `Menu` becomes a screen. Its public methods are listed as numbered choices, with their names split into words (`CreateBook` becomes "Create Book"). `GoBack` is built in and leaves the screen.
- **CRUD by name.** A method named `Create<Model>`, `View<Model>`, `Remove<Model>` or `Select<Model>` is routed to a generic operation. Conmo finds the model type by name, prompts for each writable string or value-type property (skipping booleans and properties ending in `Id`), and calls the matching `<Model>Services` class: `Add`, `GetAll`, `Remove` or `Get`. For `Remove` and `Select`, the value your method returns is the id that gets passed on.
- **Navigation.** A method named `GoTo<MenuName>` opens that menu class. A value it returns is passed to the menu's constructor; a number or string is first resolved to a model with `Select<Model>`.
- **No registration.** A small container creates and caches menus and services by type name the first time they are needed.

## Example

```csharp
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
```

That is the whole app. Running it:

```
Library Menu

Press 0 to Create Book
Press 1 to View Book
Press 2 to Remove Book
Press 3 to Go Back

Choose: 0
Title: Dune
Author: Frank Herbert
Year: 1965
Book created successfully
```

Choosing 1 then prints `1. Dune by Frank Herbert (1965)` through `BookServices.GetAll`.

The example lives in [`samples/Library`](samples/Library):

```sh
dotnet run --project samples/Library
```

## Requirements

.NET 8 SDK.

## Status

Written in 2021 as an experiment in reflection-driven design. It works as shown above, but it is not published as a package or maintained as a product.

## License

[MIT](LICENSE)
