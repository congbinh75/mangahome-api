START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'mangahome') THEN
        CREATE SCHEMA mangahome;
    END IF;
END $EF$;

CREATE TABLE IF NOT EXISTS mangahome.__scripts_history (
    id serial NOT NULL,
    script_name character varying(150) NOT NULL,
    executed_at timestamp with time zone NOT NULL,
    CONSTRAINT pk___scripts_history PRIMARY KEY (id)
);

DO $EF$
BEGIN
	IF EXISTS (SELECT 1 FROM mangahome.__scripts_history WHERE script_name = '001.add_users_table.sql') THEN
	    RAISE EXCEPTION 'Script already executed: 001.add_users_table.sql';
	END IF;
END $EF$;

---

CREATE TABLE mangahome.users (
    id uuid NOT NULL,
    username character varying(32) NOT NULL,
    email text NOT NULL,
    role text NOT NULL,
    password text NOT NULL,
    salt bytea NOT NULL,
    is_deleted boolean NOT NULL,
    deleted_at timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE UNIQUE INDEX ix_users_username_email ON mangahome.users (username, email);

---

INSERT INTO mangahome.__scripts_history (script_name, executed_at)
VALUES ('001.add_users_table.sql', CURRENT_TIMESTAMP);

COMMIT;
