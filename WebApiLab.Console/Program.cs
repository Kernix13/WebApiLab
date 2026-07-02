using System.Text.Json;
using WebApiLab.Console.Models;

HttpClient client = new HttpClient();

client.BaseAddress = new Uri("http://localhost:5195");

HttpResponseMessage response = await client.GetAsync("/people");
// HttpResponseMessage response = await client.GetAsync("/api/People");

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