using System.Text.Json;
using WebApiLab.Console.Models;

HttpClient client = new HttpClient();

client.BaseAddress = new Uri("http://localhost:5185");

// HttpResponseMessage response = await client.GetAsync("/people");
HttpResponseMessage response = await client.GetAsync("/api/People");

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
else
{
    Console.WriteLine($"Error: {response.StatusCode}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

HttpResponseMessage singleResponse = await client.GetAsync("/api/People/V59OF92YF627HFY0");
// I think response should be singleResponse here, not response. Otherwise, it will always be true because the first response was successful.
if (response.IsSuccessStatusCode)
{
    string jsonResponse = await singleResponse.Content.ReadAsStringAsync();

    var person = JsonSerializer.Deserialize<Person>(
        jsonResponse,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    );
    // I'm not getting Name or Language or Version. I think the problem is that the property names in the JSON response don't match the property names in the Person class. I need to check the JSON response and make sure that the property names match.
    // 🚫 Swagger shows this error on the page:
    //      For 'id': Value must be an integer. 
    // But it is defined as a string in the Person class. I need to change the type of Id to int in the Person class.

    Console.WriteLine($"NAME: {person.Name} speaks {person.Language} {person.Version} {person.Id}");
}
else
{
    Console.WriteLine("------------------------------");
    Console.WriteLine($"Error: {singleResponse.StatusCode}");
    Console.WriteLine(await singleResponse.Content.ReadAsStringAsync());
}