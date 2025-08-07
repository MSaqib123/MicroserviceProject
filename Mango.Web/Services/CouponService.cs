using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto)
        {
            var Request = new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = couponDto,
                Url = SD.CouponAPIBase + "/api/coupon"
            };
            return await _baseService.SendAsync(Request);
        }

        public async Task<ResponseDto?> DeleteCouponsAsync(int id)
        {
            try
            {
                var Request = new RequestDto()
                {
                    ApiType = SD.ApiType.DELETE,
                    Url = SD.CouponAPIBase + "/api/coupon/" + id
                };
                return await _baseService.SendAsync(Request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            var Request = new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon"
            };
            return await _baseService.SendAsync(Request);
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            var Request = new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode/" + couponCode
            };
            return await _baseService.SendAsync(Request);
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            var Request = new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/" + id
            };
            return await _baseService.SendAsync(Request);
        }

        public async Task<ResponseDto?> UpdateCouponsAsync(CouponDto couponDto)
        {
            var Request = new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = couponDto,
                Url = SD.CouponAPIBase + "/api/coupon"
            };
            return await _baseService.SendAsync(Request);
        }

    }
}
