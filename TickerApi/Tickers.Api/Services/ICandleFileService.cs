using Tickers.Api.Controllers.Dto;

namespace Tickers.Api.Services
{
    public interface ICandleFileService
    {
        List<TickerInformations> LoadTickerInformations(string filePath);
        List<string> LoadTickers(string filePath);
    }
}
