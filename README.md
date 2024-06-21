# SPOT_API

## Development Environment Preparation

### To drop all postgress table

Method 1
````
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
````

Method 2
````
DO $$ DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = current_schema()) LOOP
        EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;
````

## Adding UUID as primary key in postgres

Reference: [https://arctype.com/blog/postgres-uuid/](https://arctype.com/blog/postgres-uuid/)
````
SELECT * FROM pg_extension

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE PINK_FLOYD (
	id uuid DEFAULT uuid_generate_v4 (),
	album_name VARCHAR NOT NULL,
	release_date DATE NOT NULL,
	PRIMARY KEY (id)
);

````
## IOT Database Script

````
CREATE SEQUENCE devicedata_id_seq;

CREATE TABLE public.devicedata
(
    deviceid character varying COLLATE pg_catalog."default" NOT NULL,
    createdate timestamp without time zone NOT NULL DEFAULT now(),
    data character varying COLLATE pg_catalog."default",
    latitude character varying COLLATE pg_catalog."default",
    longitude character varying COLLATE pg_catalog."default",
    alarm character varying COLLATE pg_catalog."default",
    batt character varying COLLATE pg_catalog."default",
    mode character varying COLLATE pg_catalog."default",
    fw_ver character varying COLLATE pg_catalog."default",
    hw_ver character varying COLLATE pg_catalog."default",
    wifi_rssi character varying COLLATE pg_catalog."default",
    ble_rssi character varying COLLATE pg_catalog."default",
    date_time character varying COLLATE pg_catalog."default",
    prox_id_list character varying COLLATE pg_catalog."default",
    id integer NOT NULL DEFAULT nextval('devicedata_id_seq'::regclass),
    dashboard_status character varying COLLATE pg_catalog."default"
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

````
## Deployment Using Visual Studio

1. Please ensure that only the web api is in the project. Other project such as console (for web jobs) need to unload first)
2. Right click on the SPOT API project and select publish
3. Click publish 
