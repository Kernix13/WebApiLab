using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApiLab.API.Models;

namespace WebApiLab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private List<Person> People = new List<Person>();

    public PeopleController()
    {
        string jsonFile = System.IO.File.ReadAllText("./Resources/64KB.json");
        var peopleData = JsonSerializer.Deserialize<List<Person>>(
            jsonFile,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (peopleData != null)
        {
            People = peopleData;
        }
    }

    // GET: api/People
    [HttpGet]
    public ActionResult<List<Person>> GetAll()
    {
        return Ok(People);
    }

    // GET: api/People/{id}
    [HttpGet("{id}")]
    public ActionResult<Person> GetPerson(string id)
    {
        var person = People.FirstOrDefault(p => p.Id == id);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    // POST: api/People
    [HttpPost]
    public IActionResult CreatePerson(Person newPerson)
    {
        // Send back the data with a 201 Created status
        return CreatedAtAction(nameof(GetPerson), new { id = newPerson.Id }, newPerson);
    }

    // Having trouble with this - do I need Services?
    // PUT: api/People/{id}
    [HttpPut("{id}")]
    public IActionResult UpdatePerson(string id, Person updatedPerson)
    {
        return NoContent();
    }

    // DELETE: api/People/{id}
    [HttpDelete("{id}")]
    public IActionResult DeletePerson(string id)
    {
        var person = People.FirstOrDefault(p => p.Id == id);
        if (person == null)
        {
            return NotFound();
        }

        People.Remove(person);
        return NoContent();
    }
}