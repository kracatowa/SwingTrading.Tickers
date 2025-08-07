using System.Text.Json;
using Tickers.Api.Controllers.Dto;

namespace Tickers.Api.Services
{
    public class CandleFileService : ICandleFileService
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

        public TickerUpdate LoadTickerInformations(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at path: {filePath}", filePath);

            try
            {
                var jsonContent = File.ReadAllText(filePath);

                var tickerInformations = JsonSerializer.Deserialize<TickerUpdate>(jsonContent, _jsonSerializerOptions)
                                            ?? throw new InvalidOperationException("Failed to deserialize JSON content into TickerInformations.");

                return tickerInformations;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Error occurred while parsing JSON file.", ex);
            }
        }

        public List<string> LoadTickers(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at path: {filePath}", filePath);

            try
            {
                var fileContent = File.ReadAllText(filePath);
                var tickers = fileContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(ticker => ticker.Trim())
                                         .ToList();

                return tickers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred while reading or processing the file.", ex);
            }
        }
    }
}
