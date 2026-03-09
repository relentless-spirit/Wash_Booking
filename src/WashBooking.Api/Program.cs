using System.Text.Json.Serialization;
using BuildingBlocks.Infrastructure.DependencyInjection;
using WashBooking.Api.DependencyInjections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var moduleAssemblies = new[]
{
    Modules.Users.Application.AssemblyReference.Assembly,
    // Mốt có module khác thì phẩy một cái rồi nhét vô đây:
    // Modules.Bookings.Application.AssemblyReference.Assembly,
    // Modules.Payments.Application.AssemblyReference.Assembly
};

builder.Services.AddBuildingBlocks(moduleAssemblies)
    .AddUsersModule(builder.Configuration); 

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();


await app.RunAsync();