using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhotoGallery.Core.Domain.Entities;
using PhotoGallery.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

app.Run();
