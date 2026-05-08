CREATE TABLE clients (
    client_id SERIAL PRIMARY KEY,
    full_name TEXT,
    birth_date DATE,
    city TEXT,
    updated_at TIMESTAMP DEFAULT now()
);

INSERT INTO clients(full_name, birth_date, city)
VALUES
('John Smith', '1990-05-10', 'Berlin'),
('user1', '1990-05-10', 'Berlin'),
('user2', '1990-05-10', 'Berlin'),
('user3', '1990-05-11', 'Berlin'),
('user4', '1990-05-10', 'Berlin'),
('user5', '1990-05-10', 'Berlin'),
('Anna White', '1985-11-23', 'Paris');

