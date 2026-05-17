namespace CarPark.DTOs
{
    // ── CarPark ──────────────────────────────────────────────
    public record CarParkDto(
        int Id,
        string CarParkNo,
        string Address,
        decimal XCoord,
        decimal YCoord,
        string CarParkType,
        string TypeOfParkingSystem,
        string ShortTermParking,
        bool FreeParking,
        bool NightParking,
        int CarParkDecks,
        decimal? GantryHeight,
        bool CarParkBasement
    );

    // ── Favourites ───────────────────────────────────────────
    public record FavouriteDto(
        int CarParkId,
        string CarParkNo,
        string Address,
        DateTime SavedAt
    );

    public record AddFavouriteRequest(string CarParkNo);

    // ── Auth ─────────────────────────────────────────────────
    public record RegisterRequest(
        string Username,
        string Email,
        string Password
    );

    public record LoginRequest(
        string Username,
        string Password
    );

    public record AuthResponse(
        string Token,
        string Username
    );
}