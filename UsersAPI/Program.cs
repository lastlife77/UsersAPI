using Application;
using Data;
using Infrastructure;
using UsersAPI;
using UsersAPI.Configurations;

var builder = WebApplication.CreateBuilder(args);

// AddAsync services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .ConfigureInfrastructureServices(builder.Configuration)
    .ConfigureDataServices(builder.Configuration)
    .ConfigureApplicationServices();


builder.Services.ConfigureSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AuthorizedCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5024",
                "https://localhost:7186"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AuthorizedCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
