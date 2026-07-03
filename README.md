# Connecting a Web API to a Console Application

Creation of a .NET solution that contains at web API and a console application that consumes data from the API

> This is all I have for this README at this time.

> ### Why does my API port number keep changing?!?

Lock down the port # by stop using IIS Express and use the native .NET server profile instead.

- Navigate into your WebApiLab.API folder and run `dotnet run --launch-profile http`

This link is showing the data: `http://localhost:5195/api/People`

> Watch the class meet recording, then also look at my local `mslearn-create-razor-pages-aspnet-core` (what a friggin horrible folder name). That project is from [Create a web UI with ASP.NET Core](https://learn.microsoft.com/en-us/training/modules/create-razor-pages-aspnet-core/). That project is a different version of **ContosoPizza**.
