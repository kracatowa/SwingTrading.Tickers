namespace Tickers.Domain.Periods
{
    public class PeriodEvaluator
    {
        public static PeriodYFinanceMapper EvaluatePeriod(DateTimeOffset date)
        {
            var daysDifference = (DateTime.UtcNow - date.UtcDateTime).Days;

            return daysDifference switch
            {
                <= 1 => PeriodYFinanceMapper.OneDay,
                <= 5 => PeriodYFinanceMapper.FiveDays,
                <= 30 => PeriodYFinanceMapper.OneMonth,
                <= 90 => PeriodYFinanceMapper.ThreeMonths,
                _ => PeriodYFinanceMapper.FiveYears,
            };
        }
    }
}
