namespace Tickers.Api.Controllers.Dto
{
    public record TickerCandleUpdateRequest(string Ticker, int MissingDays);
}
