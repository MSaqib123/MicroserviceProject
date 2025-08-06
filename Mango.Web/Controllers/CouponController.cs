using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICouponService _couponService;

        public CouponController(ILogger<HomeController> logger, ICouponService couponService)
        {
            _couponService = couponService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<CouponDto>? list = new();
            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

    }
}
