
public static class ErrorShared
{

              public static void Main(string[] args)
                 {

                  }
        // =========================================================
        // ACCOUNT
        // =========================================================
        public static class Account
        {
            // Business Rules
            public const int ReactivationPeriodDays = 30;

            // Password Rules
            public const int MinimumPasswordLength = 8;

            // Exception Messages
            public const string EmailRequired ="Email is required.";

            public const string PasswordRequired ="Password is required.";

            public const string InvalidEmail ="Invalid email format.";

            public const string InvalidPassword ="Password must be at least 8 characters and contain " +"uppercase, lowercase, number, and special character.";

            public const string AccountNotFound = "Account not found.";

            public const string AccountAlreadyActive ="Account is already active.";

            public const string AccountAlreadyDeleted ="Account is already deleted.";

            public const string AccountDeletionDateMissing ="Account deletion date is missing.";

            public const string ReactivationPeriodExpired ="The reactivation period has expired.";

            public const string CannotUpdateDeletedAccount ="Cannot update a deleted account.";

            public const string EmailAlreadyExists ="An account with this email already exists.";

            public const string CurrentPasswordRequired ="Current password is required.";

            public const string CurrentPasswordIncorrect ="Current password is incorrect.";

            public const string AccountReactivatedSuccessfully = "Account Reactivated Successfully";

            public const string AccountDeletedSuccessfully = "Account Deleted Successfully";

            public const string PasswordsDoNotMatch ="New password and confirm password do not match.";
            public const string TermsNotAccepted = "You must accept the terms and conditions.";
            public const string AccountUpdatedSuccessfully = "Account updated successfully.";
            public const string InvalidCredentials = "Invalid email or password.";
            public const string AccountDeactivated = "Account deactivated.";
            public const string EmailNotVerified = "Please verify your email before logging in.";
            public const string CannotAnonymizeActiveAccount = "An active account cannot be anonymized.";
            public const string CannotDeactivateProjectManager = "Cannot deactivate this account while the employee manages an active project. Assign another project manager first.";
            public const string CreateAccountThroughSignup = "Create the account through signup so it is connected to an employee.";
            public const string EmailSenderNotConfigured = "Email sender address is not configured.";
            public const string EmailServiceRejectedPrefix = "Email service rejected the message: ";
            public const string EmailVerifiedSuccessfully = "Email verified successfully.";
            public const string VerificationCodeSentSuccessfully = "Verification code sent successfully.";
            public const string PasswordResetCodeSentSuccessfully = "Password reset code sent successfully.";
            public const string ResetCodeVerifiedSuccessfully = "Password reset code verified successfully.";
            public const string AccountAnonymized = "Deleted account data was anonymized.";

        // Email Verification
            public const string VerificationCodeRequired ="Verification code is required.";

            public const string InvalidVerificationCode ="Invalid verification code.";

            public const string VerificationCodeExpired ="Verification code has expired.";

            public const string EmailAlreadyVerified ="Email is already verified.";

            public const string ResetCodeRequired ="Reset code is required.";

            public const string InvalidResetCode ="Invalid password reset code.";

            public const string ResetCodeExpired ="Password reset code has expired.";

            public const string PasswordResetSuccessful ="Password reset successfully.";





    }


        // =========================================================
        // EMPLOYEE
        // =========================================================
        public static class Employee
        {
            // Business Rules
            public const int PhoneNumberLength = 10;
            public const int ReactivationPeriodDays = 30;

            // Exception Messages
            public const string EmployeeNotFound ="Employee not found.";

            public const string FirstNameRequired ="First name is required.";

            public const string LastNameRequired ="Last name is required.";

            public const string PhoneRequired ="Phone is required.";

            public const string InvalidPhoneNumber ="Phone number must contain exactly 10 digits.";

            public const string PhoneAlreadyExists ="This phone number is already in use.";

            public const string EmployeeAlreadyDeleted ="Employee is already deleted.";

            public const string CannotUpdateDeletedEmployee ="Cannot update a deleted employee.";

            public const string EmployeeAlreadyActive ="Employee is already active.";

            public const string EmployeeDeletionDateMissing ="Deleted date is not available.";

            public const string EmployeeDeletedSuccessfully = "Employee Deleted Successfully";

            public const string EmployeeReactivatedSuccessfully = "Employee Reactivated Successfully";

            public const string EmployeeReactivationPeriodExpired ="Employee cannot be reactivated after 30 days.";

