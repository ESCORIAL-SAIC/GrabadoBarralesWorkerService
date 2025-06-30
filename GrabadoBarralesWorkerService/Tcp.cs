using GrabadoBarralesWorkerService.Models.MySql;
using GrabadoBarralesWorkerService.Models.PgSql;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Text;

namespace GrabadoBarralesWorkerService
{
    public class Tcp(IDbContextFactory<MySqlContext> mySqlFactory, IDbContextFactory<ESCORIALContext> pgSqlFactory)
    {
        public async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                await using var mySql = await mySqlFactory.CreateDbContextAsync();
                await using var pgSql = await pgSqlFactory.CreateDbContextAsync();
                await using var stream = client.GetStream();
                while (client.Connected)
                {
                    try
                    {
                        var buffer = new byte[1024];
                        var bytesRead = await stream.ReadAsync(buffer);
                        if (bytesRead == 0)
                            break;

                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Mensaje recibido: {receivedMessage}");
                        //---------------------------------------------------------


                        var lastReg = await mySql
                            .registros
                            .OrderByDescending(x => x.fecha)
                            .FirstOrDefaultAsync(x => x.puesto.ToString() == receivedMessage);

                        if (lastReg == null)
                        {
                            Console.WriteLine("No se encontraron registros en la base de datos.");
                            continue;
                        }

                        var serial = $"B{lastReg.id:D8}";
                        var newReg = new etiquetas_maestro_barrales()
                        {
                            fecha_alta = (DateTime)lastReg.fecha!,
                            operador = lastReg.operador,
                            serie = serial,
                            tipo_barral_codigo = lastReg.tipo_barral.ToString()
                        };
                        pgSql.etiquetas_maestro_barrales.Add(newReg);
                        await pgSql.SaveChangesAsync();


                        //---------------------------------------------------------
                        var responseMessage = serial;
                        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                        await stream.WriteAsync(responseBytes);
                        Console.WriteLine("Respuesta enviada al cliente.");
                    }
                    catch (Exception ex)
                    {
                        var error = new errores()
                        {
                            fecha = DateTime.Now,
                            message = ex.Message,
                            inner_exception = ex.InnerException?.Message
                        };
                        mySql.errores.Add(error);
                        await mySql.SaveChangesAsync();
                        Console.WriteLine($"Error en procesamiento de mensaje: {ex.Message}\nInner exception: {ex.InnerException}");

                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de conexión: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Cliente desconectado.");
            }
        }
    }
}