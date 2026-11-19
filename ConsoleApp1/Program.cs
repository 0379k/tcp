using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 9090;
            // Слушаем только локальные подключения (127.0.0.1)
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);

            try
            {
                listener.Start();
                Console.WriteLine($"[СЕРВЕР] Запущен на 127.0.0.1:{port}. Ожидание клиента...");

                while (true)
                {
                    // AcceptTcpClient блокирует поток, пока не подключится клиент
                    using TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"[СЕРВЕР] Подключился клиент: {client.Client.RemoteEndPoint}");

                    using NetworkStream stream = client.GetStream();

                    // Буфер для чтения данных
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        // Декодируем полученные байты в строку (UTF-8)
                        string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"[СЕРВЕР] Получено: \"{receivedText}\"");

                        // Переводим в верхний регистр
                        string responseText = receivedText.ToUpperInvariant();
                        Console.WriteLine($"[СЕРВЕР] Отправляем: \"{responseText}\"");

                        // Кодируем обратно в байты и отправляем
                        byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }

                    // Блок using автоматически закроет stream и client
                    Console.WriteLine("[СЕРВЕР] Клиент отключен. Ждем следующего...\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}