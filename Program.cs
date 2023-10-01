var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); // активируем порт для Swagger
builder.Services.AddSwaggerGen(); // регистрируем Swagger

// добавляем контекст работы с базой данных
builder.Services.AddDbContext<NoteDb>(options =>{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

// regitser repository
builder.Services.AddScoped<INoteRepository, NoteRepository>();

var app = builder.Build();
//var notes = new List<Note>();


// for process develop
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<NoteDb>();
    db.Database.EnsureCreated();
}

//app.MapGet("/", () => "Hello World!");
//app.MapGet("/notes", async (NoteDb db) => await db.Notes.ToListAsync()); // get all notes
app.MapGet("/notes", async (INoteRepository repository) =>
    Results.Ok(await repository.GetNotesAsync()))
    .Produces<List<Note>>(StatusCodes.Status200OK) // Swagger documentation
    .WithName("GetAllNotes")
    .WithTags("Getters");

/*app.MapGet("/notes/{id}", async (int id, NoteDb db) => await db.Notes.FirstOrDefaultAsync(n => n.Id == id)
    is Note note ? Results.Ok(note) : Results.NotFound()
    );*/ // get note

app.MapGet("/notes/{id}", async (int id, INoteRepository repository) =>
    await repository.GetNoteAsync(id) is Note note
    ? Results.Ok(note)
    : Results.NotFound())
    .Produces<Note>(StatusCodes.Status200OK)
    .WithName("GetNote")
    .WithTags("Getters");

/*app.MapPost("/notes", async ([FromBody] Note note, [FromServices] NoteDb db, HttpResponse responce) =>
    {
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return Results.Created($"/notes/{note.Id}", note);
    });*/ // add note

app.MapPost("/notes", async ([FromBody] Note note, INoteRepository repository) =>
    {
        await repository.InsertNoteAsync(note);
        await repository.SaveAsync();
        return Results.Created($"/notes/{note.Id}", note);
    })
    .Accepts<Note>("application/json")
    .Produces<Note>(StatusCodes.Status201Created)
    .WithName("CreateNote")
    .WithTags("Creators");

/*app.MapPut("/notes", async ([FromBody] Note note, NoteDb db) => {
    var noteFromDb = await db.Notes.FindAsync(new object[] {note.Id});
    if (noteFromDb == null) return Results.NotFound();
    noteFromDb.Title = note.Title;
    noteFromDb.Body = note.Body;
    await db.SaveChangesAsync();
    return Results.NoContent();
});*/ // edit note

app.MapPut("/notes", async ([FromBody] Note note, INoteRepository repository) => 
    {
        await repository.UpdateNoteAsync(note);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<Note>("application/json")
    .WithName("UpdateHotel")
    .WithTags("Updaters");

/*app.MapDelete("notes/{id}", async (int id, NoteDb db) => {
    var noteFromDb = await db.Notes.FindAsync(new object[] {id});
    if (noteFromDb == null) return Results.NotFound();
    db.Notes.Remove(noteFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});*/ // delete note

app.MapDelete("notes/{id}", async (int id, INoteRepository repository) => 
    {
        await repository.DeleteNoteAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .WithName("DeleteNote")
    .WithTags("Deleters");

app.MapGet("/notes/search/title/{query}", 
    async (string query, INoteRepository repository) =>
        await repository.GetNoteAsync(query) is IEnumerable<Note> notes
        ? Results.Ok(notes)
        : Results.NotFound(Array.Empty<Note>()))
        .Produces<List<Note>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("SearchHotels")
        .WithTags("Getters");

app.UseHttpsRedirection();

app.Run();
