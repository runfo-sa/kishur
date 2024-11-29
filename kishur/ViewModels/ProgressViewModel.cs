using CommunityToolkit.Mvvm.ComponentModel;
using kishur.Models;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kishur.ViewModels;

public partial class ProgressViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _start;

    [ObservableProperty]
    private int _end;

    [ObservableProperty]
    private int _current = 0;

    private Kishur _kishur;

    public ProgressViewModel() { }

    public ProgressViewModel(Kishur kishur)
    {
        Start = kishur.Start;
        End = kishur.End;
        _kishur = kishur;

        new Thread(new ThreadStart(StartPrinting)).Start();
    }

    private void StartPrinting()
    {
        for (int i = End; i >= Start; i--)
        {
            Current = i;
            Print().Wait();
        }
    }

    private async Task Print()
    {
        using Socket clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.NoDelay = true;

        IPAddress ip = IPAddress.Parse(_kishur.PrinterIp);
        IPEndPoint ipep = new(ip, 9100);
        await clientSocket.ConnectAsync(ipep);

        var content = await File.ReadAllTextAsync("./label.zpl");
        content = content.Replace("[@Day;Spanish@]", _kishur.Day.Spanish);
        content = content.Replace("[@Day;Hebrew@]", _kishur.Day.HexHebrew);
        content = content.Replace("[@Current@]", Current.ToString("D3"));

        byte[] fileBytes = Encoding.ASCII.GetBytes(content);
        await clientSocket.SendAsync(fileBytes);
    }
}