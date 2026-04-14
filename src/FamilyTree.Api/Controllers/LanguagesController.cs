using FamilyTree.Application.DTOs.Language;
using FamilyTree.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTree.Api.Controllers;

[ApiController]
[Route("api/languages")]
[Authorize]
public class LanguagesController(ILanguageService languageService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByWorld([FromQuery] Guid worldId) =>
        Ok(await languageService.GetByWorldIdAsync(worldId));

    [HttpPost]
    public async Task<IActionResult> GetOrCreate([FromBody] CreateLanguageDto dto)
    {
        var language = await languageService.GetOrCreateAsync(dto.Name, dto.WorldId);
        return Ok(language);
    }
}
