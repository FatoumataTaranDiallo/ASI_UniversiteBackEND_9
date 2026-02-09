using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context)
    : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        Parcours p = (await Context.Parcours.FindAsync(parcours.Id))!;
        Etudiant e = (await Context.Etudiants.FindAsync(etudiant.Id))!;

        p.Inscrits!.Add(e);
        await Context.SaveChangesAsync();

        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;

        p.Inscrits!.Add(e);
        await Context.SaveChangesAsync();

        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        Parcours p = (await Context.P­­arcours.FindAsync(parcours!.Id))!;

        foreach (Etudiant etud in etudiants)
        {
            Etudiant e = (await Context.Etudiants.FindAsync(etud.Id))!;
            p.Inscrits!.Add(e);
        }

        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;

        foreach (long id in idEtudiants)
        {
            Etudiant e = (await Context.Etudiants.FindAsync(id))!;
            p.Inscrits!.Add(e);
        }

        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        Ue ue = (await Context.Ues.FindAsync(idUe))!;

        p.UesEnseignees!.Add(ue);
        await Context.SaveChangesAsync();

        return p;
    }
}
