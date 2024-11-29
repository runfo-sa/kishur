using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kishur.Core;
using kishur.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace kishur.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public static ObservableCollection<Day> Days { get; set; }

    public Interaction<ProgressViewModel, int> ProgressDialog { get; }

    [ObservableProperty]
    private Day _selectedDay;

    [ObservableProperty]
    private int _rangeStart;

    [ObservableProperty]
    private int _rangeEnd;

    [ObservableProperty]
    private string _printerIp;

    [ObservableProperty]
    private string _invalidMessage;

    public MainViewModel()
    {
        ProgressDialog = new Interaction<ProgressViewModel, int>();
        Days = [
            new Day("Lunes", "%D7%A9%D7%A0%D7%99"),
            new Day("Martes", "%D7%A9%D7%9C%D7%99%D7%A9%D7%99"),
            new Day("Miercoles", "%D7%A8%D7%91%D7%99%D7%A2%D7%99"),
            new Day("Jueves", "%D7%97%D7%9E%D7%99%D7%A9%D7%99"),
            new Day("Viernes", "%D7%A9%D7%99%D7%A9%D7%99"),
        ];

        if (File.Exists("./cache.xml"))
        {
            XmlSerializer serializer = new(typeof(Kishur));
            using FileStream stream = new("./cache.xml", FileMode.Open);

            var kishur = (Kishur?)serializer.Deserialize(stream);
            if (kishur != null)
            {
                SelectedDay = kishur.Value.Day;
                PrinterIp = kishur.Value.PrinterIp;
                RangeStart = kishur.Value.Start;
                RangeEnd = kishur.Value.End;
            }
        }
        else
        {
            SelectedDay = Days[0];
            RangeStart = 1;
            RangeEnd = 400;
            PrinterIp = "";
        }

        InvalidMessage = "";
    }

    [RelayCommand]
    private async Task StartPrinting()
    {
        if (ValidateIPv4())
        {
            var kishur = new Kishur(RangeStart, RangeEnd, PrinterIp, SelectedDay);
            var vm = new ProgressViewModel(kishur);
            await ProgressDialog.HandleAsync(vm);

            XmlSerializer serializer = new(typeof(Kishur));
            using FileStream stream = new("./cache.xml", FileMode.Create);
            serializer.Serialize(stream, kishur);
        }
    }

    public bool ValidateIPv4()
    {
        if (string.IsNullOrWhiteSpace(PrinterIp))
        {
            return false;
        }

        string[] splitValues = PrinterIp.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        return splitValues.All(r => byte.TryParse(r, out byte tempForParsing));
    }

    partial void OnPrinterIpChanged(string value)
    {
        if (ValidateIPv4())
        {
            InvalidMessage = "";
        }
        else
        {
            InvalidMessage = "IP invalida";
        }
    }
}