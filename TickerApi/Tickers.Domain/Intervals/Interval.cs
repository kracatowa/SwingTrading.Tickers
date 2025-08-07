namespace Tickers.Domain.Intervals
{
    public class Interval
    {
        public Guid Id { get; private set; }
        public IntervalTypes IntervalType { get; private set; }
        public List<Candle> Candles { get; private set; } = [];

        public Interval(IntervalTypes intervalType, List<Candle> candles)
        {
            IntervalType = intervalType;
            Candles = candles;
        }

        protected Interval() { }
    }
}
