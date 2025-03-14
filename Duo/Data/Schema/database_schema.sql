create table Users (			<-- Users table not User -->
	userID int primary key identity(1, 1),
	username varchar(30) unique
)

CREATE TABLE Hashtags (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Tag NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO Categories (Id, Name) VALUES
(1, 'General-Discussion'),
(2, 'Lesson-Help'),
(3, 'Off-topic'),
(4, 'Discover'),
(5, 'Announcements');


CREATE TABLE Posts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(20) NOT NULL,
    Description NVARCHAR(4000) NOT NULL,		<-- We need to modify in the diagram(can t put 5000) -->
    UserID INT NOT NULL,
    CategoryID INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    LikeCount INT DEFAULT 0,
    
    CONSTRAINT fk_user FOREIGN KEY (userID) REFERENCES Users(userID) ON DELETE CASCADE,
    CONSTRAINT fk_category FOREIGN KEY (CategoryID) REFERENCES Categories(Id) ON DELETE CASCADE
);

<-- Need to add a many to many table -->
CREATE TABLE PostHashtags (
    PostID INT NOT NULL,
    HashtagID INT NOT NULL,

    PRIMARY KEY (PostID, HashtagID),

    CONSTRAINT fk_post FOREIGN KEY (PostID) REFERENCES Posts(Id) ON DELETE CASCADE,
    CONSTRAINT fk_hashtag FOREIGN KEY (HashtagID) REFERENCES Hashtags(Id) ON DELETE CASCADE
);

<-- Look for delete and update -->
CREATE TABLE Comments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Content NVARCHAR(1000) NOT NULL,
    UserID INT NOT NULL,
    PostID INT NOT NULL,
    ParentCommentID INT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LikeCount INT DEFAULT 0,
    Level INT CHECK (Level BETWEEN 1 AND 3),

    CONSTRAINT fk_userid FOREIGN KEY (UserID) REFERENCES Users(userID) on delete no action,
    CONSTRAINT fk_posts FOREIGN KEY (PostID) REFERENCES Posts(Id) on delete no action,
    CONSTRAINT fk_parent_comment FOREIGN KEY (ParentCommentID) REFERENCES Comments(Id) on delete no action
);


<-- Insert section -->

insert into Users values ('Andrei') , ('Ion'), ('Maria')


insert into Hashtags values ('a')
insert into Hashtags values ('Mac')


INSERT INTO Posts (Title, Description, UserID, CategoryID) VALUES
('New Gadget', 'A review of the latest tech gadget.', 1, 1),
('Paris Trip', 'My amazing trip to Paris and the Eiffel Tower.', 2, 2),
('Delicious Pizza', 'Tried a new pizza place, it was fantastic!', 3, 3),
('Coding Tips', 'Some useful tips for beginner programmers.', 1, 1),
('Mountain Hike', 'Hiking in the beautiful mountains.', 2, 2),
('Homemade Pasta', 'Making fresh pasta at home.', 3, 3),
('AI Advancements', 'The latest developments in artificial intelligence.',1,1),
('Tokyo Adventures', 'Exploring the streets of Tokyo.',2,2),
('Chocolate Cake','A delicious chocolate cake recipe.',3,3);


INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Great post!', 1, 1, NULL, 1)
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('I agree!', 2, 1, 1, 2)

INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('Thanks for sharing!', 3, 2, NULL, 1)
INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level) VALUES
('This is helpful.',1,3,NULL,1); 