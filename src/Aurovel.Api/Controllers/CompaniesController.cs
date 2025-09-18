
using Microsoft.AspNetCore.Mvc;
using Aurovel.Infrastructure.Repositories;
using Aurovel.Domain.Documents;
using MongoDB.Bson;
using FluentValidation;
using Aurovel.Application.Validation;
using System.Text.Json;

namespace Aurovel.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyRepository _repo;
    private readonly IValidator<BsonDocument> _createValidator;
    private readonly IValidator<BsonDocument> _updateValidator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="createValidator"></param>
    /// <param name="updateValidator"></param>
    public CompaniesController(ICompanyRepository repo, CompanyCreateValidator createValidator, CompanyUpdateValidator updateValidator)
    {
        _repo = repo;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JsonElement body)
    {

        BsonDocument bson;
        try
        {
            bson = BsonDocument.Parse(body.GetRawText());

            var val = await _createValidator.ValidateAsync(bson);

            if (!val.IsValid)
                return BadRequest(new
                {
                    error = "Dados inválidos",
                    details = val.Errors.Select(e => e.ErrorMessage)
                }
            );

            var entity = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Company>(bson);

            var created = await _repo.CreateAsync(entity);

            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "JSON inválido", message = ex.Message });
        }

     
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <param name="status"></param>
    /// <param name="kycScoreMin"></param>
    /// <param name="kycScoreMax"></param>
    /// <param name="search"></param>
    /// <param name="sortBy"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int limit = 10,
        [FromQuery] string? status = null, [FromQuery] int? kycScoreMin = null, 
        [FromQuery] int? kycScoreMax = null,
        [FromQuery] string? search = null, 
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
    {
        var (items, total) = await _repo.ListAsync(page, 
            limit, 
            status, 
            kycScoreMin, 
            kycScoreMax, 
            search, 
            sortBy, 
            sortOrder);
        return Ok(new { items, total, page, limit });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        var (items, total) = await _repo.ListAsync(1, 1, null, null, null, null, "createdAt", "desc");
        
        var byStatus = items
            .GroupBy(i => i.Compliance?.RiskClassification ?? "unknown")
            .ToDictionary(g => g.Key, g => g.Count());
        
        return Ok(new { total, byStatus });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var item = await _repo.GetByIdAsync(id);

        return item is null ? NotFound(new { error = "Empresa não encontrada" }) : Ok(new {  item });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cnpj"></param>
    /// <returns></returns>
    [HttpGet("cnpj/{cnpj}")]
    public async Task<IActionResult> GetByCnpj([FromRoute] string cnpj)
    {
        var item = await _repo.GetByCnpjAsync(cnpj);

        return item is null ? NotFound(new { error = "Empresa não encontrada" }) : Ok(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] BsonDocument body)
    {
        var val = await _updateValidator.ValidateAsync(body);
        
        if (!val.IsValid) 
            return BadRequest(new { error = "Dados inválidos", details = val.Errors.Select(e => e.ErrorMessage) });
        
        var entity = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Company>(body);
        
        entity.Id = id;
        
        var updated = await _repo.UpdateAsync(id, entity);
        
        return updated is null ? NotFound(new { error = "Empresa não encontrada" }) : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var ok = await _repo.DeleteAsync(id);

        return ok ? NoContent() : NotFound(new { error = "Empresa não encontrada" });
    }
}
