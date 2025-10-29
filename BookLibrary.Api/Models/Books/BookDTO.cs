namespace BookLibrary.Api.Models.Books
{
    public sealed class BookDTO
    {
        public int Id { get; init; }
        public string Title { get; init; } = null!;
        public DateTime PublishedDate { get; init; }
        public string? AuthorName { get; init; } = string.Empty;
    }
}
