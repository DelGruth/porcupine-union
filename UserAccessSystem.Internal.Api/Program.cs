using System.Text.Json.Serialization;
using UserAccessSystem.Internal.Application.Common;
using UserAccessSystem.Internal.Infrastructure.Common;
using UserAccessSystem.Internal.Persistence.Common;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.IncludeFields = true;
    });
;

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
