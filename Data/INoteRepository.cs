public interface INoteRepository : IDisposable
{
    Task<List<Note>> GetNotesAsync();
    Task<List<Note>> GetNoteAsync(string title);
    Task<Note> GetNoteAsync(int noteId);
    Task InsertNoteAsync(Note note);
    Task UpdateNoteAsync(Note note);
    Task DeleteNoteAsync(int noteId);
    Task SaveAsync();
}