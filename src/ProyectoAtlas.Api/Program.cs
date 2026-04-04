using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAtlas.Api.Errors;
using ProyectoAtlas.Api.OpenApi;
using ProyectoAtlas.Application.Errors;
using ProyectoAtlas.Infrastructure.DocumentationRelations;
using ProyectoAtlas.Infrastructure.DocumentationVersions;
using ProyectoAtlas.Infrastructure.Documentations;
using ProyectoAtlas.Infrastructure.Features;
using ProyectoAtlas.Infrastructure.Persistence;
using ProyectoAtlas.Infrastructure.Projects;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.InvalidModelStateResponseFactory = _ =>
  {
    ApiErrorResponse error = new(
        StatusCode: StatusCodes.Status400BadRequest,
        Message: "The request payload is invalid.",
        Code: AtlasErrorCodes.ValidationError);

    return new BadRequestObjectResult(error);
  };
});


builder.Services.AddScoped<HealthCheckQueryHandler>();
builder.Services.AddScoped<CreateProjectCommandHandler>();
builder.Services.AddScoped<ListProjectsQueryHandler>();
builder.Services.AddScoped<GetProjectBySlugQueryHandler>();
builder.Services.AddScoped<UpdateProjectCommandHandler>();
builder.Services.AddScoped<DeleteProjectCommandHandler>();
builder.Services.AddScoped<CreateProjectDocumentationCommandHandler>();
builder.Services.AddScoped<ListProjectDocumentationsQueryHandler>();
builder.Services.AddScoped<GetProjectDocumentationBySlugQueryHandler>();
builder.Services.AddScoped<UpdateProjectDocumentationCommandHandler>();
builder.Services.AddScoped<DeleteProjectDocumentationCommandHandler>();
builder.Services.AddScoped<ListDocumentationVersionsQueryHandler>();
builder.Services.AddScoped<GetDocumentationVersionByNumberQueryHandler>();
builder.Services.AddScoped<CreateDocumentationRelationCommandHandler>();
builder.Services.AddScoped<ListDocumentationRelationsQueryHandler>();
builder.Services.AddScoped<DeleteDocumentationRelationCommandHandler>();
builder.Services.AddScoped<CreateProjectFeatureCommandHandler>();
builder.Services.AddScoped<ListProjectFeaturesQueryHandler>();
builder.Services.AddScoped<GetProjectFeatureBySlugQueryHandler>();
builder.Services.AddScoped<UpdateProjectFeatureCommandHandler>();
builder.Services.AddScoped<DeleteProjectFeatureCommandHandler>();


builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDocumentationRepository, DocumentationRepository>();
builder.Services.AddScoped<IDocumentationVersionRepository, DocumentationVersionRepository>();
builder.Services.AddScoped<IDocumentationRelationRepository, DocumentationRelationRepository>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
  options.AddSchemaTransformer(OpenApiExampleTransformers.ApplyExamples);
});

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
app.UseMiddleware<ApiExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
