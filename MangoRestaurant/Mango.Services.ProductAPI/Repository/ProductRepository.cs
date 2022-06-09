using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;

        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {

            Product model = _mapper.Map<ProductDto, Product>(productDto);
            
            if(model.ProductId > 0)
            {
                Product product = await _db.Products.Where(w => w.ProductId == model.ProductId).FirstOrDefaultAsync();
                product.Name = model.Name != null ? model.Name : product.Name;
                product.Description = model.Description != null ? model.Description : product.Description;
                product.Price = model.Price != null ? model.Price : product.Price;
                product.CategoryName = model.CategoryName != null ? model.CategoryName : product.CategoryName;
                product.ImageUrl = model.ImageUrl != null ? model.ImageUrl : product.ImageUrl;

                _db.Update(product);
            } else
            {
                _db.Add(model);
            }

            await _db.SaveChangesAsync();
            return _mapper.Map<Product, ProductDto>(model);

        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                Product product = await _db.Products.FirstOrDefaultAsync(f => f.ProductId == productId);
                if (product == null)
                {
                    return false;
                }
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                return true;
            } catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public  async Task<ProductDto> GetProductById(int productId)
        {
            Product product = await _db.Products.Where(w => w.ProductId == productId).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            List<Product> productList = await _db.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(productList);

        }
    }
}
