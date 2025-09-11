namespace Tickers.Domain.Intervals
{
    public class Interval
    {
        public Guid Id { get; private set; }
        public IntervalTypes IntervalType { get; private set; }
        public DateTimeOffset LastUpdate { get;private set; }
        public List<Candle> Candles { get; private set; } = [];

        public Interval(IntervalTypes intervalType, List<Candle> candles, DateTimeOffset lastUpdate)
        {
            IntervalType = intervalType;
            Candles = candles;
            LastUpdate = lastUpdate;
        }

        protected Interval() { }

        public void SetLastUpdate(DateTimeOffset lastUpdate)
        {
            LastUpdate = lastUpdate;
        }
    }
}
