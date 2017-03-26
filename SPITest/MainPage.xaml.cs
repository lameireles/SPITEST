using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SPITest
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SpiDevice MAGI;
        private DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task StartScenarioAsync()
        {
            String spiDeviceSelector = SpiDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(spiDeviceSelector);

            // 0 = Chip select line to use.
            var MAGI_Settings = new SpiConnectionSettings(0);

            // 5MHz is the rated speed of the ADXL345 accelerometer.
          //  MAGI_Settings.ClockFrequency = 5000000;

            // The accelerometer expects an idle-high clock polarity.
            // We use Mode3 to set the clock polarity and phase to: CPOL = 1, CPHA = 1.
            MAGI_Settings.Mode = SpiMode.Mode0;
            // If this next line crashes with an ArgumentOutOfRangeException,
            // then the problem is that no SPI devices were found.
            //
            // If the next line crashes with Access Denied, then the problem is
            // that access to the SPI device (ADXL345) is denied.
            //
            // The call to FromIdAsync will also crash if the settings are invalid.
            //
            // FromIdAsync produces null if there is a sharing violation on the device.
            // This will result in a NullReferenceException a few lines later.
            MAGI = await SpiDevice.FromIdAsync(devices[0].Id, MAGI_Settings);
           
            textBlock1.Text = MAGI.DeviceId.ToString();
            // 
            // Initialize the accelerometer:
            //
            // For this device, we create 2-byte write buffers:
            // The first byte is the register address we want to write to.
            // The second byte is the contents that we want to write to the register. 
            //

           
        // MAGI.Write(WriteBuf_PowerControl);

            // Start the polling timer.

            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(50) };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await StartScenarioAsync();
        }
        void Timer_Tick(object sender, object e)
        {
            // const byte ACCEL_SPI_RW_BIT = 0x80; // Bit used in SPI transactions to indicate read/write
            // const byte ACCEL_SPI_MB_BIT = 0x40; // Bit used to indicate multi-byte SPI transactions
            const byte ACCEL_REG_X = 0x32;      // Address of the X Axis data register
            //const int ACCEL_RES = 1024;         // The ADXL345 has 10 bit resolution giving 1024 unique values
            //const int ACCEL_DYN_RANGE_G = 8;    // The ADXL345 had a total dynamic range of 8G, since we're configuring it to +-4G 
            //const int UNITS_PER_G = ACCEL_RES / ACCEL_DYN_RANGE_G;  // Ratio of raw int values to G units

            byte[] ReadBuf = new byte[1];    // Read buffer of size 6 bytes (2 bytes * 3 axes) + 1 byte padding
            byte[] RegAddrBuf = new byte[1 + 6]; // Register address buffer of size 1 byte + 6 bytes padding

            // Register address we want to read from with read and multi-byte bit set
            RegAddrBuf[0] = ACCEL_REG_X;

            // If this next line crashes, then there was an error communicating with the device.
            //MAGI.TransferFullDuplex(RegAddrBuf, ReadBuf);
            MAGI.Read(ReadBuf);
            // In order to get the raw 16-bit data values, we need to concatenate two 8-bit bytes for each axis
            //short AccelerationRawX = BitConverter.ToInt16(ReadBuf, 1);
            //short AccelerationRawY = BitConverter.ToInt16(ReadBuf, 3);
            //short AccelerationRawZ = BitConverter.ToInt16(ReadBuf, 5);

            // Convert raw values to G's and display them.
            if (ReadBuf[0].ToString()!="0") { 
            RxText.Text = ReadBuf[0].ToString();
        }
           // X.Text = ((double)AccelerationRawX / UNITS_PER_G).ToString();
            //Y.Text = ((double)AccelerationRawY / UNITS_PER_G).ToString();
            //Z.Text = ((double)AccelerationRawZ / UNITS_PER_G).ToString();
        }

        private void envia_Click(object sender, RoutedEventArgs e)
        {
            byte[] WriteBuf_DataFormat = new byte[] {12};

            // 0x2D is address of power control register, 0x08 puts the accelerometer into measurement mode.

        //    byte[] WriteBuf_PowerControl = new byte[] { 0x2D, 0x08 };

            // Write the register settings.
            //
            // If this next line crashes with a NullReferenceException, then
            // there was a sharing violation on the device.
            // (See comment earlier in this function.)
            //
            // If this next line crashes for some other reason, then there was
            // an error communicating with the device.

            //MAGI.Write(WriteBuf_DataFormat);
            MAGI.Write(new byte[] {12});
            MAGI.Write(new byte[] { 14 });
            MAGI.Write(new byte[] { 15 });
           // MAGI.Write(new byte[] { 48 });
            MAGI.Write(new byte[] { 13 });
          //  MAGI.Write(WriteBuf_DataFormat);

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MAGI.Write(new byte[] { 21 });
            MAGI.Write(new byte[] { 22 });
            MAGI.Write(new byte[] { 23 });
            // MAGI.Write(new byte[] { 48 });
            MAGI.Write(new byte[] { 13 });
        }
    }
}
