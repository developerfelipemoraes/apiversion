using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Aurovel.Domain.Documents
{
    [BsonIgnoreExtraElements]
    public class Company
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonElement("basicInfo")]
        public BasicInfo BasicInfo { get; set; } = new();

        [BsonElement("corporateStructure")]
        public CorporateStructure CorporateStructure { get; set; } = new();

        [BsonElement("addresses")]
        public CompanyAddresses Addresses { get; set; } = new();

        [BsonElement("contact")]
        public CompanyContact Contact { get; set; } = new();

        [BsonElement("keyContacts")]
        public List<KeyContact> KeyContacts { get; set; } = new();

        [BsonElement("financial")]
        public FinancialInfo Financial { get; set; } = new();

        [BsonElement("banking")]
        public BankingInfo Banking { get; set; } = new();

        [BsonElement("fleet")]
        public FleetInfo Fleet { get; set; } = new();

        [BsonElement("licenses")]
        public LicensesInfo Licenses { get; set; } = new();

        [BsonElement("compliance")]
        public ComplianceInfo Compliance { get; set; } = new();

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? UpdatedAt { get; set; }
    }

    // -------- basicInfo --------
    [BsonIgnoreExtraElements]
    public class BasicInfo
    {
        [BsonElement("legalName")] public string? LegalName { get; set; }
        [BsonElement("tradeName")] public string? TradeName { get; set; }
        [BsonElement("cnpj")] public string? Cnpj { get; set; }
        [BsonElement("stateRegistration")] public string? StateRegistration { get; set; }
        [BsonElement("municipalRegistration")] public string? MunicipalRegistration { get; set; }

        [BsonElement("foundingDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? FoundingDate { get; set; }

        [BsonElement("legalNature")] public string? LegalNature { get; set; }
        [BsonElement("companySize")] public string? CompanySize { get; set; }
        [BsonElement("mainActivity")] public string? MainActivity { get; set; }
        [BsonElement("secondaryActivities")] public List<string>? SecondaryActivities { get; set; }
    }

    // -------- corporateStructure --------
    [BsonIgnoreExtraElements]
    public class CorporateStructure
    {
        [BsonElement("authorizedCapital")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? AuthorizedCapital { get; set; }

        [BsonElement("paidCapital")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? PaidCapital { get; set; }

        [BsonElement("numberOfPartners")] public int? NumberOfPartners { get; set; }

        [BsonElement("partners")] public List<Partner>? Partners { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Partner
    {
        [BsonElement("name")] public string? Name { get; set; }
        [BsonElement("cpf")] public string? Cpf { get; set; }
        [BsonElement("participation")] public int? Participation { get; set; } // %
        [BsonElement("role")] public string? Role { get; set; }
    }

    // -------- addresses --------
    [BsonIgnoreExtraElements]
    public class CompanyAddresses
    {
        [BsonElement("headquarters")] public Address? Headquarters { get; set; }
        [BsonElement("correspondence")] public Address? Correspondence { get; set; }
    }

    // -------- contact --------
    [BsonIgnoreExtraElements]
    public class CompanyContact
    {
        [BsonElement("email")] public string? Email { get; set; }
        [BsonElement("phone")] public string? Phone { get; set; }
        [BsonElement("mobile")] public string? Mobile { get; set; }
        [BsonElement("website")] public string? Website { get; set; }
        [BsonElement("linkedin")] public string? Linkedin { get; set; }
    }

    // -------- keyContacts --------
    [BsonIgnoreExtraElements]
    public class KeyContact
    {
        [BsonElement("name")] public string? Name { get; set; }
        [BsonElement("position")] public string? Position { get; set; }
        [BsonElement("email")] public string? Email { get; set; }
        [BsonElement("phone")] public string? Phone { get; set; }
    }

    // -------- financial --------
    [BsonIgnoreExtraElements]
    public class FinancialInfo
    {
        [BsonElement("annualRevenue")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? AnnualRevenue { get; set; }

        [BsonElement("numberOfEmployees")] public int? NumberOfEmployees { get; set; }
        [BsonElement("creditRating")] public string? CreditRating { get; set; }
        [BsonElement("paymentTerms")] public string? PaymentTerms { get; set; }

        [BsonElement("averageTicket")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? AverageTicket { get; set; }

        [BsonElement("mainClients")] public List<string>? MainClients { get; set; }
    }

    // -------- banking --------
    [BsonIgnoreExtraElements]
    public class BankingInfo
    {
        [BsonElement("primaryBank")] public string? PrimaryBank { get; set; }
        [BsonElement("accountNumber")] public string? AccountNumber { get; set; }
        [BsonElement("agency")] public string? Agency { get; set; }
        [BsonElement("pixKey")] public string? PixKey { get; set; }
    }

    // -------- fleet --------
    [BsonIgnoreExtraElements]
    public class FleetInfo
    {
        [BsonElement("hasFleet")] public bool? HasFleet { get; set; }
        [BsonElement("vehicleCount")] public int? VehicleCount { get; set; }
        [BsonElement("vehicles")] public List<Vehicle>? Vehicles { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Vehicle
    {
        [BsonElement("type")] public string? Type { get; set; }
        [BsonElement("brand")] public string? Brand { get; set; }
        [BsonElement("model")] public string? Model { get; set; }
        [BsonElement("year")] public int? Year { get; set; }
        [BsonElement("plate")] public string? Plate { get; set; }
    }

    // -------- licenses --------
    [BsonIgnoreExtraElements]
    public class LicensesInfo
    {
        [BsonElement("hasLicenses")] public bool? HasLicenses { get; set; }
        [BsonElement("licenses")] public List<LicenseItem>? Licenses { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class LicenseItem
    {
        [BsonElement("type")] public string? Type { get; set; }
        [BsonElement("number")] public string? Number { get; set; }

        [BsonElement("issueDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? IssueDate { get; set; }

        [BsonElement("expiryDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ExpiryDate { get; set; }

        [BsonElement("issuingBody")] public string? IssuingBody { get; set; }
    }

    // -------- compliance --------
    [BsonIgnoreExtraElements]
    public class ComplianceInfo
    {
        [BsonElement("kycScore")] public int? KycScore { get; set; }
        [BsonElement("riskClassification")] public string? RiskClassification { get; set; }

        [BsonElement("lastReview")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LastReview { get; set; }

        [BsonElement("nextReview")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? NextReview { get; set; }

        [BsonElement("complianceOfficer")] public string? ComplianceOfficer { get; set; }

        [BsonElement("documents")] public List<ComplianceDocument>? Documents { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ComplianceDocument
    {
        [BsonElement("type")] public string? Type { get; set; }
        [BsonElement("status")] public string? Status { get; set; }

        [BsonElement("lastUpdate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LastUpdate { get; set; }
    }
}