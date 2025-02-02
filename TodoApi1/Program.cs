using Microsoft.EntityFrameworkCore;
using TodoApi;
var builder = WebApplication.CreateBuilder(args);
//cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//DbContext configuration
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));
//swagger
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//using in cors
app.UseCors();

// Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();  

// Define the API endpoints
//שליפה
app.MapGet("/items", async (ToDoDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});
//עדכון
app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if(item == null){
          return Results.NotFound($"Item with ID {id} not found.");
    }
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    //saving the item
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapPost("/items" , async(Item updatedItem,ToDoDbContext db )=>
{
    await db.Items.AddAsync(updatedItem);
    await db.SaveChangesAsync();
    return Results.Ok(updatedItem);
});


app.MapDelete("/items/{id}" , async(int id ,ToDoDbContext db)=>
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
