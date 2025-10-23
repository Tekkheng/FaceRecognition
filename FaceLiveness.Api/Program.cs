using FaceLiveness.Application.Interfaces;
using FaceLiveness.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var visionBase = builder.Configuration["VISION_BASEURL"] ?? "http://vision:5000";

builder.Services.AddRazorPages();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("vision", c => { c.Timeout = TimeSpan.FromSeconds(30); });

builder.Services.AddScoped<IFaceClient>(sp =>
{
    var http = sp.GetRequiredService<System.Net.Http.IHttpClientFactory>().CreateClient("vision");
    return new PythonFaceClient(http, visionBase);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapRazorPages();

app.Run();
