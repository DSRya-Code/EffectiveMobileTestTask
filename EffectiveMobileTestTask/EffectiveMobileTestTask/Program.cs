using EffectiveMobileTestTask.BLL;
using EffectiveMobileTestTask.Extensions;
using Scalar.AspNetCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddApplicationServices();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapApplicationEndpoints();

        app.SeedNodeTree();

        app.Run();
    }

}