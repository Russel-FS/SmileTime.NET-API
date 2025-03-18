-- Tabla de Usuarios
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Avatar NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastActive DATETIME,
    IsActive BIT DEFAULT 1
);

-- Tabla de Roles
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255)
);

-- Tabla de Usuario-Roles (Relación muchos a muchos)
CREATE TABLE UserRoles (
    UserId INT,
    RoleId INT,
    AssignedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    PRIMARY KEY (UserId, RoleId)
);

-- Tabla de Conversaciones
CREATE TABLE Conversations (
    ConversationId INT PRIMARY KEY IDENTITY(1,1),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    Type NVARCHAR(20) CHECK (Type IN ('individual', 'group')),
    Title NVARCHAR(255),
    IsActive BIT DEFAULT 1
);

-- Tabla de Participantes de Conversación
CREATE TABLE ConversationParticipants (
    ConversationId INT,
    UserId INT,
    JoinedAt DATETIME DEFAULT GETDATE(),
    LeftAt DATETIME NULL,
    LastRead DATETIME,
    IsAdmin BIT DEFAULT 0,
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    PRIMARY KEY (ConversationId, UserId)
);

-- Tabla de Mensajes
CREATE TABLE Messages (
    MessageId INT PRIMARY KEY IDENTITY(1,1),
    ConversationId INT,
    SenderId INT,
    Content NVARCHAR(MAX),
    MessageType NVARCHAR(20) CHECK (MessageType IN ('text', 'image', 'file', 'system')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    ModifiedAt DATETIME,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
    FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);

-- Tabla de Estado de Mensajes
CREATE TABLE MessageStatus (
    MessageId INT,
    UserId INT,
    Status NVARCHAR(20) CHECK (Status IN ('delivered', 'read')),
    StatusTimestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MessageId) REFERENCES Messages(MessageId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    PRIMARY KEY (MessageId, UserId)
);

-- Tabla de Archivos Adjuntos
CREATE TABLE Attachments (
    AttachmentId INT PRIMARY KEY IDENTITY(1,1),
    MessageId INT,
    FileUrl NVARCHAR(MAX),
    FileName NVARCHAR(255),
    FileType NVARCHAR(50),
    FileSize BIGINT,
    UploadedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MessageId) REFERENCES Messages(MessageId)
);

-- Tabla de Estado de Typing
CREATE TABLE TypingStatus (
    UserId INT,
    ConversationId INT,
    IsTyping BIT,
    LastTypingAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
    PRIMARY KEY (UserId, ConversationId)
);