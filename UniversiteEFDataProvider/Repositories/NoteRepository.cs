using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context)
    : INoteRepository
{
    private UniversiteDbContext Context => context;

    public async Task<Note> CreateAsync(Note note)
    {
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();
        return note;
    }

    public async Task<List<Note>> FindByConditionAsync(
        Expression<Func<Note, bool>> expression)
    {
        return await Context.Notes
            .Where(expression)
            .ToListAsync();
    }

    public async Task<Note?> FindByEtudiantAndUeAsync(long etudiantId, long ueId)
    {
        return await Context.Notes.FindAsync(etudiantId, ueId);
    }
    public async Task UpdateAsync(Note note) 
    {
        Context.Notes.Update(note);
        await Context.SaveChangesAsync();
    }
}