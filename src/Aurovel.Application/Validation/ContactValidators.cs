
using FluentValidation;
using MongoDB.Bson;

namespace Aurovel.Application.Validation;

public class ContactCreateValidator : AbstractValidator<BsonDocument>
{
    public ContactCreateValidator()
    {
        RuleFor(d => d.Contains("personalInfo")).Equal(true).WithMessage("personalInfo é obrigatório");
        RuleFor(d => d["personalInfo"]["fullName"].AsString).NotEmpty();
        RuleFor(d => d["personalInfo"]["cpf"].AsString).NotEmpty();
    }
}

public class ContactUpdateValidator : AbstractValidator<BsonDocument>
{
    public ContactUpdateValidator()
    {
        // rules leves; Node usa Joi mais detalhado (TODO confirmar regra original)
        RuleFor(d => d.Contains("personalInfo")).Equal(true);
    }
}
