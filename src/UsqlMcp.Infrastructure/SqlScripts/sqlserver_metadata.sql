-- SQL Server Metadata Extraction Queries

-- Tables Metadata
SELECT 
    t.name AS table_name,
    CASE WHEN t.type = 'U' THEN 'TABLE' ELSE 'VIEW' END AS table_type,
    CAST(ep.value AS NVARCHAR(MAX)) AS table_description
FROM 
    sys.tables t
LEFT JOIN 
    sys.extended_properties ep ON ep.major_id = t.object_id AND ep.minor_id = 0 AND ep.name = 'MS_Description'
ORDER BY 
    t.name;

-- Columns Metadata
SELECT 
    t.name AS table_name,
    c.name AS column_name,
    ty.name AS data_type,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable,
    CAST(ep.value AS NVARCHAR(MAX)) AS column_description,
    CASE WHEN dc.definition IS NOT NULL THEN dc.definition ELSE NULL END AS column_default
FROM 
    sys.tables t
JOIN 
    sys.columns c ON t.object_id = c.object_id
JOIN 
    sys.types ty ON c.user_type_id = ty.user_type_id
LEFT JOIN 
    sys.extended_properties ep ON ep.major_id = t.object_id AND ep.minor_id = c.column_id AND ep.name = 'MS_Description'
LEFT JOIN 
    sys.default_constraints dc ON dc.parent_object_id = t.object_id AND dc.parent_column_id = c.column_id
ORDER BY 
    t.name, c.column_id;

-- Primary Keys
SELECT 
    t.name AS table_name,
    c.name AS column_name
FROM 
    sys.tables t
JOIN 
    sys.indexes i ON t.object_id = i.object_id
JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN 
    sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE 
    i.is_primary_key = 1
ORDER BY 
    t.name, ic.key_ordinal;

-- Foreign Keys
SELECT 
    pt.name AS table_name,
    pc.name AS column_name,
    rt.name AS foreign_table_name,
    rc.name AS foreign_column_name
FROM 
    sys.foreign_keys fk
JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
JOIN 
    sys.tables pt ON fk.parent_object_id = pt.object_id
JOIN 
    sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
JOIN 
    sys.tables rt ON fk.referenced_object_id = rt.object_id
JOIN 
    sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
ORDER BY 
    pt.name, pc.name;

-- Indexes
SELECT 
    t.name AS table_name,
    i.name AS index_name,
    c.name AS column_name,
    i.is_unique
FROM 
    sys.tables t
JOIN 
    sys.indexes i ON t.object_id = i.object_id
JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN 
    sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE 
    i.is_primary_key = 0 AND i.is_unique_constraint = 0
ORDER BY 
    t.name, i.name, ic.key_ordinal;

-- Views
SELECT 
    v.name AS view_name,
    OBJECT_DEFINITION(v.object_id) AS view_definition
FROM 
    sys.views v
ORDER BY 
    v.name;

-- Stored Procedures
SELECT 
    p.name AS procedure_name,
    OBJECT_DEFINITION(p.object_id) AS definition
FROM 
    sys.procedures p
ORDER BY 
    p.name;

-- Functions
SELECT 
    f.name AS function_name,
    OBJECT_DEFINITION(f.object_id) AS definition
FROM 
    sys.objects f
WHERE 
    f.type IN ('FN', 'IF', 'TF')
ORDER BY 
    f.name;

-- Triggers
SELECT 
    tr.name AS trigger_name,
    OBJECT_NAME(tr.parent_id) AS table_name,
    OBJECT_DEFINITION(tr.object_id) AS trigger_definition,
    CASE WHEN tr.is_instead_of_trigger = 1 THEN 'INSTEAD OF' ELSE 'AFTER' END AS trigger_timing,
    CASE 
        WHEN OBJECTPROPERTY(tr.object_id, 'ExecIsInsertTrigger') = 1 THEN 'INSERT'
        WHEN OBJECTPROPERTY(tr.object_id, 'ExecIsUpdateTrigger') = 1 THEN 'UPDATE'
        WHEN OBJECTPROPERTY(tr.object_id, 'ExecIsDeleteTrigger') = 1 THEN 'DELETE'
    END AS trigger_event
FROM 
    sys.triggers tr
ORDER BY 
    OBJECT_NAME(tr.parent_id), tr.name;