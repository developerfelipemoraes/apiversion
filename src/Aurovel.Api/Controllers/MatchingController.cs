
using Microsoft.AspNetCore.Mvc;
using Aurovel.Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aurovel.Api.Controllers;

[ApiController]
[Route("api/matching")]
public class MatchingController : ControllerBase
{
    private readonly IContactRepository _contacts;
    private readonly ICompanyRepository _companies;

    public MatchingController(IContactRepository contacts, ICompanyRepository companies)
    {
        _contacts = contacts;
        _companies = companies;
    }

    [HttpGet("company/{companyId}/contacts")]
    public async Task<IActionResult> MatchCompanyToContacts([FromRoute] string companyId)
    {
        // TODO: Confirmar regra original. Versão simples: retorna contatos vinculados a companyId.
        var (items, _) = await _contacts.ListAsync(1, 100, null, null, null, null, null, "createdAt", "desc");
        var linked = items.Where(c => c.LinkedCompanies.Any(l => l.CompanyId == companyId));
        return Ok(new { items = linked, total = linked.Count() });
    }

    [HttpGet("contact/{contactId}/companies")]
    public async Task<IActionResult> MatchContactToCompanies([FromRoute] string contactId)
    {
        var c = await _contacts.GetByIdAsync(contactId);
        if (c is null) return NotFound(new { error = "Contato não encontrado" });
        var ids = c.LinkedCompanies.Select(l => l.CompanyId).ToHashSet();
        var list = new List<object>(); // In real impl, fetch companies by ids (batch)
        foreach (var id in ids)
        {
            var comp = await _companies.GetByIdAsync(id);
            if (comp != null) list.Add(comp);
        }
        return Ok(new { items = list, total = list.Count });
    }

    [HttpGet("best-matches")]
    public async Task<IActionResult> GetBestMatches()
    {
        // TODO: Confirmar regra original. Versão simples: retorna 0 resultado até definirmos a lógica.
        return Ok(new { items = new object[0], total = 0 });
    }

    [HttpPost("links")]
    public async Task<IActionResult> CreateLink([FromBody] dynamic body)
    {
        string contactId = body?.contactId;
        string companyId = body?.companyId;
        string role = body?.role ?? "commercial";
        var updated = await _contacts.LinkToCompanyAsync(contactId, companyId, role);
        if (updated is null) return NotFound(new { error = "Contato não encontrado" });
        return Ok(new { message = "Vínculo criado com sucesso", data = updated });
    }

    [HttpDelete("links/{companyId}/{contactId}")]
    public async Task<IActionResult> RemoveLink([FromRoute] string companyId, [FromRoute] string contactId)
    {
        var updated = await _contacts.UnlinkFromCompanyAsync(contactId, companyId);
        if (updated is null) return NotFound(new { error = "Contato não encontrado" });
        return Ok(new { message = "Vínculo removido com sucesso", data = updated });
    }
}
