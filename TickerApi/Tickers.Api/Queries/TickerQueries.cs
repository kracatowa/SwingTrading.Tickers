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
                                        .FirstOrDefault() ?? default
                            })
                            .ToListAsync();

            return tickers;
        }

        public async Task<List<Ticker>> GetTickersLimitedCandles(int candleLimit, IntervalTypes intervalType)
        {
            var tickers = await tickerContext.Tickers
                .Include(t => t.Intervals.Where(i => i.IntervalType == intervalType))
                    .ThenInclude(i => i.Candles.OrderByDescending(c => c.Date).Take(candleLimit))
                .ToListAsync();

            var result = new List<Ticker>();

            foreach (var ticker in tickers)
            {
                var tickerResult = new Ticker
                {
                    Symbol = ticker.Symbol,
                    IntervalType = intervalType
                };

                var queryCandleResult = ticker.Intervals.First(x => x.IntervalType == intervalType).Candles;

                var candles = queryCandleResult
                    .OrderByDescending(c => c.Date)
                    .Take(candleLimit)
                    .Select(c => new Candle
                    {
                        Date = c.Date,
                        Open = c.Open,
                        High = c.High,
                        Low = c.Low,
                        Close = c.Close,
                        Volume = c.Volume,
                        Dividends = c.Dividends,
                        StockSplits = c.StockSplits
                    })
                    .ToList();

                tickerResult.Candles.AddRange(candles);


                result.Add(tickerResult);
            }

            return result;
        }

        public class SymbolPeriodChecker
        {
            public string Symbol { get; set; }
            public DateTimeOffset Date { get; set; }
        }

        public class Ticker
        {
            public string Symbol { get; set; }
            public IntervalTypes IntervalType { get; set; }
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
