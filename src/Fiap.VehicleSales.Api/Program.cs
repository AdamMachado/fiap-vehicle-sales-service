using Fiap.VehicleSales.Application.UseCases.Sales;
using Fiap.VehicleSales.Application.UseCases.Vehicles;
using Fiap.VehicleSales.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Fiap.VehicleSales.Infrastructure.DependencyInjection;
using Fiap.VehicleSales.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fiap Vehicle Sales API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Informe apenas o token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Em ambiente de teste, a infraestrutura será registrada pela CustomWebApplicationFactory com SQLite.
// Em execução normal, usa PostgreSQL via AddInfrastructure.
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

builder.Services.AddScoped<CreateVehicleUseCase>();
builder.Services.AddScoped<UpdateVehicleUseCase>();
builder.Services.AddScoped<GetVehicleByIdUseCase>();
builder.Services.AddScoped<ListAvailableVehiclesUseCase>();
builder.Services.AddScoped<ListSoldVehiclesUseCase>();
builder.Services.AddScoped<PurchaseVehicleUseCase>();
builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Keycloak:Authority"];

            var metadataAddress = builder.Configuration["Keycloak:MetadataAddress"];
            if (!string.IsNullOrWhiteSpace(metadataAddress))
                options.MetadataAddress = metadataAddress;

            options.Audience = builder.Configuration["Keycloak:Audience"];
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "preferred_username",
                RoleClaimType = ClaimTypes.Role,
                ValidateAudience = false
            };
        });
}

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<VehicleSalesDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
