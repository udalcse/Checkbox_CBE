﻿declare c integer;
begin
select count(*) into c
from all_tables
where
table_name = 'CHANGELOG';

IF c != 1 THEN
  execute immediate 'CREATE TABLE changelog(
	change_number NUMBER(11,4), 
	complete_dt DATE, 
	applied_by VARCHAR2(100), 
	description VARCHAR2(500) )';
END IF;
end;
