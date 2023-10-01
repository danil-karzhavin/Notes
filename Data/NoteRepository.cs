// через этот класс обращается к базе данных и получаем из нее данные.
public class NoteRepository : INoteRepository
{
    private NoteDb _context; // контекст базы данных
    public NoteRepository(NoteDb context)
    {
        _context = context;
    }
    public Task<List<Note>> GetNotesAsync() => _context.Notes.ToListAsync();

    public async Task<Note> GetNoteAsync(int noteId) =>
        await _context.Notes.FindAsync(new object[]{noteId}); // поиск по первичному ключу.

    public Task<List<Note>> GetNoteAsync(string title) =>
        _context.Notes.Where(note => note.Title.Contains(title)).ToListAsync();

    public async Task InsertNoteAsync(Note note) => await _context.Notes.AddAsync(note);

    public async Task UpdateNoteAsync(Note note)
    {
        var noteFromBd = await _context.Notes.FindAsync(new object[]{note.Id});
        if (noteFromBd == null) return;
        noteFromBd.Title = note.Title;
        noteFromBd.Body = note.Body;
    }

    public async Task DeleteNoteAsync(int noteId)
    {
        var noteFromBd = await _context.Notes.FindAsync(new object[]{noteId});
        if (noteFromBd == null) return;
        _context.Notes.Remove(noteFromBd);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    private bool _disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}