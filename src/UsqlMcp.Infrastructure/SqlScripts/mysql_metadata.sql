-- MySQL Metadata Extraction Queries

-- Tables Metadata
SELECT 
    table_name,
    table_type,
    table_comment AS table_description
FROM 
    information_schema.tables
WHERE 
    table_schema = DATABASE()
ORDER BY 
    table_name;

-- Columns Metadata
SELECT 
    table_name,
    column_name,
    data_type,
    character_maximum_length,
    numeric_precision,
    numeric_scale,
    is_nullable,
    column_default,
    column_comment AS column_description
FROM 
    information_schema.columns
WHERE 
    table_schema = DATABASE()
ORDER BY 
    table_name, ordinal_position;

-- Primary Keys
SELECT 
    table_name,
    column_name
FROM 
    information_schema.key_column_usage
WHERE 
    constraint_name = 'PRIMARY' AND
    table_schema = DATABASE()
ORDER BY 
    table_name, ordinal_position;

-- Foreign Keys
SELECT 
    k.table_name,
    k.column_name,
    k.referenced_table_name AS foreign_table_name,
    k.referenced_column_name AS foreign_column_name
FROM 
    information_schema.key_column_usage k
JOIN 
    information_schema.table_constraints t ON k.constraint_name = t.constraint_name
WHERE 
    t.constraint_type = 'FOREIGN KEY' AND
    k.table_schema = DATABASE() AND
    k.referenced_table_name IS NOT NULL
ORDER BY 
    k.table_name, k.ordinal_position;

-- Indexes
SELECT 
    table_name,
    index_name,
    column_name,
    NOT non_unique AS is_unique
FROM 
    information_schema.statistics
WHERE 
    table_schema = DATABASE() AND
    index_name != 'PRIMARY'
ORDER BY 
    table_name, index_name, seq_in_index;

-- Views
SELECT 
    table_name AS view_name,
    view_definition
FROM 
    information_schema.views
WHERE 
    table_schema = DATABASE();

-- Stored Procedures
SELECT 
    routine_name AS procedure_name,
    routine_definition AS definition
FROM 
    information_schema.routines
WHERE 
    routine_schema = DATABASE() AND
    routine_type = 'PROCEDURE'
ORDER BY 
    routine_name;

-- Functions
SELECT 
    routine_name AS function_name,
    routine_definition AS definition
FROM 
    information_schema.routines
WHERE 
    routine_schema = DATABASE() AND
    routine_type = 'FUNCTION'
ORDER BY 
    routine_name;

-- Triggers
SELECT 
    trigger_name,
    event_manipulation AS trigger_event,
    action_statement AS trigger_body,
    action_timing AS trigger_timing,
    event_object_table AS table_name
FROM 
    information_schema.triggers
WHERE 
    trigger_schema = DATABASE()
ORDER BY 
    trigger_name;