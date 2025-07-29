using MediatR;

namespace Tickers.Api.Commands.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = request.GetType().Name;

            logger.LogInformation("Handling request: {RequestType}", requestName);

            try
            {
                var returnValue = await next(cancellationToken);
                logger.LogInformation("Handled request: {RequestType}", requestName);
                return returnValue;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while handling request: {RequestType}", requestName);
                throw;
            }
        }
    }
}
