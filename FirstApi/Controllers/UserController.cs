using FirstApi.DTO;
using FirstApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("FistApi/[Controller]")]
    public class UserController : ControllerBase
    {
        // DBContext injection
        private readonly MyDbContext database;
        public UserController(MyDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult AllUsers(int? gender)
        {
            if(gender == null)
            {
                List<UserEntity> users = database.Users.ToList();
                List<UserModel> usersGenderResponses = users.Select(MapUserEntityToUserModel).ToList();

                return Ok(usersGenderResponses);
            }
            else
            {
                string genderString;
                switch (gender)
                {
                    case 1:
                        genderString = "Maschio";
                        break;
                    case 2:
                        genderString = "Femmina";
                        break;
                    case 3:
                        genderString = "Altro";
                        break;
                    default:
                        return BadRequest("Valore di genere inserito non valido");
                }

                List<UserEntity> users = database.Users.Where( user => user.Gender == gender).ToList();
                if (users.Count == 0)
                {
                    return NotFound("Nessun utente trovato");
                }

                List<UserModel> usersGenderResponses = users.Select(MapUserEntityToUserModel).ToList();

                return Ok(new
                {
                    Message = "Utenti trovati",
                    Gender = "Genere Cercato: " + genderString,
                    Users = usersGenderResponses
                });
            }

            
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = database.Users.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                return NotFound("Utente non trovato");
            }
            else
            {
                return Ok(MapUserEntityToUserModel(user));
            }
        }

        [HttpPost]
        public IActionResult AddUser( AddUserRequest dto)
        {
            var entity = new UserEntity
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Username,
                Password = dto.Password,
                EnrollmentDate = dto.EnrollmentDate,
                Gender = dto.Gender
            };

            database.Users.Add(entity);
            database.SaveChanges();  // SaveChanges() returns a Task<int> that indicates how many rows were affected by the change.

            return Ok("utente aggiunto"+ entity.ToString());  // returns 201 Created response with a location header. 
        }

        [HttpPut]
        public async Task<HttpStatusCode> UpdateUser(UpdateUserRequest dto)
        {
            var entity = await database.Users.FirstOrDefaultAsync(s => s.Id == dto.Id);
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.UserName = dto.Username;
            entity.Password = dto.Password;
            entity.EnrollmentDate = dto.EnrollmentDate;
            entity.Gender = dto.Gender;
            await database.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        
        
        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var userToDelete = database.Users.FirstOrDefault(user => user.Id == id);
            if (userToDelete == null)
            {
                return NotFound("Utente non trovato");
            }

            database.Users.Remove(userToDelete);
            database.SaveChanges();

            return Ok("Utente: "+userToDelete.ToString()+" eliminato");
        }


        // Maps the UserEntity to UserModel
        private UserModel MapUserEntityToUserModel(UserEntity user)
        {
            string genderString = GetUserGenderString(user.Gender);
            return new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Password = user.Password,
                EnrollmentDate = user.EnrollmentDate,
                Gender = genderString
            };
        }

        // Maps the gender int to a string
        private string GetUserGenderString(int gender)
        {
            switch (gender)
            {
                case 1:
                    return "Maschio";
                case 2:
                    return "Femmina";
                case 3:
                    return "Altro";
                default:
                    return "Sconosciuto";
            }
        }
    }


}