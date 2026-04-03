namespace Api;

internal static class Constants
{
    internal static class PublicIdPrefix
    {
        internal const string User = "usr_";

        internal const string Project = "prj_";

        internal const string Employee = "emp_";
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
    }
}
