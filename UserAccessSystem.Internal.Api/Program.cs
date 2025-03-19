using UserAccessSystem.Internal.Application.Common;
using UserAccessSystem.Internal.Infrastructure.Common;
using UserAccessSystem.Internal.Persistence.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddApplication().AddInfrastrucutre().AddPersistence();

var app = builder.Build();

app.BuildPersistence();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
