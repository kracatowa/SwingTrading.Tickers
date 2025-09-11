using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Tickers.Api.Commands;
using Tickers.Api.Controllers.Dto;
using Tickers.Api.Queries;
using Tickers.Api.Services;
using Tickers.Domain.Intervals;

namespace Tickers.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TickersController(ICandleFileService candleFileService,
                                   IMediator mediator,
                                   ITickerQueries tickerQueries,
                                   ILogger<TickersController> logger) : ControllerBase
    {
        [HttpGet("AddTicker")]
        public async Task<IActionResult> AddTickersAsync(string symbol)
        {
            logger.LogInformation("AddTickersAsync called with symbol: {symbol}", symbol);

            if (string.IsNullOrWhiteSpace(symbol))
            {
                logger.LogWarning("AddTickersAsync failed due to empty symbol.");
                return BadRequest("Symbol cannot be null or empty.");
            }

            if (!Regex.IsMatch(symbol, "^[A-Za-z.]{1,4}$"))
            {
                logger.LogWarning("AddTickersAsync failed due to invalid symbol format: {symbol}", symbol);
                return BadRequest("Symbol must be 1-4 characters, letters or periods only.");
            }

            var tickers = await tickerQueries.GetTicker(symbol);
            if (tickers != null)
            {
                return Conflict("Symbol already exists");
            }

            var command = new AddTickerCommand { Symbol = symbol };
            await mediator.Send(command);
            logger.LogInformation("Sent AddTickerCommand for ticker: {Ticker}", symbol);

            logger.LogInformation("AddTickersAsync completed successfully.");
            return Ok("Tickers added successfully.");
        }

        [HttpPost("AddTickers")]
        public async Task<IActionResult> AddTickersAsync([FromBody] ProcessFileEvent addTickersEvent)
        {
            logger.LogInformation("AddTickersAsync called with FilePath: {FilePath}", addTickersEvent.FilePath);

            if (string.IsNullOrWhiteSpace(addTickersEvent.FilePath))
            {
                logger.LogWarning("AddTickersAsync failed due to empty FilePath.");
                return BadRequest("File path cannot be null or empty.");
            }

            var tickers = candleFileService.LoadTickers(addTickersEvent.FilePath);
            logger.LogInformation("Loaded {Count} tickers from file.", tickers.Count);

            foreach (var ticker in tickers)
            {
                var command = new AddTickerCommand { Symbol = ticker };
                await mediator.Send(command);
                logger.LogInformation("Sent AddTickerCommand for ticker: {Ticker}", ticker);
            }

            logger.LogInformation("AddTickersAsync completed successfully.");
            return Ok("Tickers added successfully.");
        }

        [HttpPost("AddCandles")]
        public async Task<IActionResult> AddCandlesAsync([FromBody] ProcessFileEvent addTickersEvent)
        {
            logger.LogInformation("AddCandlesAsync called with FilePath: {FilePath}", addTickersEvent.FilePath);

            if (string.IsNullOrWhiteSpace(addTickersEvent.FilePath))
            {
                logger.LogWarning("AddCandlesAsync failed due to empty FilePath.");
                return BadRequest("File path cannot be null or empty.");
            }

            var tickerInformation = candleFileService.LoadTickerInformations(addTickersEvent.FilePath);

            var command = new AddCandlesCommand { TickerInformations = tickerInformation };
            await mediator.Send(command);
            logger.LogInformation("Sent AddCandlesCommand for ticker: {Ticker}", tickerInformation.Ticker);

            logger.LogInformation("AddCandlesAsync completed successfully.");
            return Ok("Candles added successfully.");
        }

        [HttpGet("GetTickersNeedingCandleUpdates/{intervalType}")]
        public async Task<ActionResult<List<TickerCandleUpdateRequest>>> GetTickersNeedingCandleUpdate(IntervalTypes intervalType)
        {
            logger.LogInformation("GetTickersNeedingCandleUpdate called with intervaltype: {Interval}", intervalType.ToString().ToUpperInvariant());

            var tickers = await tickerQueries.GetTickersNeedingCandleUpdates(intervalType);
            logger.LogInformation("Retrieved {Count} tickers needing updates.", tickers.Count);

            var tickersToBeUpdated = new List<TickerCandleUpdateRequest>();

            foreach (var ticker in tickers)
            {
                var missingDays = (DateTime.UtcNow - ticker.Date.UtcDateTime).Days;

                tickersToBeUpdated.Add(new TickerCandleUpdateRequest(ticker.Symbol, missingDays));
                logger.LogInformation("Ticker {Symbol} added to update list with missing days: {missingDays}", ticker.Symbol, missingDays);
            }

            logger.LogInformation("GetTickersNeedingCandleUpdate completed successfully.");
            return tickersToBeUpdated;
        }

        [HttpGet("GetTickersLimitCandles/{candleLimit}/{intervalTypes}")]
        public async Task<ActionResult<List<TickerQueries.Ticker>>> GetTickersNeedingCandleUpdate(int candleLimit, IntervalTypes intervalTypes)
        {
            logger.LogInformation("GetTickersLimitCandles called with candleLimit: {CandleLimit}, intervalTypes: {IntervalType}", candleLimit, intervalTypes.ToString().ToUpperInvariant());

            var tickers = await tickerQueries.GetTickersLimitedCandles(candleLimit, intervalTypes);
            logger.LogInformation("Retrieved {Count} tickers with limited candles.", tickers.Count);

            return tickers;
        }
    }
}
