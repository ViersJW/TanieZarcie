﻿using System.Linq;
using WEB.Shop.Application.ViewModels;
using WEB.Shop.DataBase;

namespace WEB.Shop.Application.ProductsAdmin
{
    public class GetProduct
    {
        private ApplicationDbContext _context;

        public GetProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProductViewModel Do(int id) =>
            _context.Products.Where(i => i.Id == id).Select(i => new ProductViewModel
            {
                Name = i.Name,
                Description = i.Description,
                Value = i.Value
            })
            .FirstOrDefault();
    }
}
