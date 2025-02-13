using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticalController : ControllerBase
    {
        private readonly IStatisticalService _statisticalService;
        public StatisticalController(IStatisticalService statisticalService)
        {
            _statisticalService = statisticalService;
        }
    }
}
