namespace FreshCart.Common.Reviews;

public sealed class ProductReview
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public Guid OrderItemId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedOn { get; set; }
}
