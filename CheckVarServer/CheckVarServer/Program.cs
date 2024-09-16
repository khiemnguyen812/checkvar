using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowAllOrigins");

app.MapPost("/checkvar", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
    var keyword = data["keyword"];

    var filePath = "data.csv"; 
    var pythonScriptPath = "solve.py"; 

    var quotedKeyword = $"\"{keyword}\"";

    var start = new ProcessStartInfo
    {
        FileName = "python",
        Arguments = $"{pythonScriptPath} {filePath} {quotedKeyword}",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true, 
        CreateNoWindow = true,
        StandardOutputEncoding = Encoding.UTF8, 
        StandardErrorEncoding = Encoding.UTF8 
    };

    using var process = Process.Start(start);
    using var outputReader = process.StandardOutput;
    var result = await outputReader.ReadToEndAsync();
    process.WaitForExit();

    // Check for errors
    var error = await process.StandardError.ReadToEndAsync();
    if (!string.IsNullOrEmpty(error))
    {
        Console.WriteLine("Error: " + error);
        return Results.Problem("Error executing Python script: " + error);
    }

    // Check for result
    if (string.IsNullOrEmpty(result))
    {
        Console.WriteLine("No output from Python script.");
        return Results.Problem("No output from Python script.");
    }

    var results = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(result);

    return Results.Json(results);
});

app.Run();