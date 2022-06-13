namespace RescueService
{
    public class ApplicationOptions
    {
        public IEnumerable<SystemDemandOption> SystemDemandOptions { get; set; }
        public int DelayInSeconds { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
    }

    public class SystemDemandOption
    {
        public string HostOrIpAddress { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServiceName { get; set; }
        public string ServiceTitle { get; set; }
    }
}
