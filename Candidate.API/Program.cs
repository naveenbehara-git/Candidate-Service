using Candidate.API.Services;
using Candidate.Application.Services;
using Candidate.Infrastructure.Data;
using Candidate.Infrastructure.Repositories;
using Candidate.Domain.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DBConnection");

builder.Services.AddScoped<DbExecutor>(provider =>
{
    return new DbExecutor(connectionString);
});

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<DatabaseService>(options =>
                          options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICandidateService, CandidateService>();


var app = builder.Build();

app.MapGet("/api/candidate/get", async (string UserId, ICandidateService candidateService) =>
{
    var userGuid = Guid.Parse(UserId);
    var candidateProfile = await candidateService.GetCandidateProfileById(userGuid);
    return candidateProfile;
});

app.MapPost("/api/candidate/add", async (Candidate.Domain.Model.Candidate candidate, ICandidateService candidateService) =>
{
    var result = await candidateService.AddCandidateProfile(candidate);
    return result;
});

app.MapPut("/api/candidate/update", async (Candidate.Domain.Model.Candidate candidate, ICandidateService candidateService) =>
{
    var result = await candidateService.UpdateCandidateProfile(candidate);
    return result;
});


app.MapDelete("/api/candidate/{id}", async (Guid id, ICandidateService candidateService) =>
{
    var result = await candidateService.DeleteCandidateProfile(id);
    return result;
});

if (app.Environment.IsDevelopment())
{
    DbInitializer.Run(connectionString);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.MapControllers();

app.Run();
