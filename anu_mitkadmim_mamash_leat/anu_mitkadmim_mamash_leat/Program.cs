using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using anu_mitkadmim_mamash_leat.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using anu_mitkadmim_mamash_leat.Controllers;
using Microsoft.AspNetCore.Builder;
using anu_mitkadmim_mamash_leat.hub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

builder.Services.AddDbContext<anu_mitkadmim_mamash_leatContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("anu_mitkadmim_mamash_leatContext") ?? throw new InvalidOperationException("Connection string 'anu_mitkadmim_mamash_leatContext' not found.")));

builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTParams:Audience"],
        ValidIssuer = builder.Configuration["JWTParams:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTParams:SecretKey"]))
    };
});

builder.Services.AddScoped<serviceContacts>();
builder.Services.AddScoped<indipandesService>();

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All", builder => { builder.WithOrigins("http://localhost:3000/").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});
*/
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All", builder => { builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials(); });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("Allow All");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<Class>("/Class");
});

app.Run();
