-- SQLite Metadata Extraction Queries

-- Tables Metadata
SELECT 
    name AS table_name,
    'TABLE' AS table_type,
    '' AS table_description
FROM 
    sqlite_master
WHERE 
    type = 'table' AND
    name NOT LIKE 'sqlite_%'
ORDER BY 
    name;

-- Columns Metadata
SELECT 
    m.name AS table_name,
    p.name AS column_name,
    p.type AS data_type,
    p."notnull" AS is_not_null,
    p.dflt_value AS column_default,
    p.pk AS is_primary_key
FROM 
    sqlite_master m
JOIN 
    pragma_table_info(m.name) p
WHERE 
    m.type = 'table' AND
    m.name NOT LIKE 'sqlite_%'
ORDER BY 
    m.name, p.cid;

-- Primary Keys
SELECT 
    m.name AS table_name,
    p.name AS column_name
FROM 
    sqlite_master m
JOIN 
    pragma_table_info(m.name) p
WHERE 
    m.type = 'table' AND
    m.name NOT LIKE 'sqlite_%' AND
    p.pk = 1
ORDER BY 
    m.name, p.cid;

-- Foreign Keys
SELECT 
    m.name AS table_name,
    p."from" AS column_name,
    p."table" AS foreign_table_name,
    p."to" AS foreign_column_name
FROM 
    sqlite_master m
JOIN 
    pragma_foreign_key_list(m.name) p
WHERE 
    m.type = 'table' AND
    m.name NOT LIKE 'sqlite_%'
ORDER BY 
    m.name, p.id;

-- Indexes
SELECT 
    m.name AS table_name,
    i.name AS index_name,
    ii.name AS column_name,
    i."unique" AS is_unique
FROM 
    sqlite_master m
JOIN 
    sqlite_master i ON m.name = i.tbl_name AND i.type = 'index'
JOIN 
    pragma_index_info(i.name) pi
JOIN 
    pragma_table_info(m.name) ii ON pi.cid = ii.cid
WHERE 
    m.type = 'table' AND
    m.name NOT LIKE 'sqlite_%' AND
    i.name NOT LIKE 'sqlite_%'
ORDER BY 
    m.name, i.name, pi.seqno;

-- Views
SELECT 
    name AS view_name,
    sql AS view_definition
FROM 
    sqlite_master
WHERE 
    type = 'view'
ORDER BY 
    name;

-- Triggers
SELECT 
    name AS trigger_name,
    tbl_name AS table_name,
    sql AS trigger_definition
FROM 
    sqlite_master
WHERE 
    type = 'trigger'
ORDER BY 
    name;