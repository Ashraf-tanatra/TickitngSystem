using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAllIdsToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE #EmployeeIdMap (OldId int NOT NULL PRIMARY KEY, NewId uniqueidentifier NOT NULL UNIQUE);
                CREATE TABLE #AccountIdMap (OldId int NOT NULL PRIMARY KEY, NewId uniqueidentifier NOT NULL UNIQUE);
                CREATE TABLE #ProjectIdMap (OldId int NOT NULL PRIMARY KEY, NewId uniqueidentifier NOT NULL UNIQUE);
                CREATE TABLE #TicketIdMap (OldId int NOT NULL PRIMARY KEY, NewId uniqueidentifier NOT NULL UNIQUE);
                CREATE TABLE #AttachmentIdMap (OldId int NOT NULL PRIMARY KEY, NewId uniqueidentifier NOT NULL UNIQUE);
                CREATE TABLE #HistoryIdMap (OldId int NOT NULL PRIMARY KEY, NewId uniqueidentifier NOT NULL UNIQUE);

                INSERT INTO #EmployeeIdMap SELECT Id, NEWID() FROM Employees;
                INSERT INTO #AccountIdMap SELECT Id, NEWID() FROM Accounts;
                INSERT INTO #ProjectIdMap SELECT Id, NEWID() FROM Projects;
                INSERT INTO #TicketIdMap SELECT TicketId, NEWID() FROM Tickets;
                INSERT INTO #AttachmentIdMap SELECT Id, NEWID() FROM Attachments;
                INSERT INTO #HistoryIdMap SELECT Id, NEWID() FROM TicketHistories;

                CREATE TABLE Employees_Guid (
                    Id uniqueidentifier NOT NULL,
                    FName nvarchar(50) NOT NULL,
                    LName nvarchar(50) NOT NULL,
                    Phone nvarchar(10) NOT NULL,
                    Gender char(1) NOT NULL,
                    DeletedAt date NULL,
                    IsDeleted bit NOT NULL,
                    CreatedAt datetime2 NOT NULL,
                    UpdatedAt datetime2 NULL,
                    ProfileImageUrl varchar(500) NULL
                );

                CREATE TABLE Accounts_Guid (
                    Id uniqueidentifier NOT NULL,
                    Email varchar(255) NOT NULL,
                    PasswordHash varchar(500) NOT NULL,
                    EmployeeId uniqueidentifier NOT NULL,
                    IsDeleted bit NOT NULL,
                    CreatedAt datetime2 NOT NULL,
                    UpdatedAt datetime2 NULL,
                    DeletedAt datetime2 NULL,
                    VerificationCode varchar(6) NULL,
                    VerificationCodeExpiresAt datetime NULL,
                    IsEmailVerified bit NOT NULL,
                    PasswordResetCode varchar(6) NULL,
                    PasswordResetCodeExpiresAt datetime NULL,
                    IsAnonymized bit NOT NULL
                );

                CREATE TABLE Projects_Guid (
                    Id uniqueidentifier NOT NULL,
                    ProjectName varchar(125) NOT NULL,
                    ProjectDescription varchar(255) NULL,
                    ProjectStatus nvarchar(max) NOT NULL,
                    StartedAt date NULL,
                    EndAt date NULL,
                    ProjectManagerId uniqueidentifier NOT NULL,
                    CreatedAt datetime2 NOT NULL,
                    UpdatedAt datetime2 NULL
                );

                CREATE TABLE ProjectEmployees_Guid (
                    ProjectId uniqueidentifier NOT NULL,
                    EmployeeId uniqueidentifier NOT NULL,
                    Role nvarchar(50) NULL
                );

                CREATE TABLE Tickets_Guid (
                    TicketId uniqueidentifier NOT NULL,
                    TicketTitle varchar(255) NOT NULL,
                    Description varchar(2500) NULL,
                    DueTo date NULL,
                    TicketStatus nvarchar(max) NOT NULL,
                    Priority nvarchar(max) NOT NULL,
                    EmployeeId uniqueidentifier NULL,
                    ProjectId uniqueidentifier NOT NULL,
                    TicketCreatedById uniqueidentifier NOT NULL,
                    CreatedAt datetime2 NOT NULL,
                    UpdatedAt datetime2 NULL
                );

                CREATE TABLE Attachments_Guid (
                    Id uniqueidentifier NOT NULL,
                    URL varchar(500) NOT NULL,
                    TicketId uniqueidentifier NOT NULL,
                    CreatedAt datetime2 NOT NULL,
                    UpdatedAt datetime2 NULL,
                    OriginalFileName varchar(255) NOT NULL,
                    StoredFileName varchar(255) NOT NULL,
                    ContentType varchar(100) NOT NULL,
                    SizeInBytes bigint NOT NULL
                );

                CREATE TABLE TicketHistories_Guid (
                    Id uniqueidentifier NOT NULL,
                    TicketId uniqueidentifier NOT NULL,
                    ActionByEmployeeId uniqueidentifier NOT NULL,
                    Action nvarchar(max) NOT NULL,
                    OldValue nvarchar(max) NULL,
                    NewValue nvarchar(max) NULL,
                    ModifiedAt datetime2 NOT NULL,
                    FromEmployeeId uniqueidentifier NULL,
                    ToEmployeeId uniqueidentifier NULL,
                    CreatedAt datetime2 NOT NULL,
                    UpdatedAt datetime2 NULL,
                    Note varchar(2500) NULL
                );

                INSERT INTO Employees_Guid
                    (Id, FName, LName, Phone, Gender, DeletedAt, IsDeleted, CreatedAt, UpdatedAt, ProfileImageUrl)
                SELECT
                    map.NewId, employee.FName, employee.LName, employee.Phone, employee.Gender,
                    employee.DeletedAt, employee.IsDeleted, employee.CreatedAt, employee.UpdatedAt,
                    CASE
                        WHEN employee.ProfileImageUrl IS NULL THEN NULL
                        ELSE REPLACE(
                            employee.ProfileImageUrl,
                            '/api/Employee/' + CONVERT(varchar(20), employee.Id) + '/ProfilePhoto/',
                            '/api/Employee/' + CONVERT(varchar(36), map.NewId) + '/ProfilePhoto/')
                    END
                FROM Employees employee
                INNER JOIN #EmployeeIdMap map ON map.OldId = employee.Id;

                INSERT INTO Accounts_Guid
                    (Id, Email, PasswordHash, EmployeeId, IsDeleted, CreatedAt, UpdatedAt, DeletedAt,
                     VerificationCode, VerificationCodeExpiresAt, IsEmailVerified, PasswordResetCode,
                     PasswordResetCodeExpiresAt, IsAnonymized)
                SELECT
                    accountMap.NewId, account.Email, account.PasswordHash, employeeMap.NewId,
                    account.IsDeleted, account.CreatedAt, account.UpdatedAt, account.DeletedAt,
                    account.VerificationCode, account.VerificationCodeExpiresAt, account.IsEmailVerified,
                    account.PasswordResetCode, account.PasswordResetCodeExpiresAt, account.IsAnonymized
                FROM Accounts account
                INNER JOIN #AccountIdMap accountMap ON accountMap.OldId = account.Id
                INNER JOIN #EmployeeIdMap employeeMap ON employeeMap.OldId = account.EmployeeId;

                INSERT INTO Projects_Guid
                    (Id, ProjectName, ProjectDescription, ProjectStatus, StartedAt, EndAt,
                     ProjectManagerId, CreatedAt, UpdatedAt)
                SELECT
                    projectMap.NewId, project.ProjectName, project.ProjectDescription, project.ProjectStatus,
                    project.StartedAt, project.EndAt, managerMap.NewId, project.CreatedAt, project.UpdatedAt
                FROM Projects project
                INNER JOIN #ProjectIdMap projectMap ON projectMap.OldId = project.Id
                INNER JOIN #EmployeeIdMap managerMap ON managerMap.OldId = project.ProjectManagerId;

                INSERT INTO ProjectEmployees_Guid (ProjectId, EmployeeId, Role)
                SELECT projectMap.NewId, employeeMap.NewId, projectEmployee.Role
                FROM ProjectEmployees projectEmployee
                INNER JOIN #ProjectIdMap projectMap ON projectMap.OldId = projectEmployee.ProjectId
                INNER JOIN #EmployeeIdMap employeeMap ON employeeMap.OldId = projectEmployee.EmployeeId;

                INSERT INTO Tickets_Guid
                    (TicketId, TicketTitle, Description, DueTo, TicketStatus, Priority, EmployeeId,
                     ProjectId, TicketCreatedById, CreatedAt, UpdatedAt)
                SELECT
                    ticketMap.NewId, ticket.TicketTitle, ticket.Description, ticket.DueTo,
                    ticket.TicketStatus, ticket.Priority, assignedMap.NewId, projectMap.NewId,
                    creatorMap.NewId, ticket.CreatedAt, ticket.UpdatedAt
                FROM Tickets ticket
                INNER JOIN #TicketIdMap ticketMap ON ticketMap.OldId = ticket.TicketId
                INNER JOIN #ProjectIdMap projectMap ON projectMap.OldId = ticket.ProjectId
                INNER JOIN #EmployeeIdMap creatorMap ON creatorMap.OldId = ticket.TicketCreatedById
                LEFT JOIN #EmployeeIdMap assignedMap ON assignedMap.OldId = ticket.EmployeeId;

                INSERT INTO Attachments_Guid
                    (Id, URL, TicketId, CreatedAt, UpdatedAt, OriginalFileName, StoredFileName, ContentType, SizeInBytes)
                SELECT
                    attachmentMap.NewId, attachment.URL, ticketMap.NewId, attachment.CreatedAt,
                    attachment.UpdatedAt, attachment.OriginalFileName, attachment.StoredFileName,
                    attachment.ContentType, attachment.SizeInBytes
                FROM Attachments attachment
                INNER JOIN #AttachmentIdMap attachmentMap ON attachmentMap.OldId = attachment.Id
                INNER JOIN #TicketIdMap ticketMap ON ticketMap.OldId = attachment.TicketId;

                INSERT INTO TicketHistories_Guid
                    (Id, TicketId, ActionByEmployeeId, Action, OldValue, NewValue, ModifiedAt,
                     FromEmployeeId, ToEmployeeId, CreatedAt, UpdatedAt, Note)
                SELECT
                    historyMap.NewId, ticketMap.NewId, actionByMap.NewId, history.Action,
                    history.OldValue, history.NewValue, history.ModifiedAt, fromMap.NewId,
                    toMap.NewId, history.CreatedAt, history.UpdatedAt, history.Note
                FROM TicketHistories history
                INNER JOIN #HistoryIdMap historyMap ON historyMap.OldId = history.Id
                INNER JOIN #TicketIdMap ticketMap ON ticketMap.OldId = history.TicketId
                INNER JOIN #EmployeeIdMap actionByMap ON actionByMap.OldId = history.ActionByEmployeeId
                LEFT JOIN #EmployeeIdMap fromMap ON fromMap.OldId = history.FromEmployeeId
                LEFT JOIN #EmployeeIdMap toMap ON toMap.OldId = history.ToEmployeeId;

                DROP TABLE Attachments;
                DROP TABLE TicketHistories;
                DROP TABLE Tickets;
                DROP TABLE ProjectEmployees;
                DROP TABLE Accounts;
                DROP TABLE Projects;
                DROP TABLE Employees;

                EXEC sp_rename 'Employees_Guid', 'Employees';
                EXEC sp_rename 'Accounts_Guid', 'Accounts';
                EXEC sp_rename 'Projects_Guid', 'Projects';
                EXEC sp_rename 'ProjectEmployees_Guid', 'ProjectEmployees';
                EXEC sp_rename 'Tickets_Guid', 'Tickets';
                EXEC sp_rename 'Attachments_Guid', 'Attachments';
                EXEC sp_rename 'TicketHistories_Guid', 'TicketHistories';

                ALTER TABLE Employees ADD CONSTRAINT PK_Employees PRIMARY KEY (Id);
                ALTER TABLE Accounts ADD CONSTRAINT PK_Accounts PRIMARY KEY (Id);
                ALTER TABLE Projects ADD CONSTRAINT PK_Projects PRIMARY KEY (Id);
                ALTER TABLE ProjectEmployees ADD CONSTRAINT PK_ProjectEmployees PRIMARY KEY (ProjectId, EmployeeId);
                ALTER TABLE Tickets ADD CONSTRAINT PK_Tickets PRIMARY KEY (TicketId);
                ALTER TABLE Attachments ADD CONSTRAINT PK_Attachments PRIMARY KEY (Id);
                ALTER TABLE TicketHistories ADD CONSTRAINT PK_TicketHistories PRIMARY KEY (Id);

                CREATE UNIQUE INDEX IX_Accounts_Email ON Accounts (Email);
                CREATE UNIQUE INDEX IX_Accounts_EmployeeId ON Accounts (EmployeeId);
                CREATE INDEX IX_Projects_ProjectManagerId ON Projects (ProjectManagerId);
                CREATE INDEX IX_ProjectEmployees_EmployeeId ON ProjectEmployees (EmployeeId);
                CREATE INDEX IX_Tickets_EmployeeId ON Tickets (EmployeeId);
                CREATE INDEX IX_Tickets_ProjectId ON Tickets (ProjectId);
                CREATE INDEX IX_Tickets_TicketCreatedById ON Tickets (TicketCreatedById);
                CREATE INDEX IX_Attachments_TicketId ON Attachments (TicketId);
                CREATE INDEX IX_TicketHistories_ActionByEmployeeId ON TicketHistories (ActionByEmployeeId);
                CREATE INDEX IX_TicketHistories_FromEmployeeId ON TicketHistories (FromEmployeeId);
                CREATE INDEX IX_TicketHistories_TicketId ON TicketHistories (TicketId);
                CREATE INDEX IX_TicketHistories_ToEmployeeId ON TicketHistories (ToEmployeeId);

                ALTER TABLE Accounts ADD CONSTRAINT FK_Accounts_Employees_EmployeeId
                    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id) ON DELETE CASCADE;
                ALTER TABLE Projects ADD CONSTRAINT FK_Projects_Employees_ProjectManagerId
                    FOREIGN KEY (ProjectManagerId) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE ProjectEmployees ADD CONSTRAINT FK_ProjectEmployees_Employees_EmployeeId
                    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE ProjectEmployees ADD CONSTRAINT FK_ProjectEmployees_Projects_ProjectId
                    FOREIGN KEY (ProjectId) REFERENCES Projects (Id) ON DELETE NO ACTION;
                ALTER TABLE Tickets ADD CONSTRAINT FK_Tickets_Employees_EmployeeId
                    FOREIGN KEY (EmployeeId) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE Tickets ADD CONSTRAINT FK_Tickets_Employees_TicketCreatedById
                    FOREIGN KEY (TicketCreatedById) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE Tickets ADD CONSTRAINT FK_Tickets_Projects_ProjectId
                    FOREIGN KEY (ProjectId) REFERENCES Projects (Id) ON DELETE NO ACTION;
                ALTER TABLE Attachments ADD CONSTRAINT FK_Attachments_Tickets_TicketId
                    FOREIGN KEY (TicketId) REFERENCES Tickets (TicketId) ON DELETE CASCADE;
                ALTER TABLE TicketHistories ADD CONSTRAINT FK_TicketHistories_Employees_ActionByEmployeeId
                    FOREIGN KEY (ActionByEmployeeId) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE TicketHistories ADD CONSTRAINT FK_TicketHistories_Employees_FromEmployeeId
                    FOREIGN KEY (FromEmployeeId) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE TicketHistories ADD CONSTRAINT FK_TicketHistories_Employees_ToEmployeeId
                    FOREIGN KEY (ToEmployeeId) REFERENCES Employees (Id) ON DELETE NO ACTION;
                ALTER TABLE TicketHistories ADD CONSTRAINT FK_TicketHistories_Tickets_TicketId
                    FOREIGN KEY (TicketId) REFERENCES Tickets (TicketId) ON DELETE NO ACTION;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                THROW 50000, 'ConvertAllIdsToGuid is a one-way, data-preserving migration. Restore the pre-migration database backup to roll it back.', 1;
                """);
        }
    }
}
