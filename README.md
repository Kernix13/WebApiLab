# Connecting a Web API to a Console Application

Creation of a .NET solution that contains a wWb API and a Console application that consumes data from the API.

> This is all I have for this README at this time.

This link is showing the data: `http://localhost:5195/api/People`

> Look at my local `mslearn-create-razor-pages-aspnet-core`. That project is from [Create a web UI with ASP.NET Core](https://learn.microsoft.com/en-us/training/modules/create-razor-pages-aspnet-core/). That project is a different version of **ContosoPizza**.

<span aria-hidden="true"><br></span>

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download) 8.0
- [Visual Studio Code](https://code.visualstudio.com/) with [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

<span aria-hidden="true"><br></span>

## Installation & Usage

1. Clone this repository and switch into project folder

   ```sh
   git clone https://github.com/Kernix13/WebApiLab.git
   cd WebApiLab
   ```

2. Run the application

   ```bash
   # add commands later
   ```

3. Build the application

   ```bash
   # add commands later
   ```

### <span aria-hidden="true">⚡</span> Quick Start

```sh
git clone https://github.com/Kernix13/WebApiLab.git
cd WebApiLab
# add commands later
```

<span aria-hidden="true"><br></span>

## Important notes and code from this project

### Lesson 1 Solution Setup

#### Notes

- Go to http://localhost:5195/Swagger/index.html

#### Shell commands

```sh
# Create .NET solution file and folder, then cd into folder
dotnet new sln -n WebApiLab -o WebApiLab
cd WebApiLab

# Create a C# Web API project
dotnet new webapi -o WebApiLab.API -n WebApiLab.API
# Connect the project to the solution file
dotnet sln add WebApiLab.API/WebApiLab.API.csproj
# Build our solution to check project
dotnet build
# Now listening on: http://localhost:5195

cd WebApiLab.API
# Add Swagger
dotnet add package Swashbuckle.AspNetCore # or
dotnet add WebApiLab.API/WebApiLab.API.csproj package Swashbuckle.AspNetCore
```

#### Code

```cs
// in Progam.cs of API project, add the following
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// inside the if statement add:
app.UseSwagger();
app.UseSwaggerUI();

// Go to http://localhost:5195/swagger
```

### Lesson 2 Web API Setup

#### Notes

- Download the [dummy data](https://microsoftedge.github.io/Demos/json-dummy-data/64KB.json), create a folder named `Resources` and place the file in there. It should be named `64KB.json`
- create a `Models` folder and add `Person.cs` to it
- `record` vs `class`
- PropertyNameCaseInsensitive:
- the flag `-p` is short for `--project`

#### Code

```cs
// in Models/Person.cs
namespace WebApiLab.API.Models;

public record Person(string Name, string Language, string Id, string Bio, decimal Version);
```

```cs
// in Program.cs
using System.Text.Json;
using WebApiLab.API.Models;

// read dummy data into memory
string jsonFile = File.ReadAllText("./Resources/64KB.json");
// deserialize the file into a List of our Person object
var jsonData = JsonSerializer.Deserialize<List<Person>>(
    jsonFile,
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

// HTTP GET method that the jsonData (List of Person objects) called GetPeople
app.MapGet(pattern: "/people", handler: () => jsonData)
    .WithName(endpointName: "GetPeople")
    .WithOpenApi()
    .Produces<List<Person>>(statusCode: StatusCodes.Status200OK);
```

```sh
# Run project from WebApiLab root
dotnet run -p WebApiLab.API/WebApiLab.API.csproj
# Go to http://localhost:5195/swagger click "Try it out" and then the "Execute"
```

### Lesson 3 Console Application

> What is required to create a web API and how to consume it?

#### Shell commands

```sh
# create console application
dotnet new console -n WebApiLab.Console -o WebApiLab.Console
# add new application to the solution
dotnet sln add WebApiLab.Console/WebApiLab.Console.csproj
# build from root
dotnet build
# run console app
dotnet run --project WebApiLab.Console/WebApiLab.Console.csproj
```

#### Notes

The point of a web API: to share information with other apps or end users. In order to reach out to our web API, we are going to need three things:

1. A JSON deserializer,
1. an HTTP client (to call the API), and
1. a model for the data we are getting back

- Create a `Models` folder > add `Person.cs`
- HTTP client request -> returns an object of type `HttpResponseMessage`
- `IsSuccessStatusCode`: property on `HttpResponseMessage` holds a boolean for the request
- `GetAsync`:

#### Code

```cs
// Models/Person.cs
namespace WebApiLab.Console.Models;

public record Person(string Name, string Language, string Id, string Bio, decimal Version);

// Program.cs
using System.Text.Json;
using WebApiLab.Console.Models;

// setup HTTP client
HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5195");
HttpResponseMessage response = await client.GetAsync("/people");

if (response.IsSuccessStatusCode)
{
    string jsonResponse = await response.Content.ReadAsStringAsync();

    var people = JsonSerializer.Deserialize<List<Person>>(
        jsonResponse,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    foreach (var person in people)
    {
        Console.WriteLine($"{person.Name} speaks {person.Language}");
    }
}
```

### Lesson 4 Controller-Based APIs

> Convert minimal API to an API that uses controllers

#### Notes

- Controllers are fully-implemented classes in the .NET framework that are responsible for handling HTTP requests.
- These classes are created in separate files as opposed to handling all of our HTTP requests in Program.cs

Controller:

- add a `Controllers` folder - add `PeopleController.cs`
- [ApiController] which tells .NET that this is an API controller and
- [Route] which contains data about what route should be used to access this controller.
- HttpGet: responds if an HTTP GET request is sent to the controller
- `dotnet run --project WebApiLab.API/WebApiLab.API.csproj`
- `IActionResult` use case: Complex logic with highly dynamic responses.
  - It gives you total freedom to return any status code helper method (`Ok`, `NotFound`, `BadRequest`).
  - However, to document the API for tools like Swagger, you have to manually decorate the method with attributes
- `ActionResult` use case: Custom action result types or legacy MVC.
  - It implicitly casts data models into a 200 OK response while still allowing you to return standard HTTP status code results.
  - Swagger reads the `<Product>` type directly without requiring type-mapping attributes

#### Code

```cs
// in PeopleController.cs
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApiLab.API.Models;

namespace WebApiLab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    // List of People objects
    private List<Person> People = new List<Person>();

    public PeopleController()
    {
        // read from JSON file then add it to People object
        // 🚫 this is not normally how data is loaded up in real world applications
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

    // POST: api/People

    // PUT: api/People/{id}

    // DELETE: api/People/{id}
}
```

```cs
// in Program.cs
// Tell WebApplicationBuilder controllers will be used
builder.Services.AddControllers();

// tell app to map the controllers we have in the Controllers folder
app.MapControllers();
```

```cs
// in Console Program.cs
// HttpResponseMessage response = await client.GetAsync("/people");
HttpResponseMessage response = await client.GetAsync("/api/People");
```

Your console app will now consume the new API method we created using the PeopleController

### Lesson 5 Adding Lookup Functionality

...handle requests for a single known element in the list of items... add a new controller method

#### Code

- start by creating a new path to get an item by its ID: `api/People/{id}`
- `HttpGet` attribute creates that new path and accepts an ID as input as part of the URL
- use Linq to filter the list of people bu the URL `id`
- The `FirstOrDefault` method will return either a matching record of type `Person` or `null`
- If person is `null`, we return the correct HTTP code to relay to the program that is consuming our API that nothing was found
- next update console app to use the new API method (see below)

```cs
// in PeopleController.cs below GetAll
// GET: api/People/{id}
[HttpGet("{id}")]
public ActionResult<Person> GetPerson(string id)
{
   var person = People.FirstOrDefault(p => p.Id == id);
   // var person = People.FirstOrDefault(p => p.Id == id.ToString());
   if (person == null)
   {
      return NotFound();
   }
   return Ok(person);
}
```

```cs
// in Program.cs
// Get person by Id
HttpResponseMessage singleResponse = await client.GetAsync("/api/People/V59OF92YF627HFY0");
if (response.IsSuccessStatusCode)
{
    string jsonResponse = await singleResponse.Content.ReadAsStringAsync();

    var person = JsonSerializer.Deserialize<Person>(
        jsonResponse,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    );

    Console.WriteLine("------------------------------");
    Console.WriteLine($"{person.Name} speaks {person.Language}");
}
```

### Lesson 6 Consume Web API using React (Optional)

#### Shell commands

```sh
npm create vite@latest
# Wrong, do this:
npm create vite@latest
# For project name, use WebApiLab.Web
# For package name, use webapilab-web
# choose ESLint

# or this
npx create-vite@latest WebApiLab.Web

npm install
npm run dev
```

#### Notes

- the fix for the CORS error is bad - it configures your API to accept requests from any origin

#### Code

```jsx
// in App.jsx
useEffect(() => {
  const fetchData = async () => {
    try {
      const response = await fetch('http://localhost:5195/api/People');

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const result = await response.json();
      setData(result);
    } catch (error) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };
  fetchData();
}, []);
```

```cs
// in API Program.cs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

app.UseCors();
```
