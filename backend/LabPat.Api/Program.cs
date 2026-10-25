using LabPat.Api.Middleware;
using LabPat.Application.Common;
using LabPat.Application.Features.Admin;
using LabPat.Application.Features.Auth;
using LabPat.Application.Features.Insumos;
using LabPat.Application.Features.Laudos;
using LabPat.Application.Features.Perfil;
using LabPat.Application.Features.Solicitacoes;
using LabPat.Application.Features.TiposExame;
using LabPat.Application.Features.Veterinarios;
using LabPat.Application.Features.Vinculos;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using LabPat.Infrastructure.ExternalServices;
using LabPat.Infrastructure.Identity;
using LabPat.Infrastructure.Repositories;
using LabPat.Infrastructure.Security;
using LabPat.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddOpenApi();

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };

        // Conta ou laboratório desativados perdem o acesso na hora, mesmo com token ainda válido
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var id = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                var usuarios = context.HttpContext.RequestServices.GetRequiredService<IUsuarioRepository>();

                if (!int.TryParse(id, out var usuarioId) || !await usuarios.ContaAtivaAsync(usuarioId))
                    context.Fail("Conta desativada.");
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("Gemini");
builder.Services.AddScoped<IGeminiService, GeminiService>();

// Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITipoExameRepository, TipoExameRepository>();
builder.Services.AddScoped<IInsumoRepository, InsumoRepository>();
builder.Services.AddScoped<ISolicitacaoRepository, SolicitacaoRepository>();
builder.Services.AddScoped<ILaudoRepository, LaudoRepository>();
builder.Services.AddScoped<IVetSolicitanteRepository, VetSolicitanteRepository>();
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<ILaboratorioRepository, LaboratorioRepository>();
builder.Services.AddScoped<IConviteLaboratorioRepository, ConviteLaboratorioRepository>();
builder.Services.AddScoped<IVinculoVeterinarioRepository, VinculoVeterinarioRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Infraestrutura
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddSingleton<IPdfGenerator, QuestPdfGenerator>();

// Serviços de aplicação
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITipoExameService, TipoExameService>();
builder.Services.AddScoped<IInsumoService, InsumoService>();
builder.Services.AddScoped<ISolicitacaoService, SolicitacaoService>();
builder.Services.AddScoped<ILaudoService, LaudoService>();
builder.Services.AddScoped<IVeterinarioService, VeterinarioService>();
builder.Services.AddScoped<IVinculoService, VinculoService>();
builder.Services.AddScoped<IAcessoVeterinarioService, AcessoVeterinarioService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();

// Exceções de regra de negócio -> 400/403/409
builder.Services.AddExceptionHandler<ExcecaoHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Migrations automáticas em desenvolvimento; administrador criado a partir da configuração
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (app.Environment.IsDevelopment())
        await context.Database.MigrateAsync();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbSeeder.SeedAsync(context, app.Configuration, logger);
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
