using System.Text;
using CarteiraDigitalAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CarteiraDigitalAPI.Configurations;
using CarteiraDigitalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona controllers (quando você for criar AuthController etc)
builder.Services.AddControllers();

// Configurar JWT
builder.Services.Configure<ConfiguracaoJwt>(
    builder.Configuration.GetSection("Jwt"));

// Recupera configuração apenas para o TokenValidationParameters
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<ConfiguracaoJwt>();
if (jwtConfig == null || string.IsNullOrEmpty(jwtConfig.ChaveSecreta))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid.");
}
var chave = Encoding.ASCII.GetBytes(jwtConfig.ChaveSecreta);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opcoes =>
{
    opcoes.RequireHttpsMetadata = false;
    opcoes.SaveToken = true;
    opcoes.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(chave),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtConfig.Emissor,
        ValidAudience = jwtConfig.Audiencia
    };
});

builder.Services.AddScoped<ICarteiraService, CarteiraService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Ativa Swagger no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Executa migrations e popula o banco de dados
using (var escopo = app.Services.CreateScope())
{
    var context = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    DbSeeder.Popular(context);
}

app.Run();
