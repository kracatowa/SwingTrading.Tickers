using System.Text.Json;
using Tickers.Api.Controllers.Dto;
using Tickers.Api.Services;
using Tickers.Domain.Intervals;

namespace Stocks.Api.UnitTests.Services
{
    public class CandleFileServiceTests
    {
        [Fact]
        public void LoadTickerInformations_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange  
            var service = new CandleFileService();
            var invalidFilePath = "nonexistent.json";

            // Act & Assert  
            Assert.Throws<FileNotFoundException>(() => service.LoadTickerInformations(invalidFilePath));
        }

        [Fact]
        public void LoadTickerInformations_ShouldThrowInvalidOperationException_WhenJsonIsInvalid()
        {
            // Arrange  
            var service = new CandleFileService();
            var invalidJsonFilePath = "invalid.json";
            File.WriteAllText(invalidJsonFilePath, "Invalid JSON Content");

            try
            {
                // Act & Assert  
                Assert.Throws<InvalidOperationException>(() => service.LoadTickerInformations(invalidJsonFilePath));
            }
            finally
            {
                File.Delete(invalidJsonFilePath);
            }
        }

        [Fact]
        public void LoadTickerInformations_ShouldReturnTickerInformations_WhenJsonIsValid()
        {
            // Arrange  
            var service = new CandleFileService();
            var validJsonFilePath = "valid.json";
            var validTickerInformations = new List<TickerInformations>
          {
              new() {
                  Ticker = "AAPL",
                  Interval = IntervalTypes.OneDay,
                  Candles =
                  [
                      new Candle { Open = 150.0f, High = 155.0f, Low = 149.0f, Close = 154.0f, Volume = 1000, Date = DateTime.UtcNow },
                      new Candle { Open = 154.0f, High = 156.0f, Low = 153.0f, Close = 155.0f, Volume = 1200, Date = DateTime.UtcNow.AddMinutes(-1) }
                  ]
              }
          };
            var validJsonContent = JsonSerializer.Serialize(validTickerInformations);
            File.WriteAllText(validJsonFilePath, validJsonContent);

            try
            {
                // Act  
                var result = service.LoadTickerInformations(validJsonFilePath);

                // Assert  
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Equal("AAPL", result[0].Ticker);
                Assert.Equal(IntervalTypes.OneDay, result[0].Interval);
                Assert.NotEmpty(result[0].Candles);
                Assert.Equal(2, result[0].Candles.Length);
                Assert.Equal(150.0, result[0].Candles[0].Open);
                Assert.Equal(155.0, result[0].Candles[0].High);
                Assert.Equal(149.0, result[0].Candles[0].Low);
                Assert.Equal(154.0, result[0].Candles[0].Close);
                Assert.Equal(1000, result[0].Candles[0].Volume);
            }
            finally
            {
                File.Delete(validJsonFilePath);
            }
        }
    }
}
