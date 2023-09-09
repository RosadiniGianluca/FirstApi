using FirstApi.DTO;
using FirstApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // DBContext injection
        private readonly MyDbContext database;
        public UserController(MyDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(database.Users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await database.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpPost]
        public IActionResult AddUser( UserDTO dto)
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
            database.SaveChanges();  // SaveChangesAsync() returns a Task<int> that indicates how many rows were affected by the change. await is used to wait for the task to complete before continuing execution of the program.

            return Ok("utente aggiunto"+ entity.ToString());  // returns 201 Created response with a location header. 
        }

        [HttpPut("UpdateUser")]
        public async Task<HttpStatusCode> UpdateUser(UserDTO User)
        {
            var entity = await database.Users.FirstOrDefaultAsync(s => s.Id == User.Id);
            entity.FirstName = User.FirstName;
            entity.LastName = User.LastName;
            entity.UserName = User.Username;
            entity.Password = User.Password;
            entity.EnrollmentDate = User.EnrollmentDate;
            entity.Gender = User.Gender;
            await database.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        // Which one is better?
        [HttpDelete("DeleteUser/{Id}")]
        public async Task<HttpStatusCode> DeleteUserById(int Id)
        {
            var entity = new UserEntity()
            {
                Id = Id
            };
            database.Users.Attach(entity);
            database.Users.Remove(entity);
            await database.SaveChangesAsync();
            return HttpStatusCode.OK;
        }
        
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await database.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            database.Users.Remove(user);
            await database.SaveChangesAsync();

            return Ok("Utente "+user.ToString()+" cancellato dal DB.");
        }
    }

}