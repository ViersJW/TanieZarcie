﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB.SearchEngine.Enums;
using WEB.SearchEngine.Extensions;
using WEB.SearchEngine.RegexPatterns;
using WEB.SearchEngine.SearchResultsModels;

namespace WEB.SearchEngine.Crawlers
{
    public class CrawlerMediaMarkt : WebCrawler
    {
        public override string[] BaseUrls { get { return new string[] { "https://mediamarkt.pl/" }; } }

        public CrawlerMediaMarkt()
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

            var divs = htmlDocument.DocumentNode.Descendants("a")
                .AsParallel()
                .Where(node => node.GetAttributeValue("class", "")
                .ContainsAny("m-productsBox_item"))
                .ToList();

            var tasks = new List<Task>();

            foreach (var div in divs)
            {
                ExtractProduct(div, linkStruct);
                var nodeToPass = div;
                //tasks.Add(Task.Run(() => result.Add(ExtractProduct(nodeToPass, linkStruct))));
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

            if (productNode.Descendants("div").Any(n => n.Attributes.Any(x => x.Name == "class" && x.Value == "m-priceBox_old")))
            {

            }

            #endregion

            #region Get Name


            #endregion

            #region Get Description

            #endregion

            #region Get Producer

            #endregion

            #region Get Category

            result.Category = "Markety Elektroniczne";

            #endregion

            #region Get Price and Sale Price



            #endregion

            #region Get Sale Description

            #endregion

            #region Get Sale Deadline

            #endregion

            #region Get Seller, TimeStamp, URL

            //result.Seller = this.GetType().Name.Replace("Crawler", "");
            //result.TimeStamp = DateTime.Now;
            //result.SourceUrl = linkStruct.Link;

            //var productUrl = productNode.GetAttributeValue("href", "");

            //result.SourceUrl = new Uri(new Uri(BaseUrls[0]), productUrl).ToString();

            #endregion

            return result;
        }
    }
}
