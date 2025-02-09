using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure DbContext
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

// Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Cors Usage
app.UseCors();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Define the API endpoints
app.MapGet("/items", async (ToDoDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});

app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if(item == null)
    {
        return Results.NotFound($"Item with ID {id} not found.");
    }
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapPost("/items", async (Item newItem, ToDoDbContext db) =>
{
    await db.Items.AddAsync(newItem);
    await db.SaveChangesAsync();
    return Results.Ok(newItem);
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound($"Item with ID {id} not found.");
    }
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapGet("/", () => "Todo List is running..");

app.Run();
