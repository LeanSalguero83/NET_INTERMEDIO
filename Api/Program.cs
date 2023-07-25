using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Services;
using Data;
using Data.Manager;
using Api.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var Origin = "*";


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: Origin, policy =>
    {
        policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();

    });
});
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProductosService, ProductosService>();
builder.Services.AddScoped<ProductosManager>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<UsuariosManager>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<RolesManager>();
builder.Services.AddScoped<IServiciosService, ServiciosService>();
builder.Services.AddScoped<ServiciosManager>();





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(Origin);

app.MapControllers();

app.Run();