using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Application;
using ProyectoAtlas.Application.Documentations;
using ProyectoAtlas.Application.Projects;
using ProyectoAtlas.Infrastructure.Documentations;
using ProyectoAtlas.Infrastructure.Persistence;
using ProyectoAtlas.Infrastructure.Projects;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<HealthCheckUseCase>();
builder.Services.AddScoped<CreateProjectUseCase>();
builder.Services.AddScoped<ListProjectsUseCase>();
builder.Services.AddScoped<GetProjectBySlugUseCase>();
builder.Services.AddScoped<UpdateProjectUseCase>();
builder.Services.AddScoped<DeleteProjectUseCase>();
builder.Services.AddScoped<CreateProjectDocumentationUseCase>();
builder.Services.AddScoped<ListProjectDocumentationsUseCase>();
builder.Services.AddScoped<GetProjectDocumentationBySlugUseCase>();
builder.Services.AddScoped<UpdateProjectDocumentationUseCase>();
builder.Services.AddScoped<DeleteProjectDocumentationUseCase>();


builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDocumentationRepository, DocumentationRepository>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProyectoAtlasDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "Proyecto Atlas API v1");
    options.DocumentTitle = "Proyecto Atlas API";
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
