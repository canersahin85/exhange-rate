﻿namespace CurrencyService.Dto
{
    public class ExchangeRateDto
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string ForexBuying { get; set; }
        public string ForexSelling { get; set; }
        public string BanknoteBuying { get; set; }
        public string BanknoteSelling { get; set; }
        public string Date { get; set; }
        public string CrossRateUSD { get; set; }
    }
}
