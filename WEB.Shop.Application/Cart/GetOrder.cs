﻿using System.Collections.Generic;
using System.Linq;
using WEB.Shop.Domain.Infrastructure;

namespace WEB.Shop.Application.Cart
{
    [TransientService]
    public class GetOrder
    {
        private ISessionManager _sessionManager;

        public GetOrder(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public class Response
        {
            public IEnumerable<Product> Products { get; set; }
            public CustomerInformation CustomerInformation { get; set; }
            public int GetTotalCharge() => Products.Sum(product => product.Value * product.Quantity);
        }

        public class Product
        {
            public int StockId { get; set; }
            public int Quantity { get; set; }
            public int ProductId { get; set; }
            public int Value { get; set; }
        }

        public class CustomerInformation
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
        }

        public Response Do()
        {
            var listOfProducts = _sessionManager
                .GetCart(cartProduct => new Product
                {
                    ProductId = cartProduct.ProductId,
                    StockId = cartProduct.StockId,
                    Value = (int)(cartProduct.Value * 100),
                    Quantity = cartProduct.Quantity
                });
  

            //var customerInfoString = _session.GetString("Customer-info");
            //var customerInformation = JsonConvert.DeserializeObject<CustomerInformation>(customerInfoString);

            return new Response
            {
                Products = listOfProducts,
                //CustomerInformation = new CustomerInformation
                //{
                //    FirstName = customerInformation.FirstName,
                //    LastName = customerInformation.LastName,
                //    Email = customerInformation.Email,
                //    PhoneNumber = customerInformation.PhoneNumber,
                //    Address1 = customerInformation.Address1,
                //    Address2 = customerInformation.Address2,
                //    City = customerInformation.City,
                //    PostCode = customerInformation.PostCode
                //}
            };
        }
    }
}
