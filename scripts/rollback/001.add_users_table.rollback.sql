START TRANSACTION;

---

DROP TABLE mangahome.users;

DELETE FROM mangahome.__scripts_history
WHERE script_name = '001.add_users_table.sql';

---

COMMIT;
