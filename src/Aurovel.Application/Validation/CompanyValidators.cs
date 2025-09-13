
using FluentValidation;
using MongoDB.Bson;

namespace Aurovel.Application.Validation;

public class CompanyCreateValidator : AbstractValidator<BsonDocument>
{
    public CompanyCreateValidator()
    {
        //RuleFor(d => d.Contains("identification")).Equal(true).WithMessage("identification é obrigatório");
        //RuleFor(d => d["identification"]["cnpj"].AsString).NotEmpty();
    }
}

public class CompanyUpdateValidator : AbstractValidator<BsonDocument>
{
    public CompanyUpdateValidator()
    {
        RuleFor(d => d.Contains("identification")).Equal(true);
    }
}
