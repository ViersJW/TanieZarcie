﻿using System.Linq;
using System.Threading.Tasks;
using WEB.Shop.Application.ViewModels;
using WEB.Shop.DataBase;
using WEB.Shop.Domain.Models;

namespace WEB.Shop.Application.ProductsAdmin
{
    public class DeleteProduct
    {
        private ApplicationDbContext _context;

        public DeleteProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Do(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
