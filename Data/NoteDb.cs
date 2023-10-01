// определяем контекст базы данных, который является основным классом, для взаимодействия 
//с EntityFrameWork
public class NoteDb : DbContext
{
    public NoteDb(DbContextOptions<NoteDb> options) : base(options){}
    // => ознаыает, что вызов свойства Notes вернет результат вызова метода Set<Note>
    public DbSet<Note> Notes => Set<Note>();
}
