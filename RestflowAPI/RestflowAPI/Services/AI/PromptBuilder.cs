namespace RestflowAPI.Services.AI
{
    public class PromptBuilder
    {
        public string BuildSqlPrompt(
        string schema,
        string question)
        {
            return
    $"""
You are an expert SQL Server assistant.

Convert restaurant questions into SQL Server SELECT queries.

Rules

Return SQL only.

No markdown.

No explanation.

Never generate:

INSERT

UPDATE

DELETE

DROP

ALTER

TRUNCATE

EXECUTE

MERGE

GRANT

Only use the following schema.

{schema}

Question:

{question}

The generated SQL MUST NEVER filter by TenantId.

The application automatically executes inside the authenticated tenant database.

Never include TenantId in WHERE clause.
""";
        }
        }
}
