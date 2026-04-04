using DSRS.SharedKernel.Abstractions;
using Vogen;

namespace DSRS.Domain.ValueObjects;

[ValueObject<string>(Conversions.SystemTextJson)]
public partial class FriendshipPair
{
    public static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Validation.Invalid("FriendshipPair cannot be empty");

        // Expected format: "guid1|guid2" where guid1 < guid2
        var parts = value.Split('|');
        if (parts.Length != 2)
            return Validation.Invalid("Invalid FriendshipPair format");

        if (!Guid.TryParse(parts[0], out var id1) || !Guid.TryParse(parts[1], out var id2))
            return Validation.Invalid("Invalid GUIDs in FriendshipPair");

        if (id1 == id2)
            return Validation.Invalid("Cannot create friendship pair with same player");

        // Validate ordering (must be in canonical form)
        if (id1.CompareTo(id2) >= 0)
            return Validation.Invalid("FriendshipPair must be in canonical form (smaller ID first)");

        return Validation.Ok;
    }
    public static FriendshipPair Create(PlayerId player1, PlayerId player2)
    {
        if (player1.Value == player2.Value)
            throw new InvalidOperationException("Cannot create friendship with the same player");

        // Order the IDs: smaller first
        var id1 = player1.Value;
        var id2 = player2.Value;

        var (first, second) = id1.CompareTo(id2) < 0
            ? (id1, id2)
            : (id2, id1);

        // Create canonical string representation
        var pairString = $"{first}|{second}";
        return From(pairString);
    }

    public static FriendshipPair CreateUnsafe(PlayerId player1, PlayerId player2)
    {
        var pairString = $"{player1.Value}|{player2.Value}";
        return From(pairString);
    }

    public PlayerId GetFirstPlayer()
    {
        var parts = Value.Split('|');
        return PlayerId.From(Guid.Parse(parts[0]));
    }

    public PlayerId GetSecondPlayer()
    {
        var parts = Value.Split('|');
        return PlayerId.From(Guid.Parse(parts[1]));
    }

    public (PlayerId First, PlayerId Second) GetPlayers() =>
        (GetFirstPlayer(), GetSecondPlayer());

    public bool Contains(PlayerId playerId) =>
        GetFirstPlayer() == playerId || GetSecondPlayer() == playerId;

    public PlayerId GetOtherPlayer(PlayerId player)
    {
        var first = GetFirstPlayer();
        return first == player ? GetSecondPlayer() : first;
    }
    public bool IsSelfFriendship() => GetFirstPlayer() == GetSecondPlayer();
    private static string NormalizeInput(string input)
    {
        // todo: normalize (sanitize) your input;
        return input;
    }
}