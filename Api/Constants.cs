namespace Api;

internal static class Constants
{
    internal static class PublicIdPrefix
    {
        internal const string User = "usr_";

        internal const string Project = "prj_";

        internal const string Employee = "emp_";

        internal const string SalaryPayment = "pay_";
    }

    internal static class ErrorMessage
    {
        internal const string UserNotFound = "User not found";

        internal const string ProjectNotFound = "Project not found";

        internal const string EmployeeNotFound = "Employee not found";

        internal const string EmailAlreadyInUse = "Email already in use";

        internal const string NameAndEmailRequired = "name and email are required";

        internal const string NameRequired = "name is required";

        internal const string NameAndPositionRequired = "name and position are required";

        internal const string ProjectAlreadyArchived = "Project is already archived";

        internal const string EmployeeNotAssigned = "Employee is not assigned to any project";

        internal const string EmployeeAlreadyOnProject = "Employee is already assigned to this project";

        internal const string SalaryPaymentNotFound = "Salary payment not found";

        internal const string EmployeeIdRequired = "Employee id is required";
    }
}
