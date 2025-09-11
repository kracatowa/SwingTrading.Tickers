using Tickers.Domain.Exceptions;
using Tickers.Domain.Intervals;

namespace Tickers.Domain
{
    public class Ticker
    {
        public string Symbol { get; private set; }
        public List<Interval> Intervals { get; private set; } = [];

        public Ticker(string symbol)
        {
            Symbol = symbol;
            var baseInterval = new Interval(IntervalTypes.OneDay, [], default);
            Intervals.Add(baseInterval);
        }

        protected Ticker() { }
        
        public void AddCandles(List<Candle> candles, IntervalTypes interval)
        {
            if(candles == null || candles.Count == 0)
                throw new TickerDomainException("Trying to insert empty candles in the database");

            var updateDate = (candles.MaxBy(c => c.Date)?.Date) ?? 
                                throw new TickerDomainException("Trying to insert empty candles in the database");

            var existingInterval = Intervals.FirstOrDefault(i => i.IntervalType == interval);
            if (existingInterval != null)
            {
                var existingDates = existingInterval.Candles.Select(c => c.Date).ToHashSet();
                var newCandles = candles.Where(c => !existingDates.Contains(c.Date)).ToList();
                existingInterval.Candles.AddRange(newCandles);
                existingInterval.SetLastUpdate(updateDate);
            }
            else
            {
                Intervals.Add(new Interval(interval, candles, updateDate));
            }
        }
    }
}
