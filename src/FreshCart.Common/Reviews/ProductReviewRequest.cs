using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Reviews;

public sealed class ProductReviewRequest
{
    [Range(1, 5)]
    public byte Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}
