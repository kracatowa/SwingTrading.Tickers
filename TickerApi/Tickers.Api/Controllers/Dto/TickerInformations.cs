using Tickers.Domain.Intervals;

namespace Tickers.Api.Controllers.Dto
{
    public class TickerInformations
    {
        public required string Ticker { get; set; }
        public required IntervalTypes Interval { get; set; }
        public required Candle[] Candles { get; set; }
    }
}
