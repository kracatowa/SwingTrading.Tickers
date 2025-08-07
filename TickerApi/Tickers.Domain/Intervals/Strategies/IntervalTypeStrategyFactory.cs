namespace Tickers.Domain.Intervals.Strategies
{
    public class IntervalTypeStrategyFactory
    {
        public static IIntervalTypeStrategy Create(IntervalTypes intervalType)
        {
            return intervalType switch
            {
                IntervalTypes.OneDay => new OneDayIntervalStrategy(),
                IntervalTypes.OneWeek => new OneWeekIntervalStrategy(),
                _ => throw new ArgumentException("Invalid interval type", nameof(intervalType))
            };
        }
    }
}
