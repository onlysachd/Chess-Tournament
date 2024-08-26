show search_path;
set search_path to chess_sch,public;


CREATE TABLE Players (
    player_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    country VARCHAR(50) NOT NULL,
    current_world_ranking INTEGER UNIQUE NOT NULL,
    total_matches_played INTEGER DEFAULT 0 NOT NULL
);


CREATE TABLE Matches (
    match_id SERIAL PRIMARY KEY ,
    player1_id INT NOT NULL,
    player2_id INT NOT NULL,
    match_date DATE NOT NULL,
    match_level VARCHAR(20) NOT NULL,
    winner_id INT,
    FOREIGN KEY (player1_id) REFERENCES Players(player_id),
    FOREIGN KEY (player2_id) REFERENCES Players(player_id),
    FOREIGN KEY (winner_id) REFERENCES Players(player_id)
);


CREATE TABLE Sponsors (
    sponsor_id SERIAL PRIMARY KEY ,
    sponsor_name VARCHAR(100) UNIQUE NOT NULL,
    industry VARCHAR(50) NOT NULL,
    contact_email VARCHAR(100) NOT NULL,
    contact_phone VARCHAR(20) NOT NULL
);  



CREATE TABLE Player_Sponsors (
    player_id INTEGER NOT NULL,
    sponsor_id INTEGER NOT NULL,
    sponsorship_amount NUMERIC(10, 2) NOT NULL,
    contract_start_date DATE NOT NULL,
    contract_end_date DATE NOT NULL,
    PRIMARY KEY (player_id, sponsor_id),
    FOREIGN KEY (player_id) REFERENCES Players(player_id),
    FOREIGN KEY (sponsor_id) REFERENCES Sponsors(sponsor_id)
);


--select * from Players; 
--select * from Matches; 
--select * from Sponsors; 
--select * from Player_Sponsors ; 



