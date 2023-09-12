﻿using FirstApi.DTO;
using FirstApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("FirstApi/[Controller]")]
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
            IQueryable<UserEntity> query = database.Users;  // IQueryable: rappresenta una sequenza di dati che può essere eseguita in modo remoto o locale senza specificare il tipo di dati.

            if (gender.HasValue)
            {
                query = query.Where(user => user.Gender == gender.Value);
            }

            var usersWithWork = query  
                .Join(  
                    database.Works, // Tabella "lavoro" nel database
                    user => user.WorkId,
                    work => work.Id,
                    (user, work) => new UserModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Password = user.Password,
                        EnrollmentDate = user.EnrollmentDate,
                        Gender = GetUserGenderString(user.Gender), // Metodo statico privato per ottenere la stringa del genere
                        Job = new WorkModel // Crea un oggetto WorkModel per il lavoro
                        {
                            Name = work.Name,       // Nome del lavoro dalla tabella "lavoro"
                            Company = work.Company  // Nome dell'azienda dalla tabella "lavoro"
                        }
                    })
                .OrderBy(user => user.Id)
                .ToList();

            return Ok(usersWithWork);
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            // LINQ query: join tra le tabelle "utenti" e "lavoro" per ottenere il lavoro dell'utente
            var userWithWork = database.Users
                .Where(user => user.Id == id)
                .Join(
                    database.Works, // Tabella "works" nel database
                    user => user.WorkId,
                    work => work.Id,
                    (user, work) => new UserModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Password = user.Password,
                        EnrollmentDate = user.EnrollmentDate,
                        Gender = GetUserGenderString(user.Gender),
                        Job = new WorkModel
                        {
                            Name = work.Name,
                            Company = work.Company
                        }
                    })
                .FirstOrDefault();

            if (userWithWork == null)
            {
                return NotFound("Utente non trovato");
            }
            return Ok(userWithWork);
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
                Gender = dto.Gender,
                WorkId = dto.WorkId
            };

            database.Users.Add(entity);
            database.SaveChanges();  // salvataggio delle modifiche nel database

            return Ok("Utente aggiunto: "+ entity.ToString());  // returns 201 Created response with a location header. 
        }

        [HttpPut]
        public IActionResult UpdateUser(UpdateUserRequest dto)
        {
            var existingUser = database.Users.FirstOrDefault(user => user.Id == dto.Id);
            if (existingUser != null)
            {
                existingUser.FirstName = dto.FirstName;
                existingUser.LastName = dto.LastName;
                existingUser.UserName = dto.Username;
                existingUser.Password = dto.Password;
                existingUser.EnrollmentDate = dto.EnrollmentDate;
                existingUser.Gender = dto.Gender;
                existingUser.WorkId = dto.WorkId;
                database.SaveChanges();
                return Ok("Utente aggiornato: " + existingUser.ToString() + "\n");
            }
            else
            {
                return NotFound("Utente non trovato");
            }
        }

        
        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var userToDelete = database.Users.FirstOrDefault(user => user.Id == id);
            if (userToDelete != null)
            {
                database.Users.Remove(userToDelete);
                database.SaveChanges();
                return Ok("Utente: " + userToDelete.ToString() + " eliminato");
            }
            else
            {
                return NotFound("Utente non trovato");
            }
        }

        // Maps the gender int to a string
        private static string GetUserGenderString(int gender)
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