using Renci.SshNet;
using System.Net.Sockets;
using System.Text;

namespace RescueService
{
    public class InfraService
    {
        private ILogger<InfraService> _logger;

        public InfraService(ILogger<InfraService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> IsPortAvailableAsync(string hostOrIPAddress, int port, TimeSpan timeOut, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var cancellationTokenSource = new CancellationTokenSource(timeOut);
                    var linkedinCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);
                    await client.ConnectAsync(hostOrIPAddress, port, linkedinCancellationTokenSource.Token);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while checking port {port} on {hostOrIPAddress}.", ex);
                throw;
            }
        }

        public async Task<string> ExecuteCommandAsync(string hostOrIPAddress, int port, string username, string password, string commandString, TimeSpan timeOut, CancellationToken cancellationToken)
        {
            StringBuilder output = new StringBuilder();

            try
            {
                var sshConnectionInfo = new ConnectionInfo(hostOrIPAddress, port, username, new AuthenticationMethod[]{ new PasswordAuthenticationMethod(username, password) });

                using (var sshClient = new SshClient(sshConnectionInfo))
                {
                    sshClient.Connect();

                    var outputs = new Progress<ScriptOutputLine>((stream) => { output.Append(stream.Line); });

                    using (var command = sshClient.RunCommand(commandString))
                    {
                        var cancellationTokenSource = new CancellationTokenSource(timeOut);
                        var linkedinCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);
                        await command.ExecuteAsync(outputs, linkedinCancellationTokenSource.Token);

                        if (command.ExitStatus != 0)
                            _logger.LogWarning($"Command {commandString} exited abnormally.");
                    }

                    sshClient.Disconnect();
                }

                return output.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while executing a command remote command {commandString} on {hostOrIPAddress}.", ex);
                throw;
            }
        }
    }
}
