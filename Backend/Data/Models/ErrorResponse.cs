using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class ErrorResponse
    {

        public ErrorResponse(string code, string description, ErrorType errorType) { }
        public string Code { get; set; }    
        public string Description { get; set; }
        public ErrorType ErrorType { get; set; }    
        
    }


}
