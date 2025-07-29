using MediatR;

namespace Tickers.Api.Commands
{
    public class AddTickerCommand : IRequest<Unit>
    {
        public string Symbol { get; set; }
    }
}
