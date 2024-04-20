using AutoMapper;
using BankApplication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BankApplication.Managers.CustomerManager;
using BankApplication.Managers.AccountManager;
using BankApplication.Managers.TransactionManager;
using BankApplication.Managers.EmployeeManager;
using BankApplication.Managers.AuthenticationManager;


namespace BankApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            builder.Services.AddControllers();
            builder.Services.AddDbContext<BankDBContext>((options)=>options.UseSqlServer(builder.Configuration.GetConnectionString("BankDbConnectionString")));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ICustomerManager, CustomerManager>();
            builder.Services.AddScoped<IEmployeeManager, EmployeeManager>();
            builder.Services.AddScoped<IAccountManager, AccountManager>();
            builder.Services.AddScoped<ITransactionManager,TransactionManager>();
            builder.Services.AddScoped<IAuthenticationManager,AuthenticationManger>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<BankDBContext>();
            builder.Services.AddScoped<SavingAccount>();
            builder.Services.AddScoped<CurrentAccount>();
            builder.Services.AddScoped<AccountFactory>();
            builder.Services.AddControllers().
                AddNewtonsoftJson((options) => 
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

           

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {


        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
            //building app variable
            var app = builder.Build();
            app.UseCors(mybuilder => mybuilder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}