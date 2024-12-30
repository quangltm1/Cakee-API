using Cakee_Api.Datas;
using Cakee_Api.Models;
using Cakee_Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cakee_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CakeController : ControllerBase
    {
        private readonly ICakeService _cakeService;
        public static List<Cake> cakes = new List<Cake>();

        public static List<Category> categories = new List<Category>();

        public CakeController(ICakeService cakeService)
        {
            _cakeService = cakeService;
        }

        [HttpGet("cakes/details")]
        public async Task<IActionResult> GetCakesWithDetails()
        {
            var result = await _cakeService.GetCakesWithDetailsAsync();
            return Ok(result);
        }



        [HttpGet]
        public async Task<ActionResult<List<Cake>>> GetCake()
        {
            var cakes = await _cakeService.GetAllCakes();
            var response = new List<object>(); // This will hold the formatted response
            //if cake not null then show, if null then show message not found
            if (cakes == null)
            {
                return NotFound("Cake not found");
            }
            foreach (var cake in cakes)
            {
                response.Add(new
                {
                    Id = cake.Id.ToString(),
                    CakeName = cake.CakeName,
                    CakeSizeId = cake.CakeSizeId,
                    CakeDescription = cake.CakeDescription,
                    CakeImage = cake.CakeImage,
                    CategoryId = cake.CakeCategoryId,
                    CakeRating = cake.CakeRating,
                    CakeStock = cake.CakeStock,
                    BillDetails = cake.BillDetails.Select(bill => new
                    {
                        bill.BillId,
                        bill.BillCakePrice,
                        bill.CakeId,
                        bill.BillCakeQuantity,
                    })
                });
            }
            return Ok(response);
        }
        [HttpPost]
        public ActionResult Post([FromBody] CakeVM cakeVM)
        {
            var cake = new Cake
            {
                Id = ObjectId.GenerateNewId(),
                CakeName = cakeVM.CakeName,
                CakeDescription = cakeVM.CakeDescription,
                CakeImage = cakeVM.CakeImage,
                CakeRating = cakeVM.CakeRating,
                CakeCategoryId = ObjectId.Parse(""),
                CakeSizeId = ObjectId.Parse(""),
                CakeSizePrice = ObjectId.Parse(""),
                BillDetails = new List<BillDetails>() // Initialize the required BillDetails property
            };
            cakes.Add(cake);
            return Ok();
        }
    }
}
