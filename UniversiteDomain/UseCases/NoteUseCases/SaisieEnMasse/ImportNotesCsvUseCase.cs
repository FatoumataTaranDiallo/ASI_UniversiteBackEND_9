using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.SaisieEnMasse;

public class ImportNotesCsvUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        var records = csv.GetRecords<NoteCsvDto>().ToList();

        foreach (var row in records)
        {
            // 1. Validation de la note
            if (row.Note < 0 || row.Note > 20)
                throw new Exception($"Erreur Etudiant {row.NumEtud} : La note doit être entre 0 et 20.");

            if (row.Note == null) continue;

            // 2. Récupération de l'étudiant
            var etudiants = await factory.EtudiantRepository().FindByConditionAsync(e => e.NumEtud == row.NumEtud);
            var etudiant = etudiants.FirstOrDefault();
            if (etudiant == null) throw new Exception($"L'étudiant {row.NumEtud} n'existe pas.");

            // 3. Récupération de l'UE
            var ues = await factory.UeRepository().FindByConditionAsync(u => u.NumeroUe == row.NumeroUe);
            var ue = ues.FirstOrDefault();
            if (ue == null) throw new Exception($"L'UE {row.NumeroUe} n'existe pas.");

            // 4. Gestion de la note (Mise à jour ou Création)
            var notesExistantes = await factory.NoteRepository().FindByConditionAsync(n => n.EtudiantId == etudiant.Id && n.UeId == ue.Id);
            var noteExistante = notesExistantes.FirstOrDefault();

            if (noteExistante != null)
            {
                // ELLE EXISTE : On change juste la valeur
                noteExistante.Valeur = (decimal)row.Note;
                await factory.NoteRepository().UpdateAsync(noteExistante); 
            }
            else
            {
                // ELLE N'EXISTE PAS : On la crée
                Note nouvelleNote = new Note
                {
                    Valeur = (decimal)row.Note,
                    EtudiantId = etudiant.Id,
                    UeId = ue.Id
                };
                await factory.NoteRepository().CreateAsync(nouvelleNote);
            }
        }
    }
}
