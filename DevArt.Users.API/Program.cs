using DevArt.Users.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}


builder.AddApplicationService();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
}

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.RunMigration();

app.Run();

