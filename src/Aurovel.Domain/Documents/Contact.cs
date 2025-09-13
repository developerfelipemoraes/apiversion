using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aurovel.Domain.Documents;

public class Contact
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("personalInfo")]
    public PersonalInfo? PersonalInfo { get; set; }

    [BsonElement("contact")]
    [BsonIgnoreIfNull]
    public ContactData? ContactData { get; set; }

    [BsonElement("addresses")]
    [BsonIgnoreIfNull]
    public Addresses? Addresses { get; set; }

    [BsonElement("professional")]
    [BsonIgnoreIfNull]
    public Professional? Professional { get; set; }

    [BsonElement("financial")]
    [BsonIgnoreIfNull]
    public Financial? Financial { get; set; }

    [BsonElement("banking")]
    [BsonIgnoreIfNull]
    public Banking? Banking { get; set; }

    [BsonElement("documents")]
    [BsonIgnoreIfNull]
    public DocumentsRoot? Documents { get; set; }

    [BsonElement("compliance")]
    [BsonIgnoreIfNull]
    public Compliance? Compliance { get; set; }

    [BsonElement("profile")]
    [BsonIgnoreIfNull]
    public Profile? Profile { get; set; }

    [BsonElement("linkedCompanies")]
    [BsonIgnoreIfNull]
    public List<LinkedCompanyRef> LinkedCompanies { get; set; } = new();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdatedAt { get; set; }
}

public class PersonalInfo
{
    [BsonElement("fullName")] public string? FullName { get; set; }
    [BsonElement("cpf")] public string? Cpf { get; set; }
    [BsonElement("rg")] public string? Rg { get; set; }
    [BsonElement("birthDate")] public DateTime? BirthDate { get; set; }
    [BsonElement("nationality")] public string? Nationality { get; set; }
    [BsonElement("maritalStatus")] public string? MaritalStatus { get; set; }
    [BsonElement("gender")] public string? Gender { get; set; }
    [BsonElement("motherName")] public string? MotherName { get; set; }
    [BsonElement("fatherName")] public string? FatherName { get; set; }
}

public class ContactData
{
    [BsonElement("email")] public string? Email { get; set; }
    [BsonElement("personalEmail")] public string? PersonalEmail { get; set; }
    [BsonElement("phone")] public string? Phone { get; set; }
    [BsonElement("landline")] public string? Landline { get; set; }
    [BsonElement("emergencyContact")] public EmergencyContact? EmergencyContact { get; set; }
}

public class EmergencyContact
{
    [BsonElement("name")] public string? Name { get; set; }
    [BsonElement("relationship")] public string? Relationship { get; set; }
    [BsonElement("phone")] public string? Phone { get; set; }
}

public class Addresses
{
    [BsonElement("residential")] public Address? Residential { get; set; }
    [BsonElement("professional")] public Address? Professional { get; set; }
}

public class Address
{
    [BsonElement("street")] public string? Street { get; set; }
    [BsonElement("number")] public string? Number { get; set; }
    [BsonElement("complement")] public string? Complement { get; set; }
    [BsonElement("neighborhood")] public string? Neighborhood { get; set; }
    [BsonElement("city")] public string? City { get; set; }
    [BsonElement("state")] public string? State { get; set; }
    [BsonElement("zipCode")] public string? ZipCode { get; set; }
    [BsonElement("country")] public string? Country { get; set; }
    [BsonElement("residenceType")] public string? ResidenceType { get; set; }
    [BsonElement("residenceTime")] public string? ResidenceTime { get; set; }
}

public class Professional
{
    [BsonElement("company")] public string? Company { get; set; }
    [BsonElement("position")] public string? Position { get; set; }
    [BsonElement("department")] public string? Department { get; set; }
    [BsonElement("admissionDate")] public DateTime? AdmissionDate { get; set; }
    [BsonElement("salary")] public decimal? Salary { get; set; }
    [BsonElement("workRegime")] public string? WorkRegime { get; set; }
    [BsonElement("previousExperience")] public List<Experience>? PreviousExperience { get; set; }
}

