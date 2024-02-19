using GoldenPagesUz.Data;
using GoldenPagesUz.Middlewares;
using GoldenPagesUz.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using TelegramSink;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<YpDbContext>(options =>
    options.UseSqlite("Data source=yp.db"));

builder.Services.AddHttpClient();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IYpService, YpService>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("./Logging/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.TeleSink("6905369580:AAG-fnIZpuN-NFEI7kFchiRVLIag9xUbx74", "-1002046841304")
    .CreateLogger();

var app = builder.Build(); 
 
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();