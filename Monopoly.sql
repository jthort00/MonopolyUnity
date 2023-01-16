DROP DATABASE IF EXISTS Monopoly;
CREATE DATABASE Monopoly;
USE Monopoly;

CREATE TABLE Games (
 gameID INT,
 status VARCHAR(50),
 player1 VARCHAR(50),
 player2 VARCHAR(50),
 player3 VARCHAR(50),
 player4 VARCHAR(50),
 player5 VARCHAR(50),
 Player6 VARCHAR(50),
 PRIMARY KEY (gameID)
)ENGINE=InnoDB;

CREATE TABLE Credentials (
 playerID VARCHAR(50),
 pass VARCHAR(50),
 age INT,
 PRIMARY KEY (playerID)
)ENGINE=InnoDB;

CREATE TABLE Players (
 gameID INT NOT NULL,
 playerID VARCHAR(50),
 PRIMARY KEY (gameID,playerID),
 FOREIGN KEY (gameID) REFERENCES Games(gameID) ON DELETE CASCADE ON UPDATE CASCADE,
 FOREIGN KEY (playerID) REFERENCES Credentials(playerID)
)ENGINE=InnoDB;

CREATE TABLE PlayerStatus (
 gameID INT NOT NULL,
 playerID VARCHAR(50),
 money INT,
 properties INT,
 specialcard INT,
 jail INT,
 position INT,
 PRIMARY KEY (gameID,playerID),
 FOREIGN KEY (gameID) REFERENCES Games(gameID) ON DELETE CASCADE ON UPDATE CASCADE,
 FOREIGN KEY (playerID) REFERENCES Credentials(playerID)
)ENGINE=InnoDB;

CREATE TABLE Properties (
 propertyID INT NOT NULL,
 location INT NOT NULL,
 name VARCHAR(50),
 colour VARCHAR(50),
 price INT, 
 basic_fee INT,
 building_fee INT,
 PRIMARY KEY (propertyID)
)ENGINE=InnoDB;

CREATE TABLE PropertyAssign (
 gameID INT,
 playerID VARCHAR(50),
 propertyID INT,
 constructions INT,
 mortgaged INT,
 FOREIGN KEY (gameID) REFERENCES Games(gameID),
 FOREIGN KEY (playerID) REFERENCES Credentials(playerID),
 FOREIGN KEY (propertyID) REFERENCES Properties(propertyID)
)ENGINE=InnoDB;



/*Add all the properties*/
INSERT INTO Properties VALUES (0, 1, 'Old Kent Road', 'Brown', 60, 2, 50);
INSERT INTO Properties VALUES (1, 3, 'Whitechapel Road', 'Brown', 60, 4, 50);
INSERT INTO Properties VALUES (2, 5, 'Kings Cross Station', 'Train', 200, 25, -1);
INSERT INTO Properties VALUES (3, 6, 'The Angel, Islington', 'Lblue', 100, 6, 50);
INSERT INTO Properties VALUES (4, 8, 'Euston Road', 'Lblue', 100, 6, 50);
INSERT INTO Properties VALUES (5, 9, 'Pentonville Road', 'Lblue', 120, 8, 50);
INSERT INTO Properties VALUES (6, 11, 'Pall Mall', 'Purple', 140, 10, 100);
INSERT INTO Properties VALUES (7, 12, 'Electric Company', 'Services', 150, 4, -1);
INSERT INTO Properties VALUES (8, 13, 'Whitehall', 'Purple', 140, 10, 100);
INSERT INTO Properties VALUES (9, 14, 'Northumrld Avenue', 'Purple', 160, 12, 100);
INSERT INTO Properties VALUES (10, 15, 'Marylebone Station', 'Train', 200, 25, -1);
INSERT INTO Properties VALUES (11, 16, 'Bow Street', 'Orange', 180, 14, 100);
INSERT INTO Properties VALUES (12, 18, 'Marlborough Street', 'Orange', 180, 14, 100);
INSERT INTO Properties VALUES (13, 19, 'Vine Street', 'Orange', 200, 16, 100);
INSERT INTO Properties VALUES (14, 21, 'Strand', 'Red', 220, 18, 150);
INSERT INTO Properties VALUES (15, 23, 'Fleet Street', 'Red', 220, 18, 150);
INSERT INTO Properties VALUES (16, 24, 'Trafalgar Square', 'Red', 240, 20, 150);
INSERT INTO Properties VALUES (17, 25, 'Fenchurch St. Station', 'Train', 200, 25, -1);
INSERT INTO Properties VALUES (18, 26, 'Leicester Square', 'Yellow', 260, 22, 150);
INSERT INTO Properties VALUES (19, 27, 'Coventry Street', 'Yellow', 260, 22, 150);
INSERT INTO Properties VALUES (20, 28, 'Water Works', 'Services', 150, 4, -1);
INSERT INTO Properties VALUES (21, 29, 'Piccadilly', 'Yellow', 280, 24, 150);
INSERT INTO Properties VALUES (22, 31, 'Regent Street', 'Green', 300, 26, 200);
INSERT INTO Properties VALUES (23, 32, 'Oxford Street', 'Green', 300, 26, 200);
INSERT INTO Properties VALUES (24, 34, 'Bond Street', 'Green', 320, 28, 200);
INSERT INTO Properties VALUES (25, 35, 'Liverpool St. Station', 'Train', 200, 25, -1);
INSERT INTO Properties VALUES (26, 37, 'Park Lane', 'Dblue', 350, 35, 200);
INSERT INTO Properties VALUES (27, 39, 'Mayfair', 'Dblue', 400, 50, 200);
  
 


/*Games
INSERT INTO Games VALUES (1,'Playing');
INSERT INTO Games VALUES (2,'Playing');
INSERT INTO Games VALUES (3,'Playing');

/*Players and games that are playing
INSERT INTO Players VALUES (1,'player1');
INSERT INTO Players VALUES (2,'player1');
INSERT INTO Players VALUES (1,'player2');
INSERT INTO Players VALUES (1,'player3');
INSERT INTO Players VALUES (1,'player4');
INSERT INTO Players VALUES (2,'player4');
INSERT INTO Players VALUES (2,'player5');

/*Status of each player in each game
INSERT INTO PlayerStatus VALUES (1,'player1',500,'temporary',0,0,20);
INSERT INTO PlayerStatus VALUES (2,'player1',1200, 'temporary',1,1,2);
INSERT INTO PlayerStatus VALUES (1,'player2',100, 'temporary', 0,0,24);
INSERT INTO PlayerStatus VALUES (1,'player3',2000, 'temporary', 0, 1,4);
INSERT INTO PlayerStatus VALUES (1,'player4',4000, 'temporary', 1, 0,12);
INSERT INTO PlayerStatus VALUES (2,'player4',3250, 'temporary', 0, 0,18);
INSERT INTO PlayerStatus VALUES (2,'player5',35, 'temporary', 0, 1,15);




/* Dame los ID de los jugadores y de la partida donde estan con más de 100 euros que estan en prisión */

/*SELECT Games.gameID, PlayerStatus.playerID FROM Games, PlayerStatus WHERE Games.gameID = PlayerStatus.gameID AND PlayerStatus.jail = 1 AND PlayerStatus.money > 100;*\


