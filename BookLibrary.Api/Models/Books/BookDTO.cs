namespace BookLibrary.Api.Models.Books
{
    public sealed class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        public string? AuthorName { get; set; } = string.Empty;
    }
}
