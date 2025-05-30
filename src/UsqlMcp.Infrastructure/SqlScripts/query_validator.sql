-- SQL Query Validator Functions
-- These functions can be used to validate that SQL queries are read-only
-- They are provided as templates and should be implemented in application code

-- PostgreSQL Query Validator Function
/*
CREATE OR REPLACE FUNCTION is_read_only_query(query_text TEXT) RETURNS BOOLEAN AS $$
DECLARE
    normalized_query TEXT;
    contains_write_operations BOOLEAN;
BEGIN
    -- Normalize the query (remove comments, extra whitespace, make lowercase)
    normalized_query := regexp_replace(lower(query_text), '--.*$', '', 'gm');
    normalized_query := regexp_replace(normalized_query, '/\*.*?\*/', '', 'g');
    normalized_query := regexp_replace(normalized_query, '\s+', ' ', 'g');
    
    -- Check for write operations
    contains_write_operations := 
        normalized_query ~* '\s*insert\s+' OR
        normalized_query ~* '\s*update\s+' OR
        normalized_query ~* '\s*delete\s+' OR
        normalized_query ~* '\s*truncate\s+' OR
        normalized_query ~* '\s*drop\s+' OR
        normalized_query ~* '\s*alter\s+' OR
        normalized_query ~* '\s*create\s+' OR
        normalized_query ~* '\s*grant\s+' OR
        normalized_query ~* '\s*revoke\s+' OR
        normalized_query ~* '\s*with\s+.*\s+update' OR
        normalized_query ~* '\s*with\s+.*\s+delete' OR
        normalized_query ~* '\s*with\s+.*\s+insert' OR
        normalized_query ~* '\s*set\s+' OR
        normalized_query ~* '\s*call\s+' OR
        normalized_query ~* '\s*execute\s+' OR
        normalized_query ~* '\s*do\s+';
    
    RETURN NOT contains_write_operations;
END;
$$ LANGUAGE plpgsql;
*/

-- SQL Server Query Validator Function
/*
CREATE FUNCTION dbo.is_read_only_query(@query_text NVARCHAR(MAX))
RETURNS BIT
AS
BEGIN
    DECLARE @normalized_query NVARCHAR(MAX);
    DECLARE @contains_write_operations BIT;
    
    -- Normalize the query (remove comments, extra whitespace, make lowercase)
    SET @normalized_query = LOWER(@query_text);
    
    -- Check for write operations
    SET @contains_write_operations = 
        CASE WHEN @normalized_query LIKE '%insert %' THEN 1
             WHEN @normalized_query LIKE '%update %' THEN 1
             WHEN @normalized_query LIKE '%delete %' THEN 1
             WHEN @normalized_query LIKE '%truncate %' THEN 1
             WHEN @normalized_query LIKE '%drop %' THEN 1
             WHEN @normalized_query LIKE '%alter %' THEN 1
             WHEN @normalized_query LIKE '%create %' THEN 1
             WHEN @normalized_query LIKE '%grant %' THEN 1
             WHEN @normalized_query LIKE '%revoke %' THEN 1
             WHEN @normalized_query LIKE '%with % update %' THEN 1
             WHEN @normalized_query LIKE '%with % delete %' THEN 1
             WHEN @normalized_query LIKE '%with % insert %' THEN 1
             WHEN @normalized_query LIKE '%exec %' THEN 1
             WHEN @normalized_query LIKE '%execute %' THEN 1
             ELSE 0
        END;
    
    RETURN CASE WHEN @contains_write_operations = 1 THEN 0 ELSE 1 END;
END;
*/

-- MySQL Query Validator Function
/*
DELIMITER //
CREATE FUNCTION is_read_only_query(query_text TEXT) RETURNS BOOLEAN
DETERMINISTIC
BEGIN
    DECLARE normalized_query TEXT;
    
    -- Normalize the query (make lowercase)
    SET normalized_query = LOWER(query_text);
    
    -- Check for write operations
    RETURN NOT (
        normalized_query REGEXP '\\s*insert\\s+' OR
        normalized_query REGEXP '\\s*update\\s+' OR
        normalized_query REGEXP '\\s*delete\\s+' OR
        normalized_query REGEXP '\\s*truncate\\s+' OR
        normalized_query REGEXP '\\s*drop\\s+' OR
        normalized_query REGEXP '\\s*alter\\s+' OR
        normalized_query REGEXP '\\s*create\\s+' OR
        normalized_query REGEXP '\\s*grant\\s+' OR
        normalized_query REGEXP '\\s*revoke\\s+' OR
        normalized_query REGEXP '\\s*call\\s+' OR
        normalized_query REGEXP '\\s*lock\\s+'
    );
END //
DELIMITER ;
*/

-- Oracle Query Validator Function
/*
CREATE OR REPLACE FUNCTION is_read_only_query(p_query_text IN VARCHAR2) RETURN NUMBER IS
    l_normalized_query VARCHAR2(32767);
BEGIN
    -- Normalize the query (make lowercase)
    l_normalized_query := LOWER(p_query_text);
    
    -- Check for write operations
    IF REGEXP_LIKE(l_normalized_query, '\s*insert\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*update\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*delete\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*truncate\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*drop\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*alter\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*create\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*grant\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*revoke\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*execute\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*begin\s+') OR
       REGEXP_LIKE(l_normalized_query, '\s*declare\s+') THEN
        RETURN 0; -- Not read-only
    ELSE
        RETURN 1; -- Read-only
    END IF;
END;
/
*/

-- Implementation Notes:
-- 1. These functions are provided as templates and should be adapted to your specific database system.
-- 2. For a database-agnostic approach, consider implementing query validation in the application layer using:
--    a. A SQL parser library (e.g., ANTLR with SQL grammar)
--    b. Regular expressions to detect write operations (less reliable but simpler)
-- 3. Security considerations:
--    a. Always use parameterized queries to prevent SQL injection
--    b. Consider using database roles with read-only permissions for additional security
--    c. This validation should be a secondary defense, not the primary security mechanism