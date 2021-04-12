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

namespace CoreLibrary.Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SuppliersManager _manager;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            _manager.Stop();

            _manager?.Dispose();
            _manager = null;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TextBoxSuppliers.Text, out var suppliers))
            {
                _manager = new SuppliersManager(suppliers, 1000, 5000);
                _manager.NewDataProcessed += (value) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TextBoxOutput.Text += value + "\r\n";
                    });
                    
                };
                _manager.Start();
            }
        }
    }
}
