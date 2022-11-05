using System;

namespace CodeNotion.Template.Data.SqlServer.Models;

public class OrderConfirmation
{
    public Guid OrderId { get; set; }
    public int IdentityId { get; set; }
}