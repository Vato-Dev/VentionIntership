using ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        [HttpPost("add")]
        public ActionResult<AddReply> Add([FromBody] AddRequest request)
        {
            var result = request.NumberA + request.NumberB;

            return Ok(new AddReply 
            { 
                Result = result 
            });
        }
    }
}
