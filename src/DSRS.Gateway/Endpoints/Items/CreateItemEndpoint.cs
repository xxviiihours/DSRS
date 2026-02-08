using System.ComponentModel.DataAnnotations;

namespace DSRS.Gateway.Endpoints.Items;

public class CreateItemEndpoint
{
}

public class CreatePlayerRequest
{
    public const string Route = "/items";

    [Required]
    public string Name { get; set; } = String.Empty;
    public decimal BasePrice { get; set; } = 0;
}
