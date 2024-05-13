using System.Data;
using Dapper;
using Database;
using Datebase;
using Npgsql;

namespace Logic;

public class ConceptTableCreator
{
    private readonly Connection _connection = new Connection();

    public async Task Migrate()
    {
        var query = new QueryObject("CREATE TABLE IF NOT EXISTS Concepts (id SERIAL PRIMARY KEY, db_name TEXT NOT NULL, view_name TEXT NOT NULL)");
        await _connection.Command(query).ConfigureAwait(false);
        query = new QueryObject("CREATE TABLE IF NOT EXISTS ConceptRelations (id SERIAL PRIMARY KEY, concept_id INT NOT NULL, related_concept_id INT NOT NULL, type INT NOT NULL)");
        await _connection.Command(query).ConfigureAwait(false);
    }
}
