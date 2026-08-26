using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController(IMembershipService membershipService) : ControllerBase
    {
        [HttpPost]
        public async Task<MembershipBatchResultDto> ExecuteBatchOperation(MembershipBatchOperationDto dto, CancellationToken cancellationToken)
            => await membershipService.ExecuteAsync(dto, cancellationToken); //Todo: make an IAsyncEnumerable and optimize it

    }
}
