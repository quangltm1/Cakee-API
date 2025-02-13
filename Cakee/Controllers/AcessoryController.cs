using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcessoryController : ControllerBase
    {
        private readonly IAcessoryService _acessoryService;
        public AcessoryController(IAcessoryService acessoryService)
        {
            _acessoryService = acessoryService;
        }
    }
}
