using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class RequestBrandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ResponseBrandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
    }
   


}
