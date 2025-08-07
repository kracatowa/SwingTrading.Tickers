using Tickers.Domain;
using Tickers.Domain.Intervals;
using static Tickers.Api.Queries.TickerQueries;

namespace Tickers.Api.Queries
{
    public interface ITickerQueries
    {
        Task<List<SymbolPeriodChecker>> GetTickersNeedingCandleUpdates(IntervalTypes intervalTypes);
    }
}
