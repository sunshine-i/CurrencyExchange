CREATE DATABASE CurrencyExchangeDb;
GO

USE CurrencyExchangeDb;
GO

CREATE TABLE Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Balances (
    BalanceId    INT IDENTITY(1,1) PRIMARY KEY,
    UserId       INT            NOT NULL REFERENCES Users(UserId),
    CurrencyCode NVARCHAR(10)   NOT NULL,
    Amount       FLOAT          NOT NULL DEFAULT 0,
    CONSTRAINT UQ_UserCurrency UNIQUE (UserId, CurrencyCode)
);

CREATE TABLE Transactions (
    TransactionId  INT IDENTITY(1,1) PRIMARY KEY,
    UserId         INT           NOT NULL REFERENCES Users(UserId),
    Type           NVARCHAR(10)  NOT NULL,   -- 'TOPUP' or 'EXCHANGE'
    FromCurrency   NVARCHAR(10)  NULL,
    ToCurrency     NVARCHAR(10)  NULL,
    FromAmount     FLOAT         NULL,
    ToAmount       FLOAT         NULL,
    Rate           FLOAT         NULL,
    Timestamp      DATETIME      NOT NULL DEFAULT GETDATE()
);
GO
