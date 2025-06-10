using ItauTest.Data;
using ItauTest.KafkaWorker;
using ItauTest.KafkaWorker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Registro do contexto do banco
builder.Services.AddDbContext<ItauDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Registro do serviço que consome o Kafka
builder.Services.AddSingleton<KafkaConsumerService>();


// Registro do Worker que roda em background
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
