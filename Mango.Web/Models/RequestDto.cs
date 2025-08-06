using System.ComponentModel.DataAnnotations;
using static Mango.Web.Models.SD;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        public ApiType Result { get; set; } = ApiType.GET;
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
