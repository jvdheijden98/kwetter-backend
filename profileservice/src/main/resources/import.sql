-- CREATE TABLE profiles
-- (
--     ID  INT,
--     FirstName   varChar(20),
--     LastName    varChar(20),
--     Biography   varChar(100)
-- );
INSERT INTO Profile(ID, FirstName, LastName, Biography, UserID, Username) VALUES (nextval('hibernate_sequence'), 'Victor', 'Vector', 'Ik woon samen met mijn vriendin en studeer natuurkunde.', 'username1', '123asd');
INSERT INTO Profile(ID, FirstName, LastName, Biography, UserID, Username) VALUES (nextval('hibernate_sequence'), 'Xander', 'Xylophone', 'Ik heb een prachtige animatie gemaakt', 'username2', '321dsa');