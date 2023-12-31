﻿using FirstApi.Authentication;
using FirstApi.Properties;
using FirstApi.Clients;
using FirstApi.DTO;
using FirstApi.Entities;
using FirstApi.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.Extensions.Options;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("FirstApi/[Controller]")]
    //[ServiceFilter(typeof(ApiKeyAuthenticationFilter))] // Aggiunge il filtro ad ogni richiesta HTTP del controller
    public class UserController : ControllerBase
    {
        // DBContext injection
        private readonly MyDbContext database;
        // WebhookClient injection
        private readonly WebhookClient webhookClient;
        // ServiceBusMessageHandler injection
        private readonly ServiceBusMessageHandler _messageHandler;

        public UserController(MyDbContext database, WebhookClient webhookClient, IOptions<ServiceBusConfig> serviceBusConfig)
        {
            this.database = database;
            this.webhookClient = webhookClient;
            this._messageHandler = new ServiceBusMessageHandler(serviceBusConfig);
        }

        [HttpGet, ServiceFilter(typeof(ApiKeyAuthenticationFilter))] // Aggiunge il filtro solo a questa richiesta HTTP
        public IActionResult AllUsers(int? gender, int? workId)
        {
            IQueryable<UserEntity> query = database.Users;  // IQueryable: interfaccia che rappresenta una query LINQ, ma non viene eseguita finché non viene chiamato un metodo di esecuzione (ToList(), First(), ecc.)

            if (gender.HasValue)
            {
                if (gender < 1 || gender > 3)
                {
                    return BadRequest("Valore del genere non valido");
                }
                query = query.Where(user => user.Gender == gender.Value);
            }

            if (workId.HasValue)
            {
                query = query.Where(user => user.WorkId == workId.Value);
            }

            var usersWithWork = query
                .Include(user => user.Work) // Caricamento anticipato dei dati del lavoro (eager loading)
                .OrderBy(user => user.Id)
                .ToList()
                .Select(user => MapUserEntityToUserModel(user))
                .ToList();                  // Converti gli utenti in una lista per post Webhook

            if (usersWithWork.Count != 0)
            {
                // Invia una POST con tutti gli utenti come corpo della richiesta
                webhookClient.SendPostRequest(usersWithWork);
                return Ok(usersWithWork);
            }

            webhookClient.SendPostRequest("Nessun utente come risultato della GET, query param non valida");  // Invia una POST con un messaggio di errore come corpo della richiesta
            return NotFound("Nessun utente trovato");
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = database.Users
                .Include(u => u.Work) // Eager loading dei dati del lavoro (caricamento anticipato)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Utente non trovato");
            }

            return Ok(MapUserEntityToUserModel(user));
        }

        private UserModel MapUserEntityToUserModel(UserEntity user)
        {
            return new UserModel
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
                    Name = user.Work?.Name,       // Accesso ai dati del lavoro tramite navigazione
                    Company = user.Work?.Company  // Accesso ai dati dell'azienda tramite navigazione
                }
            };
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


        [HttpPost]
        public IActionResult AddUser(AddUserRequest dto)
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

            // return Ok("Utente aggiunto: "+ entity.ToString());  // returns 201 Created response with a location header. 
            // Restituisci una risposta 201 Created con il nuovo utente
            return CreatedAtAction(nameof(GetUserById), new { id = entity.Id }, entity);
        }

        [HttpPut]
        public IActionResult UpdateUser(UpdateUserRequest dto)
        {
            try 
            {
                var existingUser = database.Users.FirstOrDefault(user => user.Id == dto.Id);
                if (existingUser != null)
                {
                    existingUser.FirstName = dto.FirstName ?? existingUser.FirstName;
                    existingUser.LastName = dto.LastName ?? existingUser.LastName;
                    existingUser.UserName = dto.Username ?? existingUser.UserName;
                    existingUser.Password = dto.Password;
                    existingUser.EnrollmentDate = dto.EnrollmentDate;
                    existingUser.Gender = dto.Gender;
                    existingUser.WorkId = dto.WorkId;
                    database.SaveChanges();
                    return Ok("Utente aggiornato: " + existingUser.ToString() + "\n");
                }
                return NotFound("Utente non trovato");
                
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Errore durante l'aggiornamento dell'utente");
            }
            catch (Exception ex) 
            {
                // Gestisci l'eccezione e restituisci una risposta appropriata
                return StatusCode(500, "Si è verificato un errore durante l'aggiornamento.");
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
    }
}