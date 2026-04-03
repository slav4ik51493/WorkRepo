namespace Api.Exceptions;

public sealed class ProjectArchivedException : BusinessException
{
    public ProjectArchivedException(string projectName)
        : base($"Project '{projectName}' is archived and cannot accept new employees.")
    {
    }
}
