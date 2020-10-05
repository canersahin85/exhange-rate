namespace CurrencyService
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using System.Linq.Expressions;
    using CurrencyService.Dto;
    using CurrencyService.Common;
    using System.IO;
    using System.Globalization;
    using CsvHelper;

    public class CurrencyApi
    {
        private IQueryable<ExchangeRateDto> GetTodayExchangeRates()
        {
            string today = "https://www.tcmb.gov.tr/kurlar/today.xml";
            //string anyDays = "https://www.tcmb.gov.tr/kurlar/202002/05102020.xml";

            XDocument xmlDoc = XDocument.Load(today);

            if (xmlDoc != null)
            {
                var exchangeRates = (from p in xmlDoc.Element("Tarih_Date").Elements("Currency")
                                     select new ExchangeRateDto
                                     {
                                         Date = (string)xmlDoc.Element("Tarih_Date").Attribute("Tarih"),
                                         CurrencyCode = (string)p.Attribute("Kod"),
                                         CurrencyName = (string)p.Element("Isim"),
                                         ForexBuying = (string)p.Element("ForexBuying"),
                                         ForexSelling = (string)p.Element("ForexSelling"),
                                         BanknoteBuying = (string)p.Element("BanknoteBuying"),
                                         BanknoteSelling = (string)p.Element("BanknoteSelling"),
                                         CrossRateUSD = (string)p.Element("CrossRateUSD"),
                                     }).AsQueryable();
                return exchangeRates;
            }
            return null;
        }

        public List<ExchangeRateDto> GetExchangeRates(ExportTypeEnum exportType, Expression<Func<ExchangeRateDto, bool>> predicate = null, List<GridSortDto> sortList = null)
        {
            var exchangeRates = GetTodayExchangeRates();

            if (exchangeRates.Count() > 0)
            {
                IQueryable<ExchangeRateDto> query = null;
                if (predicate != null)
                {
                    query = exchangeRates.Where(predicate);
                }
                if (sortList != null)
                {
                    query = exchangeRates.AsEnumerable().MultipleSort(sortList).AsQueryable();
                }

                var result = query == null ? exchangeRates.ToList() : query.ToList();

                if (exportType == ExportTypeEnum.None)
                {
                    return result;
                }
                else
                {
                    this.ExportFile(exportType, "exchangeRate", result);
                }
            }

            return null;
        }

        public List<ExchangeRateDto> GetExchangeRates(ExportTypeEnum exportType, Expression<Func<ExchangeRateDto, bool>> predicate)
        {
            var exchangeRates = GetTodayExchangeRates();

            if (exchangeRates.Count() > 0)
            {
                IQueryable<ExchangeRateDto> query = null;
                if (predicate != null)
                {
                    query = exchangeRates.Where(predicate);
                }

                var result = query == null ? exchangeRates.ToList() : query.ToList();
                if (exportType == ExportTypeEnum.None)
                {
                    return result;
                }
                else
                {
                    this.ExportFile(exportType, "exchangeRate", result);
                }
            }

            return null;
        }

        public List<ExchangeRateDto> GetExchangeRates(ExportTypeEnum exportType, List<GridSortDto> sortList)
        {
            var exchangeRates = GetTodayExchangeRates();

            if (exchangeRates.Count() > 0)
            {
                IQueryable<ExchangeRateDto> query = null;
                if (sortList != null)
                {
                    query = exchangeRates.AsEnumerable().MultipleSort(sortList).AsQueryable();
                }

                var result = query == null ? exchangeRates.ToList() : query.ToList();
                if (exportType == ExportTypeEnum.None)
                {
                    return result;
                }
                else
                {
                    this.ExportFile(exportType, "exchangeRate", result);
                }
            }

            return null;
        }

        private void ExportFile(ExportTypeEnum exportType, string filePath, List<ExchangeRateDto> rateList)
        {
            switch (exportType)
            {
                case ExportTypeEnum.Csv:
                    this.ExportCsv(filePath + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv", rateList);
                    break;
                case ExportTypeEnum.Xml:
                    this.ExportXml(filePath + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".xml", rateList);
                    break;
                case ExportTypeEnum.Json:
                    break;
            }
        }

        private void ExportCsv(string filePath, List<ExchangeRateDto> rateList)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rateList);
            }
        }

        private void ExportXml(string filePath, List<ExchangeRateDto> rateList)
        {
            var xdoc = new XDocument
                       (new XElement("document",
                           rateList.Select(item =>
                               new XElement("exchange",
                                   new XElement("date", item.Date),
                                   new XElement("currency_code", item.CurrencyCode),
                                   new XElement("currency_name", item.CurrencyName),
                                   new XElement("forex_buying", item.ForexBuying),
                                   new XElement("forex_selling", item.ForexSelling),
                                   new XElement("banknote_buying", item.BanknoteBuying),
                                   new XElement("banknote_selling", item.BanknoteSelling),
                                   new XElement("cross_rate_usd", item.CrossRateUSD)
                               )
                           )
                       )
            );

            xdoc.Save(filePath);

        }
    }
}
