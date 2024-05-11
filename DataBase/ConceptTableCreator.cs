using System;
using System.Data;
using Npgsql;
using Dapper;

public class ConceptTableCreator
{
    private readonly string _connectionString;

    public ConceptTableCreator()
    {
        _connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
    }

    public void CreateConceptTable(string conceptName, string[] fields, string parentConcept = null)
    {
        using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
        {
            dbConnection.Open();

            // Проверяем, существует ли понятие
            var concept = dbConnection.QueryFirstOrDefault("SELECT * FROM Concepts WHERE ConceptName = @ConceptName", new { ConceptName = conceptName });
            int conceptId;
            if (concept == null)
            {
                // Если понятие не существует, добавляем его в базу данных
                conceptId = dbConnection.ExecuteScalar<int>("INSERT INTO Concepts (ConceptName, ParentConceptId) VALUES (@ConceptName, NULL) RETURNING ConceptId",
                    new { ConceptName = conceptName });
            }
            else
            {
                conceptId = concept.ConceptId;
            }

            // Создаем таблицу
            var tableName = $"{conceptName}_table";
            dbConnection.Execute($"CREATE TABLE IF NOT EXISTS {tableName} (id SERIAL PRIMARY KEY)");

            // Добавляем ссылку на родительскую таблицу, если она есть
            if (!string.IsNullOrEmpty(parentConcept))
            {
                var parentConceptId = dbConnection.ExecuteScalar<int>("SELECT ConceptId FROM Concepts WHERE ConceptName = @ParentConceptName",
                    new { ParentConceptName = parentConcept });
                dbConnection.Execute($"UPDATE ()");
            }
            
            // Добавляем информацию о созданной таблице в базу данных
            dbConnection.Execute("INSERT INTO CreatedTables (TableName) VALUES (@TableName)", new { TableName = tableName });

            // Добавляем поля
            foreach (var field in fields)
            {
                var normalizedFieldName = NormalizeFieldName(field);
                dbConnection.Execute($"ALTER TABLE {tableName} ADD COLUMN {normalizedFieldName} TEXT");
                // Добавляем информацию о поле в базу данных
                dbConnection.Execute("INSERT INTO CreatedFields (FieldName, TableId, ConceptId) VALUES (@FieldName, @TableId, @ConceptId)",
                    new { FieldName = field, TableId = tableName, ConceptId = conceptId });
            }

           
        }
    }

    private string NormalizeFieldName(string fieldName)
    {
        // Метод для преобразования имени поля в название, которое может быть использовано в SQL
        return fieldName.ToLower().Replace(" ", "_");
    }

    public void CreateConceptsTable()
    {
        using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
        {
            dbConnection.Open();

            var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Concepts (
                ConceptId SERIAL PRIMARY KEY,
                ConceptName TEXT UNIQUE,
                ParentConceptId INT REFERENCES Concepts(ConceptId)
            );
        ";

            dbConnection.Execute(createTableQuery);
        }
    }

    public void CreateCreatedTablesTable()
    {
        using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
        {
            dbConnection.Open();

            var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS CreatedTables (
                TableId SERIAL PRIMARY KEY,
                TableName TEXT UNIQUE
            );
        ";

            dbConnection.Execute(createTableQuery);
        }
    }

    public void CreateCreatedFieldsTable()
    {
        using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
        {
            dbConnection.Open();

            var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS CreatedFields (
                FieldId SERIAL PRIMARY KEY,
                FieldName TEXT,
                TableId TEXT REFERENCES CreatedTables(TableName),
                ConceptId INT REFERENCES Concepts(ConceptId)
            );
        ";

            dbConnection.Execute(createTableQuery);
        }
    }
}
