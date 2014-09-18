using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpPcap;
using SharpPcap.LibPcap;

namespace pendulum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Running = false;

        public delegate void PacketHandler(byte[] data, bool fromClient);

        private class DeviceOption
        {
            public string Name { get; set; }
        
            public ICaptureDevice Value { get; set; }

            public DeviceOption(string name, ICaptureDevice device)
            {
                Name = name;
                Value = device;
            }
        }

        private List<DeviceOption> _deviceList = new List<DeviceOption>();

        public ObservableCollection<string> Packets { get; set; }

        public Sniffer Sniffer;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            EventSink.NexusPacket += OnNexusPacket;
        }

        public void OnLoaded(object sender, EventArgs args)
        {
            Packets = new ObservableCollection<string>();
            PacketList.ItemsSource = Packets;
        }

        #region Events
        public void OnToggleRunning(object sender, EventArgs args)
        {
            if (!Running && DeviceListComboBox.SelectedValue == null)
            {
                ThrowError("No valid device selected, cannot start");
                return;
            }
            ToggleRunning();
        }

        public void OnRefreshDeviceList(object sender, EventArgs args)
        {
            _deviceList.Clear();
            CaptureDeviceList devices = CaptureDeviceList.Instance;
            if (devices.Count > 0)
            {
                foreach (ICaptureDevice device in devices)
                {
                    _deviceList.Add(new DeviceOption(device.Description, device));
                }
            }
            DeviceListComboBox.ItemsSource = _deviceList;
            DeviceListComboBox.DisplayMemberPath = "Name";
            DeviceListComboBox.SelectedValuePath = "Value";
            DeviceListComboBox.SelectedIndex = 0;
        }

        public void OnNexusPacket(NexusPacketEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => Packets.Add(BitConverter.ToString(e.Data)));
        }
        #endregion

        public void ToggleRunning()
        {
            if (Running)
            {
                ToggleRunningButton.Content = "Start";
                Running = false;
                Sniffer.Stop();
                Sniffer = null;
            }
            else
            {
                ToggleRunningButton.Content = "Stop";
                Sniffer = new Sniffer((ICaptureDevice)DeviceListComboBox.SelectedValue);
                Sniffer.Start();
                Running = true;
            }            
        }

        public static void ThrowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
