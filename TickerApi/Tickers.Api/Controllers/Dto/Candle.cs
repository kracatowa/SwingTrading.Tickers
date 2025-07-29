﻿namespace Tickers.Api.Controllers.Dto
{
    public class Candle
    {
        public required DateTimeOffset Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public int Volume { get; set; }
        public float Dividends { get; set; }
        public float StockSplits { get; set; }
    }
}
