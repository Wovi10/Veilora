using Veilora.Application.DTOs.DateSuffix;
using Veilora.Application.Exceptions;
using Veilora.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veilora.Api.Controllers;

[ApiController]
[Route("api/date-suffixes")]
[Authorize]
public class DateSuffixesController(IDateSuffixService dateSuffixService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByWorld([FromQuery] Guid worldId) =>
        Ok(await dateSuffixService.GetByWorldIdAsync(worldId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDateSuffixDto dto)
    {
        var created = await dateSuffixService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDateSuffixDto dto)
    {
        var updated = await dateSuffixService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await dateSuffixService.DeleteAsync(id);
        return NoContent();
    }
}
