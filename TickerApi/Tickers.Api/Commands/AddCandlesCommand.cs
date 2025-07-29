using MediatR;
using Tickers.Api.Controllers.Dto;

namespace Tickers.Api.Commands
{
    public record AddCandlesCommand : IRequest<Unit>
    {
        public required TickerInformations TickerInformations { get; init; }
    }
}
