-- Oracle Metadata Extraction Queries

-- Tables Metadata
SELECT 
    table_name,
    'TABLE' AS table_type,
    comments AS table_description
FROM 
    all_tab_comments
WHERE 
    owner = USER AND
    table_type = 'TABLE'
ORDER BY 
    table_name;

-- Columns Metadata
SELECT 
    c.table_name,
    c.column_name,
    c.data_type,
    c.char_length AS character_maximum_length,
    c.data_precision AS numeric_precision,
    c.data_scale AS numeric_scale,
    DECODE(c.nullable, 'Y', 'YES', 'NO') AS is_nullable,
    c.data_default AS column_default,
    cc.comments AS column_description
FROM 
    all_tab_columns c
LEFT JOIN 
    all_col_comments cc ON c.owner = cc.owner AND c.table_name = cc.table_name AND c.column_name = cc.column_name
WHERE 
    c.owner = USER
ORDER BY 
    c.table_name, c.column_id;

-- Primary Keys
SELECT 
    ac.table_name,
    acc.column_name
FROM 
    all_constraints ac
JOIN 
    all_cons_columns acc ON ac.owner = acc.owner AND ac.constraint_name = acc.constraint_name
WHERE 
    ac.constraint_type = 'P' AND
    ac.owner = USER
ORDER BY 
    ac.table_name, acc.position;

-- Foreign Keys
SELECT 
    ac.table_name,
    acc.column_name,
    r_ac.table_name AS foreign_table_name,
    r_acc.column_name AS foreign_column_name
FROM 
    all_constraints ac
JOIN 
    all_cons_columns acc ON ac.owner = acc.owner AND ac.constraint_name = acc.constraint_name
JOIN 
    all_constraints r_ac ON ac.r_owner = r_ac.owner AND ac.r_constraint_name = r_ac.constraint_name
JOIN 
    all_cons_columns r_acc ON r_ac.owner = r_acc.owner AND r_ac.constraint_name = r_acc.constraint_name
WHERE 
    ac.constraint_type = 'R' AND
    ac.owner = USER
ORDER BY 
    ac.table_name, acc.position;

-- Indexes
SELECT 
    ai.table_name,
    ai.index_name,
    aic.column_name,
    DECODE(ai.uniqueness, 'UNIQUE', 1, 0) AS is_unique
FROM 
    all_indexes ai
JOIN 
    all_ind_columns aic ON ai.owner = aic.index_owner AND ai.index_name = aic.index_name
WHERE 
    ai.owner = USER AND
    ai.index_type != 'LOB'
ORDER BY 
    ai.table_name, ai.index_name, aic.column_position;

-- Views
SELECT 
    view_name,
    text AS view_definition
FROM 
    all_views
WHERE 
    owner = USER
ORDER BY 
    view_name;

-- Stored Procedures and Functions
SELECT 
    object_name AS procedure_name,
    object_type,
    TO_CLOB(dbms_metadata.get_ddl(object_type, object_name)) AS definition
FROM 
    all_objects
WHERE 
    owner = USER AND
    object_type IN ('PROCEDURE', 'FUNCTION', 'PACKAGE')
ORDER BY 
    object_name;

-- Triggers
SELECT 
    trigger_name,
    triggering_event AS trigger_event,
    trigger_body,
    trigger_type AS trigger_timing,
    table_name
FROM 
    all_triggers
WHERE 
    owner = USER
ORDER BY 
    trigger_name;