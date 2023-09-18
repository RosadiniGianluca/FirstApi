using FirstApi.DTO;
using FirstApi.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("FirstApi/[Controller]")]
    public class WorkController : ControllerBase
    {
        //DBContext injection
        private readonly MyDbContext database;
        public WorkController(MyDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult AllWorks()
        {
            var works = database.Works;
            return Ok(works);
        }

        [HttpGet("{id}")]
        public IActionResult GetWorkById(int id)
        {
            var work = database.Works.Find(id);
            if (work == null)
            {
                return NotFound("Lavoro non trovato");
            }
            return Ok(work);
        }

        [HttpPost]
        public IActionResult AddWork([FromBody]AddWorkRequest dto)
        {
            if (!ModelState.IsValid)
            {
                // Restituisci un BadRequest con gli errori di validazione
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(errors);
            }

            // Il modello è valido, procedi con l'aggiunta dell'utente

            var entity = new WorkEntity
            {
                Name = dto.Name,
                Company = dto.Company,
                Salary = dto.Salary
            };

            database.Works.Add(entity);
            database.SaveChanges();
            return CreatedAtAction(nameof(GetWorkById), new { id = entity.Id }, entity);
        }

        [HttpPut]
        public IActionResult UpdateWork([FromBody]UpdateWorkRequest dto)
        {
            var existingWork = database.Works.Find(dto.Id);
            if (existingWork != null)
            {
                existingWork.Name = dto.Name ?? existingWork.Name;  // Se dto.Name è null, mantieni il valore precedente
                existingWork.Company = dto.Company ?? existingWork.Company;
                existingWork.Salary = dto.Salary ?? existingWork.Salary;
                database.SaveChanges();
                return Ok("Lavoro aggiornato: "+ existingWork.ToString());
            }
            return NotFound("Lavoro non trovato");
        }

        [HttpDelete]
        public IActionResult DeleteWork(int id)
        {
            var workToDelete = database.Works.Find(id);
            if (workToDelete != null)
            {
                database.Works.Remove(workToDelete);
                database.SaveChanges();
                return Ok("Lavoro eliminato: "+ workToDelete.ToString());
            }
            return NotFound("Lavoro non trovato");
        }
    }
}
