using Microsoft.EntityFrameworkCore;
using PoChunSu_BackendTest_MidLevel.Models; // 引入你的 Models

var builder = WebApplication.CreateBuilder(args);

// 加入服務到容器 (Add services to the container)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 👇 就是少了這一段！告訴系統要怎麼建立 MyofficeContext，並去讀取剛剛的連線字串
builder.Services.AddDbContext<MyofficeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
