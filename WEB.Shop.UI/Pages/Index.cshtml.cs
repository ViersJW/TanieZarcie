﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using WEB.Shop.Application.Enums;
using WEB.Shop.Application.News;
using WEB.Shop.Application.Products;

namespace WEB.Shop.UI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string ShopDirection { get; set; }

        public Dictionary<string, int> ProductsOnSale { get; set; } = new Dictionary<string, int>();
        public List<GetNews.Response> News { get; set; }
        public List<ShopViewModel> Shops { get; set; } = new List<ShopViewModel>();

        public class ShopViewModel
        {
            public string Name { get; set; }
            public string SmallImagePath { get; set; } = null;
            public string LargeImagePath { get; set; } = null;
        }

        public void OnGet([FromServices] GetProducts getProducts, [FromServices] GetNews getNews)
        {
            News = getNews.Do().ToList();

            foreach (Shops shop in (Shops[])Enum.GetValues(typeof(Shops)))
            {
                var countSale = getProducts.CountProductOnSaleForShop(shop.ToString());

                if (countSale != 0)
                {
                    var shopVm = new ShopViewModel { Name = shop.ToString(), SmallImagePath = $"{shop}-s.jpg", LargeImagePath = $"{shop}.jpg" };
                    ProductsOnSale.Add(shop.ToString(), countSale);
                    Shops.Add(shopVm);
                }
            }
        }

        public IActionResult OnGetShop(string shop) => RedirectToPage("ProductsOverview", new { selectedShop = shop });
    }
}