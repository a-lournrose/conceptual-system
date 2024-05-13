using Database;
using Datebase;
using Logic.Enums;
using Logic.Models;

namespace Logic;

public class ConceptsRepository
{
    private readonly Connection _connection = new Connection();

    public async Task<Concept> CreateConcept(string conceptName)
    {
        var query = new QueryObject("INSERT INTO Concepts (db_name, view_name) VALUES (@dbName,@viewName) returning *",
            new { dbName = conceptName.GetGuid(), viewName = conceptName });
        return await _connection.CommandWithResponse<Concept>(query).ConfigureAwait(false);
    }

    public async Task<List<Concept>> GetAllConcepts()
    {
        var query = new QueryObject("SELECT * FROM Concepts");
        return await _connection.ListOrEmpty<Concept>(query).ConfigureAwait(false);
    }

    public async Task<Concept> GetConceptById(int id)
    {
        var query = new QueryObject("SELECT * FROM Concepts WHERE id = @id", new { id });
        return await _connection.FirstOrDefault<Concept>(query).ConfigureAwait(false);
    }

    public async Task CreateRelationConcepts(string conceptName, RelationType type, int? relatedConceptId = null)
    {
        var concept = await CreateConcept(conceptName);
        await CreateRelation(type, relatedConceptId, concept);
        QueryObject query;
        Concept relatedConcept;
        switch (type)
        {
            case RelationType.CreatingConcept:
                query = new QueryObject($"CREATE TABLE IF NOT EXISTS  {concept.db_name}  (id SERIAL PRIMARY KEY, value TEXT NOT NULL)");
                await _connection.Command(query).ConfigureAwait(false);
                break;
            case RelationType.WhichItIsPart:
                relatedConcept = await GetConceptById((int)relatedConceptId!);
                query = new QueryObject(
                    $"CREATE TABLE IF NOT EXISTS  {concept.db_name}  (id SERIAL PRIMARY KEY, {relatedConcept.db_name}_id INT NOT NULL REFERENCES {relatedConcept.db_name}(id))");
                await _connection.Command(query).ConfigureAwait(false);
                break;
            case RelationType.SpeciesConcept:
                relatedConcept = await GetConceptById((int)relatedConceptId!);
                query = new QueryObject(
                    $"CREATE TABLE IF NOT EXISTS  {concept.db_name}  (id SERIAL PRIMARY KEY, {relatedConcept.db_name}_id INT NOT NULL REFERENCES {relatedConcept.db_name}(id))");
                await _connection.Command(query).ConfigureAwait(false);
                break;
            case RelationType.PartOfConcept:
                relatedConcept = await GetConceptById((int)relatedConceptId!);
                query = new QueryObject($"CREATE TABLE IF NOT EXISTS  {concept.db_name}  (id SERIAL PRIMARY KEY, value TEXT NOT NULL)");
                await _connection.Command(query).ConfigureAwait(false);
                query = new QueryObject(
                    $"ALTER TABLE {relatedConcept.db_name} ADD COLUMN {concept.db_name}_id INT NOT NULL REFERENCES {concept.db_name}(id)");
                await _connection.Command(query).ConfigureAwait(false);
                break;
            case RelationType.SubclassOfConcept:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
                break;
            case RelationType.ConceptImageForConcept:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private async Task CreateRelation(RelationType type, int? relatedConceptId, Concept concept)
    {
        if (type == RelationType.CreatingConcept)
        {
            return;
        }
        var query = new QueryObject("INSERT INTO ConceptRelations (concept_id, related_concept_id, type) VALUES (@conceptId, @relatedConceptId, @type)",
            new { conceptId = concept.id, relatedConceptId, type });
        await _connection.Command(query).ConfigureAwait(false);
    }
}
