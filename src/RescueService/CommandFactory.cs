namespace RescueService
{
    public static class CommandFactory
    {
        public static string SystemCtlStatusCommand(string serviceName) => $"sudo systemctl show -p ActiveState {serviceName} | sed 's/ActiveState=//g'";
        public static string SystemCtlRestartCommand(string serviceName) => $"sudo systemctl restart {serviceName}";
    }
}
