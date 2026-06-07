using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTime RegisteredAt { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}