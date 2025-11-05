CREATE TABLE Assets (
    AssetId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NULL,
    TagId NVARCHAR(100) NULL,
    Description NVARCHAR(MAX) NULL,
    Condition NVARCHAR(100) NULL,
    Location NVARCHAR(255) NULL,
    PhotoPath NVARCHAR(500) NULL,
    IsVerified BIT DEFAULT 0,
    VerifiedAt DATETIME2 NULL,
    VerificationStatus NVARCHAR(50) DEFAULT 'Pending',
    CreatedAt DATETIME2 DEFAULT SYSUTCDATETIME()
);

CREATE TABLE Verifications (
    VerificationId INT IDENTITY(1,1) PRIMARY KEY,
    AssetId INT NOT NULL,
    Status NVARCHAR(50) NULL,
    Comment NVARCHAR(MAX) NULL,
    AgentName NVARCHAR(255) NULL,
    VerifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Verifications_Assets 
        FOREIGN KEY (AssetId) REFERENCES Assets(AssetId) ON DELETE CASCADE
);
