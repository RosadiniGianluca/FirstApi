using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApi.Entities;

[Table("user")]
public partial class UserEntity  // partial: allows to split the definition of a class or a struct, or an interface over two or more source files.
{
    
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime EnrollmentDate { get; set; }

    public int Gender { get; set; }

    public int WorkId { get; set; }

    public WorkEntity? Work { get; set; } // Proprietà di navigazione per la relazione


    public override string ToString()
    {
        return $"{FirstName} {LastName} (Username: {UserName})";
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
    public WorkModel Job { get; set; } // Adding Work property to UserModel


    public override string ToString()
    {
        return $"{FirstName} {LastName} ({UserName})";
    }
}



