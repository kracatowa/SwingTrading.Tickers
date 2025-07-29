namespace Tickers.Domain.Intervals.Strategies
{
    public class OneDayIntervalStrategy : IIntervalTypeStrategy
    {
        public DateTime GetIntervalDate()
        {
            return DateTime.UtcNow.Date; // Returns the current date at midnight (00:00:00)
        }
    }
}
