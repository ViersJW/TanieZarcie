﻿using System;

namespace WEB.SearchEngine.SearchResultsModels
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Producer { get; set; } = "Not Detected";
        public string Seller { get; set; } = "Not Detected";
        public string Category { get; set; } = "Not Detected";
        public string SourceUrl { get; set; }
        public decimal Value { get; set; }

        public bool OnSale { get; set; }
        public decimal SaleValue { get; set; }
        public string SaleDescription { get; set; }
        public DateTime SaleDeadline { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
