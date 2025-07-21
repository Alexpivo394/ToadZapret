using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ToadZapret.Services;

public class ZapretService : IZapretService
{
    private Process? zapretProcess;
    private bool isStarted = false;

    public void Start()
    {
        string appDir = AppDomain.CurrentDomain.BaseDirectory;
        string zapretDir = Path.Combine(appDir, "zapret");
        string winwsPath = Path.Combine(zapretDir, "winws.exe");

        if (!File.Exists(winwsPath))
        {
            MessageBox.Show($"Ошибка: файл {winwsPath} не найден!");
            return;
        }

        zapretProcess = new Process();
        zapretProcess.StartInfo.FileName = winwsPath;
        zapretProcess.StartInfo.WorkingDirectory = zapretDir; // Важно для доступа к спискам и бинарникам
        zapretProcess.StartInfo.UseShellExecute = false;
        zapretProcess.StartInfo.CreateNoWindow = true;
        zapretProcess.StartInfo.Arguments = "--wf-tcp=443-65535 --wf-udp=443-65535 " +
                                            "--filter-udp=443 --hostlist=\"list-youtube.txt\" --dpi-desync=fake --dpi-desync-udplen-increment=10 --dpi-desync-repeats=6 --dpi-desync-udplen-pattern=0xDEADBEEF --dpi-desync-fake-quic=\"quic_initial_www_google_com.bin\" --new " +
                                            "--filter-udp=50000-65535 --dpi-desync=fake,tamper --dpi-desync-any-protocol --dpi-desync-fake-quic=\"quic_initial_www_google_com.bin\" --new " +
                                            "--filter-tcp=443 --hostlist=\"list-youtube.txt\" --dpi-desync=fake,split2 --dpi-desync-autottl=2 --dpi-desync-fooling=md5sig --dpi-desync-fake-tls=\"tls_clienthello_www_google_com.bin\"";
        zapretProcess.Start();
        isStarted = true;
    }

    public void Stop()
    {
        if (!isStarted || zapretProcess == null)
            return;
            
        try
        {
            if (!zapretProcess.HasExited)
            {
                zapretProcess.Kill();
                zapretProcess.WaitForExit(3000); // Ждем до 3 секунд для корректного завершения
            }
        }
        catch (InvalidOperationException)
        {
            // Процесс уже завершен или не был запущен
        }
        catch (Exception)
        {
            // Игнорируем другие ошибки при завершении процесса
        }
        finally
        {
            isStarted = false;
        }
    }
    
    public void Dispose()
    {
        Stop();
        try
        {
            zapretProcess?.Dispose();
        }
        catch (Exception)
        {
            // Игнорируем ошибки при освобождении ресурсов
        }
        zapretProcess = null;
    }
}