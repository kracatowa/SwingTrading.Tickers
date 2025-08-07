using Microsoft.EntityFrameworkCore;
using Tickers.Domain.Intervals;
using Tickers.Domain.Intervals.Strategies;
using Tickers.Infrastructure;

namespace Tickers.Api.Queries
{
    public class TickerQueries(TickerContext tickerContext) : ITickerQueries
    {
        public async Task<List<SymbolPeriodChecker>> GetTickersNeedingCandleUpdates(IntervalTypes intervalType)
        {
            var intervalStrategy = IntervalTypeStrategyFactory.Create(intervalType);

            var intervalDate = intervalStrategy.GetIntervalDate();

            var tickers = await tickerContext.Tickers
                            .Include(ticker => ticker.Intervals
                                .Where(interval => interval.IntervalType == intervalType))
                            .ThenInclude(interval => interval.Candles
                                .OrderByDescending(candle => candle.Date)
                                .Take(1))
                            .Where(ticker => ticker.Intervals.Any(interval => interval.IntervalType == intervalType &&
                                                                              !interval.Candles.Any(candle => candle.Date >= intervalDate)))
                            .Select(ticker => new SymbolPeriodChecker
                            {
                                Symbol = ticker.Symbol,
                                Date = ticker.Intervals
                                        .Where(t => t.IntervalType == intervalType)
                                        .SelectMany(c => c.Candles)
                                        .OrderByDescending(c => c.Date)
                                        .Select(c => (DateTimeOffset?)c.Date)
                                        .FirstOrDefault() ?? default})
                            .ToListAsync();

            return tickers;
        }

        public class SymbolPeriodChecker
        {
            public string Symbol { get; set; }
            public DateTimeOffset Date { get; set; }
        }
    }
}
