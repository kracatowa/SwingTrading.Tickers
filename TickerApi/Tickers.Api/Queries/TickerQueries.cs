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

            // Directly filter intervals in the query to reduce unnecessary projections
            var tickers = await tickerContext.Tickers
                .SelectMany(ticker => ticker.Intervals
                    .Where(interval => interval.IntervalType == intervalType && interval.LastUpdate < intervalDate)
                    .Select(interval => new SymbolPeriodChecker
                    {
                        Symbol = ticker.Symbol,
                        Date = interval.LastUpdate
                    }))
                .ToListAsync();

            return tickers;
        }

        public async Task<List<Ticker>> GetTickersLimitedCandles(int candleLimit, IntervalTypes intervalType)
        {
            var tickers = await tickerContext.Tickers
                .Select(t => new
                {
                    t.Symbol,
                    Interval = t.Intervals
                        .Where(i => i.IntervalType == intervalType)
                        .Select(i => new
                        {
                            i.IntervalType,
                            Candles = i.Candles
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
                                .ToList()
                        })
                        .FirstOrDefault()
                })
                .Where(t => t.Interval != null)
                .ToListAsync();

            // Materialize results into the expected DTO
            var result = new List<Ticker>(tickers.Count);
            foreach (var t in tickers)
            {
                if (t.Interval != null) 
                {
                    var tickerResult = new Ticker
                    {
                        Symbol = t.Symbol,
                        IntervalType = t.Interval.IntervalType,
                        Candles = t.Interval.Candles
                    };
                    result.Add(tickerResult);
                }
            }

            return result;
        }

        public async Task<Domain.Ticker?> GetTicker(string symbol)
        {
            return await tickerContext.Tickers.FirstOrDefaultAsync(x => x.Symbol == symbol);
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
