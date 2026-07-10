using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PotyRetiradasApi.Configurations;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services;
using PotyRetiradasApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true);

var connectionString =
    builder.Configuration.GetConnectionString("RetiradasDb")
    ?? throw new InvalidOperationException(
        "A connection string 'RetiradasDb' não foi configurada.");

builder.Services.AddDbContext<RetiradasDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<AzureStorageOptions>(
    builder.Configuration.GetSection("AzureStorage"));

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("As configurações de JWT não foram encontradas.");

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
{
    throw new InvalidOperationException("A configuração 'Jwt:Issuer' não foi configurada.");
}

if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
    throw new InvalidOperationException("A configuração 'Jwt:Audience' não foi configurada.");
}

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException("A configuração 'Jwt:Key' não foi configurada.");
}

if (jwtOptions.Key.Length < 32)
{
    throw new InvalidOperationException("A configuração 'Jwt:Key' deve possuir pelo menos 32 caracteres.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PotyRetiradasWeb", policy =>
    {
        policy
            .WithOrigins(
                "https://potyretiradas.bebidaspoty.com.br"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IUnidadeRepository, UnidadeRepository>();
builder.Services.AddScoped<IUnidadeService, UnidadeService>();

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();

builder.Services.AddScoped<ITipoRepository, TipoRepository>();
builder.Services.AddScoped<ITipoService, TipoService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IConfiguracaoMensalProdutoRepository, ConfiguracaoMensalProdutoRepository>();
builder.Services.AddScoped<IConfiguracaoMensalProdutoService, ConfiguracaoMensalProdutoService>();

builder.Services.AddScoped<IRetiradaMensalProdutoRepository, RetiradaMensalProdutoRepository>();

builder.Services.AddScoped<IRetiradaMensalRepository, RetiradaMensalRepository>();
builder.Services.AddScoped<IRetiradaMensalService, RetiradaMensalService>();

builder.Services.AddScoped<IRetiradaMensalProdutoRepository, RetiradaMensalProdutoRepository>();
builder.Services.AddScoped<IRetiradaMensalProdutoService, RetiradaMensalProdutoService>();

builder.Services.AddScoped<IArquivoStorageService, AzureBlobStorageService>();

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/openapi/v1.json",
        "Poty Retiradas API v1");

    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Poty Retiradas API";
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("PotyRetiradasWeb");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
