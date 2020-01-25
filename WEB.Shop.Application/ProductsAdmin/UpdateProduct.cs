﻿using System.Threading.Tasks;
using WEB.Shop.Application.ViewModels;
using WEB.Shop.DataBase;
using WEB.Shop.Domain.Models;

namespace WEB.Shop.Application.ProductsAdmin
{
    public class UpdateProduct
    {
        private ApplicationDbContext _context;

        public UpdateProduct(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Do(Request request)
        {
            await _context.SaveChangesAsync();
            return new Response();
        }

        public class Request
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Value { get; set; }
            public string ValueDispaly => $"{Value.ToString("N2")} pln";
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Value { get; set; }
            public string ValueDispaly => $"{Value.ToString("N2")} pln";
        }
    }
}
