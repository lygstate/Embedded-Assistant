using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.IO.Ports;
using System.Reflection;

namespace Embedded_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitCOMs();
        }

        readonly string[] defaultCOM_Names = { "无串口" };

        readonly string[] defaultBaudrates =
        {
            "50", "75", "110", "150",
            "300", "600", "1200", "2400",
            "4800", "9600", "14400", "19200",
            "28800", "38400", "56000", "57600",
            "115200", "128000", "230400", "460800",
            "921600",
        };

        void InitCOMs()
        {
            this.UpdateComboBox(ref this.ComNamesComboBox, SerialPort.GetPortNames(), defaultCOM_Names);
            this.ComNamesComboBox.SelectedIndex = 0;
            this.UpdateBaudrate();
        }

        void UpdateBaudrate()
        {
            try
            {
                ComboBoxItem item = (ComboBoxItem)this.ComNamesComboBox.SelectedItem;
                var COM_Name = (string)item.Content;
                var _serialPort = new SerialPort(COM_Name);
                _serialPort.Open();
                Int32 dwSettableBaud = 0;
                try
                {
                    // Getting COMMPROP structure, and its property dwSettableBaud.
                    object p = _serialPort.BaseStream.GetType().GetField("commProp",
                       BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_serialPort.BaseStream);
                    dwSettableBaud = (Int32)p.GetType().GetField("dwSettableBaud",
                       BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);
                }
                finally
                {
                    _serialPort.Close();
                }
                var bauds = GetSettableBauds(dwSettableBaud);
                UpdateComboBox(ref this.BaudratesComboBox, bauds, defaultBaudrates);
                SetDefaultComboBoxItem(ref this.BaudratesComboBox, "9600");
            }
            catch(Exception e)
            {
            }
        }

        static string toString(object o)
        {
            try
            {
                return (string)o;
            }
            catch(Exception)
            {
                return o.ToString();
            }
        }

        static string[] GetSettableBauds(Int32 possibleBaudRates)
        {
            var _baudRateCollection = new List<Int32>();
            const int BAUD_075 = 0x00000001;
            const int BAUD_110 = 0x00000002;
            //const int BAUD_134_5 = 0x00000004;
            const int BAUD_150 = 0x00000008;
            const int BAUD_300 = 0x00000010;
            const int BAUD_600 = 0x00000020;
            const int BAUD_1200 = 0x00000040;
            const int BAUD_1800 = 0x00000080;
            const int BAUD_2400 = 0x00000100;
            const int BAUD_4800 = 0x00000200;
            const int BAUD_7200 = 0x00000400;
            const int BAUD_9600 = 0x00000800;
            const int BAUD_14400 = 0x00001000;
            const int BAUD_19200 = 0x00002000;
            const int BAUD_38400 = 0x00004000;
            const int BAUD_56K = 0x00008000;
            const int BAUD_57600 = 0x00040000;
            const int BAUD_115200 = 0x00020000;
            const int BAUD_128K = 0x00010000;
            if ((possibleBaudRates & BAUD_075) > 0)
                _baudRateCollection.Add(75);
            if ((possibleBaudRates & BAUD_110) > 0)
                _baudRateCollection.Add(110);
            if ((possibleBaudRates & BAUD_150) > 0)
                _baudRateCollection.Add(150);
            if ((possibleBaudRates & BAUD_300) > 0)
                _baudRateCollection.Add(300);
            if ((possibleBaudRates & BAUD_600) > 0)
                _baudRateCollection.Add(600);
            if ((possibleBaudRates & BAUD_1200) > 0)
                _baudRateCollection.Add(1200);
            if ((possibleBaudRates & BAUD_1800) > 0)
                _baudRateCollection.Add(1800);
            if ((possibleBaudRates & BAUD_2400) > 0)
                _baudRateCollection.Add(2400);
            if ((possibleBaudRates & BAUD_4800) > 0)
                _baudRateCollection.Add(4800);
            if ((possibleBaudRates & BAUD_7200) > 0)
                _baudRateCollection.Add(7200);
            if ((possibleBaudRates & BAUD_9600) > 0)
                _baudRateCollection.Add(9600);
            if ((possibleBaudRates & BAUD_14400) > 0)
                _baudRateCollection.Add(14400);
            if ((possibleBaudRates & BAUD_19200) > 0)
                _baudRateCollection.Add(19200);
            if ((possibleBaudRates & BAUD_38400) > 0)
                _baudRateCollection.Add(38400);
            if ((possibleBaudRates & BAUD_56K) > 0)
                _baudRateCollection.Add(56000);
            if ((possibleBaudRates & BAUD_57600) > 0)
                _baudRateCollection.Add(57600);
            if ((possibleBaudRates & BAUD_115200) > 0)
                _baudRateCollection.Add(115200);
            if ((possibleBaudRates & BAUD_128K) > 0)
                _baudRateCollection.Add(128000);

            string[] sa = new string[_baudRateCollection.Count];
            for (int i = 0; i < _baudRateCollection.Count; ++i)
            {
                sa[i] = _baudRateCollection[i].ToString();
            }
            return sa;
        }

        void SetDefaultComboBoxItem(ref ComboBox box, string defaultContent)
        {
            foreach (ComboBoxItem item in box.Items)
            {
                if ((string)item.Content == defaultContent)
                {
                    box.SelectedItem = item;
                    return;
                }
            }
            box.SelectedIndex = 0;
        }

        void UpdateComboBox(ref ComboBox box, object[] items, object[] defaultItems)
        {
            box.Items.Clear();

            if (items.Length == 0)
            {
                items = defaultItems;
            }
            foreach (var item in items)
            {
                var comboItem = new ComboBoxItem();
                comboItem.Content = item.ToString();
                box.Items.Add(comboItem);
            }

            box.SelectedIndex = 0;
        }
    }
}
