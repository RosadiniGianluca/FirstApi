using System;
using System.Collections.Generic;

namespace FirstApi.Entities;

public partial class UserEntity  // partial: allows to split the definition of a class or a struct, or an interface over two or more source files.
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

public class UserModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime EnrollmentDate { get; set; }
    public string Gender { get; set; }  // string instead of int

    public override string ToString()
    {
        return $"{FirstName} {LastName} ({UserName})";
    }
}



