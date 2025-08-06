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
        private ResponseDto _response;
        private readonly IMapper _mapper;
        
        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }


        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = objList;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
                return _response;
            }
        }


        [HttpGet()]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon objList = _db.Coupons.First(x=>x.CouponId == id);
                var dto = _mapper.Map<CouponDto>(objList);
                _response.Result = dto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }


        [HttpGet()]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon objList = _db.Coupons.First(x => x.CouponCode == code);
                var dto = _mapper.Map<CouponDto>(objList);
                _response.Result = dto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }



        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto dto)
        {
            try
            {
                Coupon dbobj = _mapper.Map<Coupon>(dto);
                _db.Coupons.Add(dbobj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(dbobj);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }




        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto dto)
        {
            try
            {
                Coupon dbobj = _mapper.Map<Coupon>(dto);
                _db.Coupons.Update(dbobj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(dbobj);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }



        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon objList = _db.Coupons.First(x => x.CouponId == id);
                _db.Coupons.Remove(objList);
                _db.SaveChanges();

                _response.Result = objList;
                _response.IsSuccess = true;
                _response.Message = "Record deleted successfully";
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }



    }
}
