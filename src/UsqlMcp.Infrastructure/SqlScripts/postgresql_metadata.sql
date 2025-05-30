-- PostgreSQL Metadata Extraction Queries

-- Tables Metadata
SELECT 
    t.table_name,
    t.table_type,
    obj_description(pgc.oid, 'pg_class') as table_description
FROM 
    information_schema.tables t
JOIN 
    pg_catalog.pg_class pgc ON t.table_name = pgc.relname
WHERE 
    t.table_schema = 'public'
ORDER BY 
    t.table_name;

-- Columns Metadata
SELECT 
    c.table_name,
    c.column_name,
    c.data_type,
    c.character_maximum_length,
    c.numeric_precision,
    c.numeric_scale,
    c.is_nullable,
    c.column_default,
    pg_catalog.col_description(pgc.oid, c.ordinal_position) as column_description
FROM 
    information_schema.columns c
JOIN 
    pg_catalog.pg_class pgc ON c.table_name = pgc.relname
WHERE 
    c.table_schema = 'public'
ORDER BY 
    c.table_name, c.ordinal_position;

-- Primary Keys
SELECT
    tc.table_name, 
    kc.column_name
FROM 
    information_schema.table_constraints tc
JOIN 
    information_schema.key_column_usage kc ON tc.constraint_name = kc.constraint_name
WHERE 
    tc.constraint_type = 'PRIMARY KEY' AND
    tc.table_schema = 'public'
ORDER BY 
    tc.table_name, kc.ordinal_position;

-- Foreign Keys
SELECT
    tc.table_name, 
    kcu.column_name, 
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM 
    information_schema.table_constraints tc
JOIN 
    information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name
JOIN 
    information_schema.constraint_column_usage ccu ON ccu.constraint_name = tc.constraint_name
WHERE 
    tc.constraint_type = 'FOREIGN KEY' AND
    tc.table_schema = 'public'
ORDER BY 
    tc.table_name, kcu.ordinal_position;

-- Indexes
SELECT
    t.relname AS table_name,
    i.relname AS index_name,
    a.attname AS column_name,
    ix.indisunique AS is_unique
FROM
    pg_class t,
    pg_class i,
    pg_index ix,
    pg_attribute a
WHERE
    t.oid = ix.indrelid
    AND i.oid = ix.indexrelid
    AND a.attrelid = t.oid
    AND a.attnum = ANY(ix.indkey)
    AND t.relkind = 'r'
ORDER BY
    t.relname, i.relname;

-- Views
SELECT
    table_name AS view_name,
    view_definition
FROM
    information_schema.views
WHERE
    table_schema = 'public';

-- Functions and Stored Procedures
SELECT
    p.proname AS procedure_name,
    pg_get_function_arguments(p.oid) AS args,
    pg_get_function_result(p.oid) AS result_type,
    p.prosrc AS definition
FROM
    pg_proc p
JOIN
    pg_namespace n ON p.pronamespace = n.oid
WHERE
    n.nspname = 'public'
ORDER BY
    p.proname;

-- Triggers
SELECT
    trigger_name,
    event_manipulation,
    event_object_table,
    action_statement,
    action_timing
FROM
    information_schema.triggers
WHERE
    trigger_schema = 'public'
ORDER BY
    event_object_table, trigger_name;