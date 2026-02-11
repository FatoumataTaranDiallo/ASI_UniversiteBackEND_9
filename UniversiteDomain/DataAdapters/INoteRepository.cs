using System.Linq.Expressions;
using UniversiteDomain.Entities;
namespace UniversiteDomain.DataAdapters;

public interface INoteRepository
{
    Task<Note> CreateAsync(Note note);
    Task<List<Note>> FindByConditionAsync(Expression<Func<Note, bool>> expression);
    // Optionnel : méthode pour trouver par Etudiant+Ue
    Task<Note?> FindByEtudiantAndUeAsync(long etudiantId, long ueId);
    Task UpdateAsync(Note note);

}