using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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
                                        .FirstOrDefault() ?? default
                            })
                            .ToListAsync();

            return tickers;
        }

        public async Task<List<Ticker>> GetTickersLimitedCandles(int candleLimit)
        {
            var tickers = await tickerContext.Tickers.Include(x => x.Intervals).ToListAsync();

            var intervals = tickers.Select(x => x.Intervals.Select(x => x.Id));

            var intervalIds = new List<Guid>();

            foreach (var interval in intervals)
            {
                intervalIds.AddRange(interval);
            }

            var candles = tickerContext.Candles
                .OrderByDescending(c => c.Date)
                .Take(candleLimit)
                .Where(x => intervalIds.Contains(x.IntervalId))
                .ToList();

            var tickerResults = new List<Ticker>();

            foreach (var ticker in tickers)
            {
                var tickerResult = new Ticker { Symbol = ticker.Symbol };

                tickerResults.Add(tickerResult);

                foreach (var interval in ticker.Intervals)
                {
                    var correspondingCandles = candles.Where(x => x.IntervalId == interval.Id);

                    var candleResults = correspondingCandles.Select(x => new Candle
                    {
                        Open = x.Open,
                        Close = x.Close,
                        High = x.High,
                        Low = x.Low,
                        Date = x.Date,
                        Volume = x.Volume
                    }).ToList();

                    var intervalResult = new Interval
                    {
                        IntervalTypes = interval.IntervalType.GetDisplayName(),
                        Candles = candleResults
                    };

                    tickerResult.Intervals.Add(intervalResult);
                }
            }

            return tickerResults;
        }

        public class SymbolPeriodChecker
        {
            public string Symbol { get; set; }
            public DateTimeOffset Date { get; set; }
        }

        public class Ticker
        {
            public string Symbol { get; set; }
            public List<Interval> Intervals { get; set; } = [];
        }

        public class Interval
        {
            public string IntervalTypes { get; set; }
            public List<Candle> Candles { get; set; } = [];
        }

        public class Candle
        {
            public DateTimeOffset Date { get; set; }
            public float Open { get; set; }
            public float High { get; set; }
            public float Low { get; set; }
            public float Close { get; set; }
            public int Volume { get; set; }
            public float Dividends { get; set; }
            public float StockSplits { get; set; }
        }
    }
}
