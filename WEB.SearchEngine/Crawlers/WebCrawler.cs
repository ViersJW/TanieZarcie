﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WEB.SearchEngine.Enums;
using WEB.SearchEngine.RegexPatterns;
using WEB.SearchEngine.SearchResultsModels;

namespace WEB.SearchEngine.Crawlers
{
    public abstract class WebCrawler
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public virtual string[] BaseUrls { get { return new string[] { "" }; } }
        public Shops Shop { get; set; }

        private HttpClient _httpClient;
        private HttpClient HttpClient { get { return _httpClient ??= new HttpClient(); } }

        public struct LinkStruct
        {
            public string Link { get; set; }
            public string Html { get; set; }
            public LinkStruct(string link, string html)
            {
                Link = link;
                Html = html;
            }
        }

        //public void GetData()
        //{
        //    try
        //    {
        //        var tasks = new List<Task>();      
        //        var webStructs = new List<LinkStruct>();

        //        var linksToVisit = ParseLinks(BaseUrls);
        //        linksToVisit.RemoveAll(item => !item.Contains("http") || !item.Contains("https"));

        //        foreach (var link in linksToVisit)
        //        {
        //            tasks.Add(Task.Run(() => webStructs.Add(new LinkStruct(link, GetSingleHtml(link)))));
        //        }

        //        Task.WaitAll(tasks.ToArray());

        //        webStructs.RemoveAll(link => !LinkCleanUp(link.Link, Shop.ToString()));
        //        webStructs.GroupBy(x => x.Link).Select(x => x.First());

        //        Products = new List<Product>();

        //        foreach (var webStruct in webStructs)
        //        {
        //            Products.AddRange(GetResultsForSingleUrl(webStruct));
        //        }

        //        var distinctProducts = Products
        //            .GroupBy(p => new { p.Name, p.Producer, p.Description, p.Value })
        //            .Select(p => p.First())
        //            .ToList();

        //        Products = distinctProducts;
        //        Products.TrimExcess();
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }
        //}

        public void GetData()
        {
            try
            {
                Products = new List<Product>();

                var linksToVisit = ParseLinks(BaseUrls);
                linksToVisit.RemoveAll(item => !item.Contains("http") || !item.Contains("https"));

                var webStructs = CreateLinkStructs(linksToVisit);

                foreach (var webStruct in webStructs)
                {
                    Products.AddRange(GetResultsForSingleUrl(webStruct));
                }

                var distinctProducts = Products
                    .GroupBy(p => new { p.Name, p.Producer, p.Description, p.Value })
                    .Select(p => p.First())
                    .ToList();

                Products = distinctProducts;
                Products.TrimExcess();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<LinkStruct> CreateLinkStructs(List<string> linksToVisit)
        {
            var webStructs = new List<LinkStruct>();
            var tasks = new List<Task>();

            foreach (var link in linksToVisit)
            {
                tasks.Add(Task.Run(() => webStructs.Add(new LinkStruct(link, GetSingleHtml(link)))));
            }

            Task.WaitAll(tasks.ToArray());

            webStructs.RemoveAll(link => !LinkCleanUp(link.Link, Shop.ToString()));
            webStructs.GroupBy(x => x.Link).Select(x => x.First());

            return webStructs;
        }

        private bool LinkCleanUp(string link, string output)
        {
            if (CrawlerRegex.StandardMatch(link, output, MatchDireciton.InputContainsMatch)) return true;
            else return false;
        }

        private string GetSingleHtml(string link)
        {
            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = HttpClient.GetAsync(link).Result;
            }
            catch (AggregateException)
            {
                return "";
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                try
                {
                    return HttpClient.GetStringAsync(link).Result;
                }
                catch (AggregateException)
                {
                    return "";
                }
            }

            return "";
        }

        //private string GetSingleHtml(string link)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        HttpResponseMessage responseMessage = null;

        //        try
        //        {
        //            responseMessage = client.GetAsync(link).Result;
        //        }
        //        catch (AggregateException)
        //        {
        //            return "";
        //        }

        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            try
        //            {
        //                return client.GetStringAsync(link).Result;
        //            }
        //            catch (AggregateException)
        //            {
        //                return "";
        //            }
        //        }
        //    }
        //    return "";
        //}

        public abstract List<Product> GetResultsForSingleUrl(LinkStruct linkStruct);

        public List<string> ParseLinks(string[] urlsToCrawl)
        {
            byte[] data;
            HashSet<string> list = new HashSet<string>();

            foreach (var urlToCrawl in urlsToCrawl)
            {
                using WebClient webClient = new WebClient();

                data = webClient.DownloadData(urlToCrawl);

                string download = Encoding.ASCII.GetString(data);

                var doc = new HtmlDocument();
                doc.LoadHtml(download);
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");

                if (nodes == null) continue;

                foreach (var n in nodes)
                {
                    string href = n.Attributes["href"].Value;
                    list.Add(GetAbsoluteUrlString(urlToCrawl, href));
                }
            }

            return list.ToList();
        }

        private string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri) uri = new Uri(new Uri(baseUrl), uri);

            return uri.ToString();
        }
    }
}
