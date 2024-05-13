using System.Text;
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

    public async Task<Concept> GetConceptRelations(int? conceptId)
    {
        var query = new QueryObject(
            $@"SELECT c.*, cr.type AS Type
FROM concepts c
JOIN ConceptRelations cr ON c.id = cr.related_concept_id
WHERE cr.concept_id = @conceptId AND cr.type <> 3
UNION ALL
SELECT c.*, cr.type AS Type
FROM concepts c
JOIN ConceptRelations cr ON c.id = cr.concept_id
WHERE cr.related_concept_id = @conceptId AND cr.type = 3 ",
            new { conceptId });
        var relations = await _connection.ListOrEmpty<ConceptRelation>(query).ConfigureAwait(false);
        var concept = await GetConceptById((int)conceptId!);
        concept.Relations = relations;
        return concept;
    }
    
    public async Task<object> GetConceptData(string conceptName)
    {
       
        var query = await GenerateQuery(conceptName);
        return await _connection.ListOrEmpty<object>(new QueryObject(query)).ConfigureAwait(false);
    } 


    private async Task<string> GenerateQuery(string conceptName)
    {
        var queryBuilder = new StringBuilder();

        var tables = await GetTablesForConcept(conceptName);

        queryBuilder.Append("SELECT ");

        for (var i = 0; i < tables.Count; i++)
        {
            if (i != 0)
                queryBuilder.Append(", ");

            queryBuilder.Append($"{tables[i]}.value AS {tables[i]}_value");
        }

        queryBuilder.Append(" FROM ");

        for (var i = 0; i < tables.Count; i++)
        {
            if (i != 0)
                queryBuilder.Append(" JOIN ");

            queryBuilder.Append($"public.{tables[i]} ON public.concepts.db_name = '{tables[i]}'");
        }

        queryBuilder.Append(" WHERE ");

        queryBuilder.Append($"public.concepts.view_name = '{conceptName}'");

        return queryBuilder.ToString();
    }

     private async Task<List<string>> GetTablesForConcept(string conceptName)
    {
        var query_db_name = new QueryObject("SELECT db_name FROM Concepts WHERE view_name = @conceptName", new { conceptName });
        var db_name = await _connection.FirstOrDefault<string>(query_db_name).ConfigureAwait(false);
        var query = new QueryObject("WITH RECURSIVE referenced_tables AS (\n    SELECT DISTINCT ccu.table_name\n    FROM information_schema.table_constraints tc\n    JOIN information_schema.key_column_usage kcu\n        ON tc.constraint_name = kcu.constraint_name\n        AND tc.table_schema = kcu.table_schema\n    JOIN information_schema.constraint_column_usage ccu\n        ON ccu.constraint_name = tc.constraint_name\n        AND ccu.table_schema = tc.table_schema\n    WHERE kcu.table_name = @table_name\n        AND kcu.table_schema = 'public'\n        AND tc.constraint_type = 'FOREIGN KEY'\n    UNION\n    SELECT DISTINCT ccu.table_name\n    FROM information_schema.table_constraints tc\n    JOIN information_schema.key_column_usage kcu\n        ON tc.constraint_name = kcu.constraint_name\n        AND tc.table_schema = kcu.table_schema\n    JOIN information_schema.constraint_column_usage ccu\n        ON ccu.constraint_name = tc.constraint_name\n        AND ccu.table_schema = tc.table_schema\n    JOIN referenced_tables rt\n        ON kcu.table_name = rt.table_name\n        AND kcu.table_schema = 'public'\n        AND tc.constraint_type = 'FOREIGN KEY'\n        AND EXISTS (\n            SELECT 1\n            FROM information_schema.columns\n            WHERE table_schema = 'public'\n            AND table_name = ccu.table_name\n            AND column_name = 'value'\n        )\n)\nSELECT *\nFROM referenced_tables;", new { table_name = db_name });
        return await _connection.ListOrEmpty<string>(query).ConfigureAwait(false);
    }
}
