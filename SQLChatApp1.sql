CREATE TABLE Users (
    userid INT PRIMARY KEY IDENTITY,
    username NVARCHAR(50) UNIQUE,
    fullname NVARCHAR(100) UNIQUE,
    email NVARCHAR(100),
    password NVARCHAR(255), 
    status INT, 
    created_at DATETIME
);

CREATE TABLE Messages (
    messageId INT PRIMARY KEY IDENTITY,
    senderId INT,
    receiverId INT,
	groupId INT,
    content NVARCHAR(MAX),
    type INT, 
    status INT, 
    created_at DATETIME,
    CONSTRAINT FK_Sender FOREIGN KEY (senderId) REFERENCES Users(userid),
	CONSTRAINT FK_Sender FOREIGN KEY (groupId) REFERENCES Groups(groupId),
    CONSTRAINT FK_Receiver FOREIGN KEY (receiverId) REFERENCES Users(userid)
);


CREATE TABLE Groups (
    groupId INT PRIMARY KEY IDENTITY,
    name NVARCHAR(255),
    LastMessageID INT,
    created_by INT,
    created_at DATETIME,
    CONSTRAINT FK_Last_Message FOREIGN KEY (LastMessageID) REFERENCES Messages(messageId),
    CONSTRAINT FK_Created_By FOREIGN KEY (created_by) REFERENCES Users(userid)
);

CREATE TABLE GroupMembers (
    groupId INT,
    userid INT,
    joined_at DATETIME,
    left_at DATETIME,
    CONSTRAINT PK_GroupMembers PRIMARY KEY (groupId, userid),
    CONSTRAINT FK_Group_ID FOREIGN KEY (groupId) REFERENCES Groups(groupId),
    CONSTRAINT FK_User_ID FOREIGN KEY (userid) REFERENCES Users(userid)
);
