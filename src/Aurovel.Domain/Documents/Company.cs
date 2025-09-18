using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aurovel.Domain.Documents
{
    [BsonIgnoreExtraElements]
    public class CompanyDataDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        // ===== Step 0: Identification (para o índice unique identification.cnpj) =====
        [BsonElement("identification")]
        public Identification Identification { get; set; } = new();

        // ===== Step 1: Identification (espelhando o payload TS na raiz) =====
        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;

        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;

        [BsonElement("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [BsonElement("nire")]
        public string? Nire { get; set; }

        [BsonElement("stateRegistration")]
        public string? StateRegistration { get; set; }

        [BsonElement("municipalRegistration")]
        public string? MunicipalRegistration { get; set; }

        [BsonElement("primaryCnae")]
        public string? PrimaryCnae { get; set; }

        [BsonElement("secondaryCnaes")]
        public List<string> SecondaryCnaes { get; set; } = new();

        [BsonElement("legalNature")]
        public string? LegalNature { get; set; }

        // 'MEI' | 'ME' | 'EPP' | 'Others'
        [BsonElement("companySize")]
        public string CompanySize { get; set; } = "Others";

        // 'Simples' | 'Lucro Presumido' | 'Lucro Real'
        [BsonElement("taxRegime")]
        public string TaxRegime { get; set; } = "Simples";

        // manter string para casar com o TypeScript
        [BsonElement("incorporationDate")]
        public string IncorporationDate { get; set; } = string.Empty;

        // ===== Step 2: Addresses =====
        [BsonElement("commercialAddress")]
        public Address CommercialAddress { get; set; } = new();

        [BsonElement("billingAddress")]
        public Address? BillingAddress { get; set; }

        [BsonElement("deliveryAddress")]
        public Address? DeliveryAddress { get; set; }

        // ===== Step 3: Contacts =====
        [BsonElement("primaryEmail")]
        public string PrimaryEmail { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("whatsapp")]
        public string? Whatsapp { get; set; }

        [BsonElement("website")]
        public string? Website { get; set; }

        [BsonElement("socialMedia")]
        public string? SocialMedia { get; set; }

        [BsonElement("keyContacts")]
        public List<KeyContact> KeyContacts { get; set; } = new();

        [BsonElement("accountingOffice")]
        public AccountingOffice? AccountingOffice { get; set; }

        // ===== Step 4: Corporate Structure =====
        // 'national' | 'foreign'
        [BsonElement("control")]
        public string Control { get; set; } = "national";

        [BsonElement("country")]
        public string? Country { get; set; }

        [BsonElement("finalBeneficiaries")]
        public List<Shareholder> FinalBeneficiaries { get; set; } = new();

        [BsonElement("administrators")]
        public List<Administrator> Administrators { get; set; } = new();

        [BsonElement("attorneys")]
        public List<Attorney> Attorneys { get; set; } = new();

        [BsonElement("relatedCompanies")]
        public List<RelatedCompany> RelatedCompanies { get; set; } = new();

        // ===== Step 5: Financial =====
        [BsonElement("socialCapital")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal SocialCapital { get; set; }

        [BsonElement("netWorth")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal NetWorth { get; set; }

        [BsonElement("revenue12m")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Revenue12m { get; set; }

        [BsonElement("relevantAssets")]
        public string? RelevantAssets { get; set; }

        [BsonElement("paymentTerms")]
        public string PaymentTerms { get; set; } = string.Empty;

        [BsonElement("creditLimit")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal CreditLimit { get; set; }

        // ===== Step 6: Banking =====
        [BsonElement("primaryBankAccount")]
        public BankAccount PrimaryBankAccount { get; set; } = new();

        [BsonElement("secondaryBankAccount")]
        public BankAccount? SecondaryBankAccount { get; set; }

        [BsonElement("pixKey")]
        public string? PixKey { get; set; }

        [BsonElement("beneficiary")]
        public string? Beneficiary { get; set; }

        // ===== Step 7: Operations =====
        [BsonElement("businessLine")]
        public string BusinessLine { get; set; } = string.Empty;

        [BsonElement("currentFleet")]
        public string CurrentFleet { get; set; } = string.Empty;

        [BsonElement("intendedUse")]
        public string IntendedUse { get; set; } = string.Empty;

        [BsonElement("preferredBrands")]
        public string PreferredBrands { get; set; } = string.Empty;

        [BsonElement("interestCategories")]
        public List<string> InterestCategories { get; set; } = new();

        // ===== Step 8: Licenses & Insurance =====
        [BsonElement("rntrc")]
        public string? Rntrc { get; set; }

        [BsonElement("rntrcValidity")]
        public string? RntrcValidity { get; set; }

        [BsonElement("operatingLicense")]
        public string? OperatingLicense { get; set; }

        [BsonElement("environmentalLicenses")]
        public string? EnvironmentalLicenses { get; set; }

        [BsonElement("insurances")]
        public List<Insurance> Insurances { get; set; } = new();

        // ===== Step 9: Compliance & LGPD =====
        [BsonElement("isPep")]
        public bool IsPep { get; set; }

        [BsonElement("pepRelationship")]
        public bool PepRelationship { get; set; }

        [BsonElement("pepName")]
        public string? PepName { get; set; }

        [BsonElement("pepCpf")]
        public string? PepCpf { get; set; }

        [BsonElement("dpoName")]
        public string? DpoName { get; set; }

        [BsonElement("dpoEmail")]
        public string? DpoEmail { get; set; }

        [BsonElement("legalBasis")]
        public string? LegalBasis { get; set; }

        [BsonElement("communicationConsent")]
        public bool CommunicationConsent { get; set; }

        [BsonElement("sharingConsent")]
        public bool SharingConsent { get; set; }

        // ===== Step 10: Documents =====
        [BsonElement("documents")]
        public Documents Documents { get; set; } = new();

        // ===== Profile data =====
        // 'client' | 'prospect'
        [BsonElement("status")]
        public string Status { get; set; } = "prospect";

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("kycScore")]
        public int KycScore { get; set; }

        // string para alinhar ao TS
        [BsonElement("nextReview")]
        public string NextReview { get; set; } = string.Empty;

        [BsonElement("vehicles")]
        public List<Vehicle> Vehicles { get; set; } = new();

        [BsonElement("completeness")]
        public int Completeness { get; set; }

        [BsonElement("monthlyTicket")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MonthlyTicket { get; set; }

        [BsonElement("vehicleCount")]
        public int VehicleCount { get; set; }

        [BsonElement("credit")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Credit { get; set; }

        // ===== Metadata (opcional) =====
        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? UpdatedAt { get; set; }
    }

    // ===== Subdocuments =====

    [BsonIgnoreExtraElements]
    public class Identification
    {
        [BsonElement("cnpj")]
        public string? Cnpj { get; set; }
    }

    // Remove this duplicate definition of the Address class if it exists in this file.
    // Ensure that only one definition of the Address class remains in the namespace.
    [BsonIgnoreExtraElements]

    public class AddressCompany
    {
        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("number")]
        public string Number { get; set; } = string.Empty;

        [BsonElement("complement")]
        public string? Complement { get; set; }

        [BsonElement("neighborhood")]
        public string Neighborhood { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class ContactCompany
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class KeyContact : ContactCompany
    {
        // 'fiscal' | 'financial' | 'logistics' | 'commercial'
        [BsonElement("role")]
        public string Role { get; set; } = "commercial";
    }

    [BsonIgnoreExtraElements]
    public class AccountingOffice
    {
        [BsonElement("companyName")]
        public string CompanyName { get; set; } = string.Empty;

        [BsonElement("crc")]
        public string Crc { get; set; } = string.Empty;

        [BsonElement("contact")]
        public ContactCompany Contact { get; set; } = new();
    }

    [BsonIgnoreExtraElements]
    public class Shareholder
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("percentage")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Percentage { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Administrator
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("instrument")]
        public string Instrument { get; set; } = string.Empty;

        // string para alinhar ao TS
        [BsonElement("date")]
        public string Date { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Attorney
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("powers")]
        public string Powers { get; set; } = string.Empty;

        [BsonElement("expirationDate")]
        public string ExpirationDate { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class RelatedCompany
    {
        [BsonElement("companyName")]
        public string CompanyName { get; set; } = string.Empty;

        [BsonElement("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [BsonElement("participationPercentage")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal ParticipationPercentage { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class BankAccount
    {
        [BsonElement("bank")]
        public string Bank { get; set; } = string.Empty;

        [BsonElement("agency")]
        public string Agency { get; set; } = string.Empty;

        [BsonElement("account")]
        public string Account { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Insurance
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("number")]
        public string Number { get; set; } = string.Empty;

        [BsonElement("validity")]
        public string Validity { get; set; } = string.Empty;

        [BsonElement("insurer")]
        public string Insurer { get; set; } = string.Empty;
    }

    [BsonIgnoreExtraElements]
    public class Vehicle
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("plate")]
        public string Plate { get; set; } = string.Empty;

        [BsonElement("usage")]
        public string Usage { get; set; } = string.Empty;

        [BsonElement("kycScore")]
        public int KycScore { get; set; }

        // 'active' | 'pending' | 'inactive'
        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("pendencies")]
        public List<string> Pendencies { get; set; } = new();
    }

    [BsonIgnoreExtraElements]
    public class Documents
    {
        [BsonElement("cnpjCard")]
        public string? CnpjCard { get; set; }

        [BsonElement("socialContract")]
        public string? SocialContract { get; set; }

        [BsonElement("registrations")]
        public string? Registrations { get; set; }

        [BsonElement("addressProof")]
        public string? AddressProof { get; set; }

        [BsonElement("clearances")]
        public string? Clearances { get; set; }

        [BsonElement("powerOfAttorney")]
        public string? PowerOfAttorney { get; set; }

        [BsonElement("digitalCertificate")]
        public string? DigitalCertificate { get; set; }

        [BsonElement("licenses")]
        public string? Licenses { get; set; }
    }
}
