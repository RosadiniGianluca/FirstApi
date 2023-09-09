using System;
using System.Collections.Generic;

namespace FirstApi.Entities;

public partial class UserEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime EnrollmentDate { get; set; }

    public int Gender { get; set; }

    public override string ToString()
    {
        return $"{FirstName} {LastName} ({UserName})";
    }

}



