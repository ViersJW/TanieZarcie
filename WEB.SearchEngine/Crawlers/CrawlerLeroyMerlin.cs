﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB.SearchEngine.Enums;
using WEB.SearchEngine.RegexPatterns;
using WEB.SearchEngine.SearchResultsModels;

namespace WEB.SearchEngine.Crawlers
{
    public class CrawlerLeroyMerlin : WebCrawler
    {
        public override string[] BaseUrls { get { return new string[] { "https://www.leroymerlin.pl/" }; } }

        public CrawlerLeroyMerlin()
        {
            if (Enum.TryParse(this.GetType().Name.Replace("Crawler", ""), true, out Shops shop))
            {
                Shop = shop;
            }
            else
            {
                Shop = Shops.None;
            }
        }

        public override List<Product> GetResultsForSingleUrl(LinkStruct linkStruct)
        {
            var result = new List<Product>();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(linkStruct.Html);

            var divs = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'product')]")?.ToList();

            if (divs == null) return result;

            var tasks = new List<Task>();

            foreach (var div in divs)
            {
                //ExtractProduct(div, linkStruct);
                var nodeToPass = div;
                tasks.Add(Task.Run(() => result.Add(ExtractProduct(nodeToPass, linkStruct))));
            }

            Task.WaitAll(tasks.ToArray());

            result.RemoveAll(x => string.IsNullOrEmpty(x.Name));
            result.TrimExcess();
            return result;
        }

        private Product ExtractProduct(HtmlNode productNode, LinkStruct linkStruct)
        {
            var result = new Product();

            #region Check if product node exists

            var promoPriceNode = productNode.Descendants("span")
                .Where(n => n.Attributes.Any(x => x.Name == "class" && CrawlerRegex.StandardMatch(x.Value, "product-price promotional", MatchDireciton.InputContainsMatch)))
                .FirstOrDefault();

            var regularPriceNode = productNode.Descendants("span")
                .Where(n => n.Attributes
                    .Any(x => x.Name == "class" && CrawlerRegex.StandardMatch(x.Value, "product-price", MatchDireciton.InputContainsMatch)
                        && !CrawlerRegex.StandardMatch(x.Value, "promotional", MatchDireciton.InputContainsMatch)))
                .FirstOrDefault();

            if (promoPriceNode == null || productNode.Descendants("h3").FirstOrDefault() == null)
            {
                return result;
            }

            #endregion

            #region Get Name

            result.Name = productNode.Descendants("h3")
                .FirstOrDefault()?
                .InnerText
                .RemoveMetaCharacters();

            #endregion

            #region Get Description

            #endregion

            #region Get Producer

            #endregion

            #region Get Category

            result.Category = "Markety Budowlane";

            #endregion

            #region Get Price and Sale Price, set OnSale Flag

            if (regularPriceNode == null)
            {
                var promoPrice = promoPriceNode.GetAttributeValue("data-price", "")?.RemoveNonNumeric();

                if (decimal.TryParse(promoPrice, out decimal promoPriceDecimal)) result.Value = promoPriceDecimal / 100;
                result.OnSale = true;
            }
            else
            {
                var promoPrice = promoPriceNode.GetAttributeValue("data-price", "")?.RemoveNonNumeric();
                var regularPrice = regularPriceNode.GetAttributeValue("data-price", "")?.RemoveNonNumeric();

                if (decimal.TryParse(promoPrice, out decimal promoPriceDecimal)) result.Value = promoPriceDecimal / 100;
                if (decimal.TryParse(regularPrice, out decimal regularPriceDecimal)) result.SaleValue = regularPriceDecimal / 100;
                result.OnSale = true;
            }

            #endregion

            #region Get Sale Description

            #endregion

            #region Get Sale Deadline

            #endregion

            #region Get Seller, TimeStamp, URL

            result.Seller = this.GetType().Name.Replace("Crawler", "");
            result.TimeStamp = DateTime.Now;

            var productUrl = productNode
                .Descendants("a")
                .FirstOrDefault(x => x.Attributes.Any(y => y.Name == "class" && y.Value.NormalizeWithStandardRegex() == "Url".NormalizeWithStandardRegex()))?
                .GetAttributeValue("href", "");

            result.SourceUrl = new Uri(new Uri(BaseUrls[0]), productUrl).ToString();

            #endregion

            return result;
        }
    }
}