public class Experience
{
    [BsonElement("company")] public string? Company { get; set; }
    [BsonElement("position")] public string? Position { get; set; }
    [BsonElement("period")] public string? Period { get; set; }
    [BsonElement("reason")] public string? Reason { get; set; }
}

public class Financial
{
    [BsonElement("monthlyIncome")] public decimal? MonthlyIncome { get; set; }
    [BsonElement("otherIncome")] public decimal? OtherIncome { get; set; }
    [BsonElement("totalIncome")] public decimal? TotalIncome { get; set; }
    [BsonElement("monthlyExpenses")] public decimal? MonthlyExpenses { get; set; }
    [BsonElement("netWorth")] public decimal? NetWorth { get; set; }
    [BsonElement("creditScore")] public int? CreditScore { get; set; }
    [BsonElement("hasDebts")] public bool HasDebts { get; set; }
    [BsonElement("debts")] public List<Debt>? Debts { get; set; }
    [BsonElement("assets")] public List<Asset>? Assets { get; set; }
}

public class Debt
{
    [BsonElement("type")] public string? Type { get; set; }
    [BsonElement("amount")] public decimal? Amount { get; set; }
}

public class Asset
{
    [BsonElement("type")] public string? Type { get; set; }
    [BsonElement("description")] public string? Description { get; set; }
    [BsonElement("value")] public decimal? Value { get; set; }
}

public class Banking
{
    [BsonElement("primaryBank")] public string? PrimaryBank { get; set; }
    [BsonElement("accountType")] public string? AccountType { get; set; }
    [BsonElement("accountNumber")] public string? AccountNumber { get; set; }
    [BsonElement("agency")] public string? Agency { get; set; }
    [BsonElement("bankingTime")] public string? BankingTime { get; set; }
    [BsonElement("creditLimit")] public decimal? CreditLimit { get; set; }
    [BsonElement("pixKey")] public string? PixKey { get; set; }
}

public class DocumentsRoot
{
    [BsonElement("hasValidDocuments")] public bool HasValidDocuments { get; set; }
    [BsonElement("documents")] public List<Document>? DocumentList { get; set; }
}

public class Document
{
    [BsonElement("type")] public string? Type { get; set; }
    [BsonElement("number")] public string? Number { get; set; }
    [BsonElement("status")] public string? Status { get; set; }
    [BsonElement("lastVerification")] public DateTime? LastVerification { get; set; }
    [BsonElement("issuingBody")] public string? IssuingBody { get; set; }
    [BsonElement("issueDate")] public DateTime? IssueDate { get; set; }
    [BsonElement("category")] public string? Category { get; set; }
    [BsonElement("expiryDate")] public DateTime? ExpiryDate { get; set; }
}

public class Compliance
{
    [BsonElement("kycScore")] public int? KycScore { get; set; }
    [BsonElement("riskClassification")] public string? RiskClassification { get; set; }
    [BsonElement("lastReview")] public DateTime? LastReview { get; set; }
    [BsonElement("nextReview")] public DateTime? NextReview { get; set; }
    [BsonElement("pep")] public bool Pep { get; set; }
    [BsonElement("sanctionsList")] public bool SanctionsList { get; set; }
    [BsonElement("complianceNotes")] public string? ComplianceNotes { get; set; }
    [BsonElement("approvedBy")] public string? ApprovedBy { get; set; }
    [BsonElement("approvalDate")] public DateTime? ApprovalDate { get; set; }
}

public class Profile
{
    [BsonElement("status")] public string? Status { get; set; } // prospect, client, inactive
    [BsonElement("riskLevel")] public string? RiskLevel { get; set; }
    [BsonElement("kycScore")] public int? KycScore { get; set; }
    [BsonElement("tags")] public List<string>? Tags { get; set; }
}

public class LinkedCompanyRef
{
    [BsonElement("companyId"), BsonRepresentation(BsonType.ObjectId)]
    public string CompanyId { get; set; } = default!;

    [BsonElement("role")]
    public string Role { get; set; } = "commercial";

    [BsonElement("addedAt")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}