using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ProductReqDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int BrandId { get; set; }

        public List<IFormFile>? Files { get; set; }
    }
}
