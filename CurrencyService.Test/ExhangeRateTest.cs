namespace CurrencyService.Test
{
    using Xunit;
    using CurrencyService.Common;
    using CurrencyService.Dto;
    using System.Collections.Generic;

    public class ExhangeRateTest
    {
        [Fact]
        public void When_Response_NotNull()
        {
            CurrencyApi service = new CurrencyApi();

            var sortList = Helper.ConvertSortModel("CurrencyCode asc, BanknoteSelling desc");

            /// Act
            var result = service.GetExchangeRates(ExportTypeEnum.None, m => m.CurrencyCode == "USD");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void When_Response_Null()
        {
            CurrencyApi service = new CurrencyApi();

            /// Act
            var result = service.GetExchangeRates(ExportTypeEnum.Xml, m => m.CurrencyCode == "USD");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void When_Request_With_Order_Success()
        {
            CurrencyApi service = new CurrencyApi();

            var sortList = Helper.ConvertSortModel("CurrencyCode desc, BanknoteSelling asc");

            /// Act
            var result = service.GetExchangeRates(ExportTypeEnum.None, sortList);

            // Assert
            Assert.NotNull(result);
        }
    }
}
