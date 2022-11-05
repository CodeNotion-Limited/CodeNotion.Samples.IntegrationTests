using System.Collections.Generic;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class Customer : IEntity
{
    public int Id { get; set; }
    public int IdentityId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? BusinessName { get; set; }
    public string? Vat { get; set; }
    public string? TaxCode { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? ReferrerFirstName { get; set; }
    public string? ReferrerLastName { get; set; }
    public string? ReferrerPhoneNumber { get; set; }
    public string? BusinessPhoneNumber { get; set; }

    public int InternalNumber { get; set; } // TODO keep it? 
    public string? AbiCode { get; set; }
    public string? CabCode { get; set; }
    public string? IbanCode { get; set; }
    public string? SwiftCode { get; set; }
    public ICollection<Order>? Orders { get; set; }
}