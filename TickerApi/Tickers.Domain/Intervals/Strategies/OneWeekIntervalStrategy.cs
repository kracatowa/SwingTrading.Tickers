namespace Tickers.Domain.Intervals.Strategies
{
    public class OneWeekIntervalStrategy : IIntervalTypeStrategy
    {
        public DateTime GetIntervalDate()
        {
            return DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek); // Returns the start of the current week (Monday)
        }
    }
}
