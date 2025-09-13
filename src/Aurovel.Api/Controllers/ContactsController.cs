
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aurovel.Infrastructure.Repositories;
using Aurovel.Domain.Documents;
using MongoDB.Bson;
using FluentValidation;
using Aurovel.Application.Validation;
using MongoDB.Bson.Serialization;
using System.Text.Json;
using MongoDB.Bson.IO;

namespace Aurovel.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _repo;
    private readonly IValidator<BsonDocument> _createValidator;
    private readonly IValidator<BsonDocument> _updateValidator;

    public ContactsController(IContactRepository repo, 
        ContactCreateValidator createValidator, 
        ContactUpdateValidator updateValidator)
    {
        _repo = repo;

        _createValidator = createValidator;

        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JsonElement body)
    {
        BsonDocument bson;
        try
        {
            bson = BsonDocument.Parse(body.GetRawText());
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "JSON inválido", message = ex.Message });
        }

        try
        {
            var entity = BsonSerializer.Deserialize<Contact>(bson);

            var created = await _repo.CreateAsync(entity);

            // 🔧 Evita o System.Text.Json serializar Bson*: usamos o driver do Mongo
            var json = created
                .ToBsonDocument()
                .ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            // Evite reutilizar 'val.Errors' aqui; isso é erro de runtime, não de validação
            return BadRequest(new { error = "Falha ao processar o documento", message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int limit = 10,
        [FromQuery] string? status = null, [FromQuery] string? riskLevel = null,
        [FromQuery] int? kycScoreMin = null, [FromQuery] int? kycScoreMax = null,
        [FromQuery] string? search = null, [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
    {
        var (items, total) = await _repo.ListAsync(page,
            limit,
            status, 
            riskLevel, 
            kycScoreMin,
            kycScoreMax, 
            search,
            sortBy, 
            sortOrder);

        return Ok(new { items, total, page, limit });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        // Simplificado: contar por profile.status
        var (items, total) = await _repo.ListAsync(1, 1, null, null, null, null, null, "createdAt", "desc");
        
        var byStatus = items
            .GroupBy(i => i.Profile?.Status ?? "unknown")
            .ToDictionary(g => g.Key, g => g.Count());
        
        return Ok(new { total, byStatus });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var item = await _repo.GetByIdAsync(id);
        
        return item is null ? NotFound(new { error = "Contato não encontrado" }) : Ok(item);
    }

    [HttpGet("cpf/{cpf}")]
    public async Task<IActionResult> GetByCpf([FromRoute] string cpf)
    {
        var item = await _repo.GetByCpfAsync(cpf);
        return item is null ? NotFound(new { error = "Contato não encontrado" }) : Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] JsonElement body )
    {
        BsonDocument bson;
        try
        {
            bson = BsonDocument.Parse(body.GetRawText());

            var val = await _updateValidator.ValidateAsync(bson);

            if (!val.IsValid) return BadRequest(new { error = "Dados inválidos", details = val.Errors.Select(e => e.ErrorMessage) });


            var entity = BsonSerializer.Deserialize<Contact>(bson);

            entity.Id = id;

            var updated = await _repo.UpdateAsync(id, entity);

            return updated is null ? NotFound(new { error = "Contato não encontrado" }) : Ok(updated);

        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "JSON inválido", message = ex.Message });
        }

        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var ok = await _repo.DeleteAsync(id);
        return ok ? NoContent() : NotFound(new { error = "Contato não encontrado" });
    }

    [HttpPost("{contactId}/companies/{companyId}")]
    public async Task<IActionResult> Link([FromRoute] string contactId, [FromRoute] string companyId, [FromBody] dynamic? body)
    {
        string role = "commercial";
      
        try { role = (string)(body?.role ?? "commercial"); } catch {}
        
        var updated = await _repo.LinkToCompanyAsync(contactId, companyId, role);
        
        return updated is null ? NotFound(new { error = "Contato não encontrado" }) : Ok(new { message = "Vínculo criado com sucesso", data = updated });
    }

    [HttpDelete("{contactId}/companies/{companyId}")]
    public async Task<IActionResult> Unlink([FromRoute] string contactId, [FromRoute] string companyId)
    {
        var updated = await _repo.UnlinkFromCompanyAsync(contactId, companyId);

        return updated is null ? NotFound(new { error = "Contato não encontrado" }) : 
            Ok(new { message = "Vínculo removido com sucesso", data = updated });
    }
}
