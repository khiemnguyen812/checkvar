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

var lines = File.ReadAllLines("data.csv");

app.MapPost("/checkvar", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
    var keyword = data["keyword"];

    var results = new List<Dictionary<string, object>>();

    //example line: 5218.87149 01/09/2024,,3000.0,,272986.010924.101858.DO DUC LOI chuyen tien
    foreach (var line in lines)
    {
        bool isMatch = line.Contains(keyword);
        if (isMatch)
        {
            var parts = line.Split(',');
            var DocNo = parts[0];
            var Credit = parts[2];
            var TransactionsInDetail = line.Split(",,").Last();
            var obj = new Dictionary<string, object>
            {
                { "DocNo", DocNo },
                { "Credit", Credit },
                { "TransactionsInDetail", TransactionsInDetail }
            };
            results.Add(obj);
        }
    }
    return Results.Json(results);   
});

app.Run();