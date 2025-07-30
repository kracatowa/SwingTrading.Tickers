using Tickers.Api.Controllers.Dto;

namespace Tickers.Api.Services
{
    public interface ICandleFileService
    {
        TickerUpdate LoadTickerInformations(string filePath);
        List<string> LoadTickers(string filePath);
    }
}
