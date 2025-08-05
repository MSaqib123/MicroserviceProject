using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace Mango.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        
        public CouponAPIController(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public object Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                return objList;
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }


        [HttpGet()]
        [Route("{id:int}")]
        public object Get(int id)
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList().Where(x=>x.CouponId == id);
                return objList;
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

    }
}