            public const string EmployeeHasActiveTickets ="Cannot delete this employee because they have active tickets. Reassign or close those tickets first.";
            public const string CannotDeleteProjectManager = "Cannot delete this employee because they manage an active project. Assign another project manager first.";
            public const string ProfileImageRequired = "Profile image is required.";
            public const string NoProfileImageUploaded = "No profile photo was uploaded.";
            public const string InvalidProfileImageType = "Profile photo must be JPG, PNG, GIF, or WEBP.";
            public const string ProfileImageNotFound = "The requested profile photo does not exist.";
            public const string CannotAnonymizeActiveEmployee = "An active employee cannot be anonymized.";
            public const string AnonymousFirstName = "Deleted";
            public const string AnonymousLastNamePrefix = "User";
            public const string NoRole = "No Role";
            public static string ProfileImageTooLarge(int maxSizeMb) => $"Profile photo must be {maxSizeMb}MB or smaller.";
        }


        // =========================================================
        // PROJECT
        // =========================================================
        public static class Project
        {
            // Exception Messages
            public const string ProjectNotFound ="Project not found.";

            public const string ProjectNameRequired ="Project name is required.";

            public const string ProjectDescriptionRequired ="Project description is required.";

            public const string EmployeeNotFound ="Employee not found.";

            public const string EmployeeAlreadyAssigned ="Employee is already assigned to this project.";

            public const string EmployeeNotAssigned ="Employee is not assigned to this project.";

            public const string ProjectManagerCannotBeRemoved ="Project manager cannot be removed from the project.";

            public const string ProjectManagerCannotBeMember ="Project manager is already part of this project and cannot be added as a regular member.";

            public const string EmployeeHasActiveTickets ="Employee has active tickets in this project. Reassign those tickets before removing the member.";

            public const string ProjectAlreadyExists ="Project already exists.";

            public const string CannotUpdateDeletedProject ="Cannot update a deleted project.";
            public const string InvalidStatus = "Invalid project status.";
            public const string OnlyManagerCanRemoveMembers = "Only the project manager can remove members.";
            public const string OnlyManagerCanUpdate = "Only the project manager can update this project.";
            public const string OnlyManagerCanDelete = "Only the project manager can delete this project.";
            public const string EmployeeIsDeleted = "Employee is deleted.";
            public const string NoEmployees = "No employees found for the specified project.";
            public const string NoProjects = "No projects found for the specified employee.";
            public const string ProjectsNotFound = "Projects were not found.";
            public const string ManagerRole = "Manager";
            public const string ManagerOrNoRole = "Manager/No Role";
        }


        // =========================================================
        // TICKET
        // =========================================================
        public static class Ticket
        {
            // Business Rules
            public const int TicketTitleMaxLength = 100;

            public const int DescriptionMaxLength = 2500;

            // Exception Messages
            public const string TicketNotFound ="Ticket not found.";

            public const string TicketTitleRequired ="Ticket title is required.";

            public const string ProjectNotFound ="The specified project does not exist.";

            public const string EmployeeNotFound ="The specified employee does not exist.";

            public const string NoTicketsForProject ="There is no ticket available for the specified project.";

            public const string NoTicketsForEmployee ="There is no ticket available for the specified employee.";

            public const string UnauthorizedTicketCreation ="Only the project manager can create tickets.";

            public const string UnauthorizedTicketReview ="Only the project manager can review this ticket.";

            public const string OnlyAssigneeCanSubmitForReview ="Only the assigned employee can submit this ticket for review.";

            public const string CommentRequired ="Comment is required.";

            public const string UnauthorizedTicketComment ="Only project members can comment on this ticket.";

            public const string InvalidPriority ="Priority must be between 1 and 3.";

            public const string EmployeeNotAssignedToProject ="Employee does not belong to this project.";
            public const string InvalidStatus = "Invalid ticket status.";
            public const string HistoryActionRequired = "History action is required.";
            public const string UnassignedDueToAccountDeactivationAction = "UnassignedDueToAccountDeactivation";
            public const string UnassignedDueToAccountDeactivationNote = "The ticket was unassigned because the employee account was deactivated.";
            public const string UnassignedEmployeeName = "Unassigned";
            public const string TicketNotFoundMessage = "Ticket was not found.";
            public const string NoFileUploaded = "No file was uploaded.";
            public const string NoFilesUploaded = "No files were uploaded.";
            public const string UploadSuccessful = "Upload successful.";
            public const string AttachmentNotFound = "The requested file does not exist.";
            public const string AttachmentUrlRequired = "Attachment URL is required.";
            public const string OriginalFileNameRequired = "Original file name is required.";
            public const string StoredFileNameRequired = "Stored file name is required.";
            public const string ContentTypeRequired = "Content type is required.";
            public const string AttachmentSizeInvalid = "Attachment size must be greater than zero.";
            public static string TooManyAttachments(int maxCount) => $"You can upload up to {maxCount} files at a time.";
            public static string AttachmentTooLarge(int maxSizeMb) => $"Each attachment must be {maxSizeMb}MB or smaller.";
        }

        public static class System
        {
            public const string ServerWorking = "Server is working!";
            public const string CleanupErrorPrefix = "Account cleanup error: ";
        }
    }
