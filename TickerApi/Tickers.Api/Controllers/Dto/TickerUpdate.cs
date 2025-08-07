using Tickers.Domain.Intervals;

namespace Tickers.Api.Controllers.Dto
{
    public record TickerUpdate(string Ticker, IntervalTypes Interval, Candle[] Candles);
}
