namespace Tickers.Domain.Periods
{
    public class PeriodYFinanceMapper(PeriodTypes PeriodTypes, string ShortName)
    {
        public PeriodTypes PeriodTypes { get; } = PeriodTypes;
        public string YFinanceShortName { get; } = ShortName;

        public static readonly PeriodYFinanceMapper OneDay = new(PeriodTypes.OneDay, "1d");
        public static readonly PeriodYFinanceMapper FiveDays = new(PeriodTypes.FiveDays, "5d");
        public static readonly PeriodYFinanceMapper OneMonth = new(PeriodTypes.OneMonth, "1mo");
        public static readonly PeriodYFinanceMapper ThreeMonths = new(PeriodTypes.ThreeMonths, "3mo");
        public static readonly PeriodYFinanceMapper FiveYears = new(PeriodTypes.FiveYears, "5y");
    }
}
