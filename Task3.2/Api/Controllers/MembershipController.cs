using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/memberships")] 
    [Authorize]
    public class MembershipController(IMembershipService membershipService) : ControllerBase
    {
    
        [HttpPost("batch")]
        public async Task<MembershipBatchResultDto> ExecuteBatchOperation(MembershipBatchOperationDto dto, CancellationToken cancellationToken)
            => await membershipService.ExecuteAsync(dto, cancellationToken); //Todo: make an IAsyncEnumerable and optimize it

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateMembershipPayloadDto dto,
            CancellationToken ct)
        {
            var batch = new MembershipBatchOperationDto
            {
                ToCreate = [dto],
            };
            var result = await membershipService.ExecuteAsync(batch, ct);

            if (result.Failures.Count > 0)
                return BadRequest(result);

            return Ok(new
            {
                userId = dto.UserId,
                organisationId = dto.OrganisationId,
                role = dto.Role,
            });
        }

        [HttpDelete("{userId}/{organisationId}")]
        public async Task<IActionResult> Delete(
            string userId,
            string organisationId,
            CancellationToken ct)
        {
            var batch = new MembershipBatchOperationDto
            {
                ToDelete =
                [
                    new DeleteMembershipPayloadDto
                    {
                        UserId = userId,
                        OrganisationId = organisationId,
                    },
                ],
            };
            var result = await membershipService.ExecuteAsync(batch, ct);

            if (result.Failures.Count > 0)
                return BadRequest(result);

            return NoContent();
        }
    }
}