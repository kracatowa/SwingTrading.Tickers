using MediatR;
using Tickers.Infrastructure.Repositories;

namespace Tickers.Api.Commands
{
    public class AddCandlesCommandHandler(ITickerRepository tickerRepository, ILogger<AddCandlesCommandHandler> logger) : IRequestHandler<AddCandlesCommand, Unit>
    {
        public async Task<Unit> Handle(AddCandlesCommand request, CancellationToken cancellationToken)
        {
            var ticker = await tickerRepository.GetTickerAsync(request.TickerInformations.Ticker, cancellationToken)
                ?? throw new ArgumentException($"Ticker with symbol {request.TickerInformations.Ticker} not found.");

            var newCandles = request.TickerInformations.Candles.Select(candle => new Domain.Candle(
                candle.Date,
                candle.Open,
                candle.High,
                candle.Low,
                candle.Close,
                candle.Volume,
                candle.Dividends,
                candle.StockSplits)).ToList();

            ticker.AddCandles(newCandles, request.TickerInformations.Interval);

            await tickerRepository.GetDbContext().SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
