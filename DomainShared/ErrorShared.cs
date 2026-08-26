
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

            public const string ProjectAlreadyExists ="Project already exists.";

            public const string CannotUpdateDeletedProject ="Cannot update a deleted project.";
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

            public const string InvalidPriority ="Priority must be between 1 and 3.";

            public const string EmployeeNotAssignedToProject ="Employee does not belong to this project.";
        }
    }
