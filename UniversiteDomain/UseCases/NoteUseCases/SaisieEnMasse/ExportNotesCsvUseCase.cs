using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;

namespace UniversiteDomain.UseCases.NoteUseCases.SaisieEnMasse;

public class ExportNotesCsvUseCase(IRepositoryFactory factory)
{
    public async Task<string> ExecuteAsync(long ueId)
    {
        // 1. On récupère l'UE
        var ue = await factory.UeRepository().FindAsync(ueId);
        if (ue == null) throw new Exception("UE non trouvée");

        // 2. On récupère les notes de cette UE (celles que tu vois dans Rider !)
        var toutesLesNotes = await factory.NoteRepository().FindByConditionAsync(n => n.UeId == ueId);
        
        var csvData = new List<NoteCsvDto>();

        // 3. On parcourt les notes et on récupère l'étudiant pour chaque note
        foreach (var note in toutesLesNotes)
        {
            var etud = await factory.EtudiantRepository().FindAsync(note.EtudiantId);

            if (etud != null)
            {
                csvData.Add(new NoteCsvDto
                {
                    NumeroUe = ue.NumeroUe,
                    IntituleUe = ue.Intitule, 
                    NumEtud = etud.NumEtud,
                    Nom = etud.Nom,
                    Prenom = etud.Prenom,
                    Note = (float?)note.Valeur
                });
            }
        }

        // 4. Écriture du fichier CSV
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });
    
        await csv.WriteRecordsAsync(csvData);
        return writer.ToString();
    } 
}