using GrabadoBarralesWorkerService.Models.MySql;
using GrabadoBarralesWorkerService.Models.PgSql;
using System.Net;
using System.Net.Sockets;

namespace GrabadoBarralesWorkerService
{
    public class Worker(ILogger<Worker> logger, Tcp tcp) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private TcpListener? _server;
        private readonly Tcp _tcp = tcp;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                const int port = 5000;
                _server = new TcpListener(IPAddress.Any, port); 
                _server.Start();
                _logger.LogInformation("TCP server listening on port {Port}", port);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var client = await _server.AcceptTcpClientAsync(stoppingToken);
                        _logger.LogInformation("Client connected.");
                        _ = Task.Run(() => _tcp.HandleClientAsync(client), stoppingToken);
                    }
                    catch (OperationCanceledException exC)
                    {
                        _logger.LogInformation(exC, "Client connection cancelled (server stopping).");
                    }
                    catch (Exception clientEx)
                    {
                        _logger.LogError(clientEx, "Error handling client: {Message}", clientEx.Message);
                    }
                }
            }
            catch (OperationCanceledException exC)
            {
                _logger.LogInformation(exC, "Server stopped.");
                Environment.Exit(exitCode: 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(exitCode: 1);
            }
            finally
            {
                _server?.Stop();
                _logger.LogInformation("TCP server stopped.");
            }
        }
    }
}