using Lemax_Take_Home.Authorization;
using Lemax_Take_Home.Mappings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Take_Home.DAL.InMemory;
using Take_Home.DAL.Interfaces;
using Take_Home.Services;
using Take_Home.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddControllers();

//configure swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using username and password."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basicAuth"
                                }
                            },
                            new string[] {}
                    }
                });
});

//configure authentication
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

//configure repositories
builder.Services.AddSingleton<IHotelCRUDService, HotelCRUDService>();
builder.Services.AddSingleton<IHotelRepository, HotelRepository>();

//configure services
builder.Services.AddSingleton<IApplicationUserService, SimpleApplicationUserService>();

//configure automapper
builder.Services.AddAutoMapper(typeof(MapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
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
