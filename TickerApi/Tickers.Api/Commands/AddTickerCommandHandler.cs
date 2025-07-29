using MediatR;
using Tickers.Infrastructure.Repositories;

namespace Tickers.Api.Commands
{
    public class AddTickerCommandHandler(ITickerRepository tickerRepository, ILogger<AddTickerCommandHandler> logger) : IRequestHandler<AddTickerCommand, Unit>
    {
        public async Task<Unit> Handle(AddTickerCommand request, CancellationToken cancellationToken)
        {
            if (await tickerRepository.GetTickerAsync(request.Symbol, cancellationToken) is not null)
            {
                logger.LogInformation("Ticker with symbol {symbol} already exists.", request.Symbol);
            }
            else
            {
                await tickerRepository.AddTickerAsync(new Domain.Ticker(request.Symbol), cancellationToken);
                await tickerRepository.GetDbContext().SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
