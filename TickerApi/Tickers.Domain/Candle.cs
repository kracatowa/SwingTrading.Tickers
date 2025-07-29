namespace Tickers.Domain
{
    public class Candle
    {
        public Guid Id { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public float Open { get; private set; }
        public float High { get; private set; }
        public float Low { get; private set; }
        public float Close { get; private set; }
        public int Volume { get; private set; }
        public float Dividends { get; private set; }
        public float StockSplits { get; private set; }

        public Candle(DateTimeOffset date, float open, float high, float low, float close, int volume, float dividends, float stockSplits)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Dividends = dividends;
            StockSplits = stockSplits;
        }

        protected Candle() { }
    }
}
