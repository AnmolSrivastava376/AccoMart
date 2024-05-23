using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data.Models
{
    public class ErrorResponse
    {

        public static readonly ErrorResponse None = new (string.Empty, string.Empty, ErrorType.Failure);
        public static readonly ErrorResponse NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

        public ErrorResponse(string code, string description, ErrorType errorType) {
            Code = code;
            Description = description;    
            ErrorType = errorType;
        }
        

        public string Code { get; set; }    
        public string Description { get; set; }
        public ErrorType ErrorType { get; set; }

        public static ErrorResponse NotFound(string code, string description, ErrorType errorType)
        {
            return new(code, description, ErrorType.NotFound);
        }

        public static ErrorResponse Validation(string code, string description, ErrorType errorType)
        { return new(code, description, ErrorType.Validation); }
        

        public static ErrorResponse Failure(string code, string description, ErrorType errorType)
        {
            return new(code, description, ErrorType.NotFound);
        }

        public static ErrorResponse Conflict(string code, string description, ErrorType errorType)
        {
            return new(code, description, ErrorType.NotFound);
        } 
 
    }


}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}
