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
using ProELib;

namespace SymbolPlacing
{
    public partial class UI : Window
    {
        private E3ApplicationInfo applicationInfo;
        private CharacteristicsWindow charWindow;

        public UI()
        {
            applicationInfo = new E3ApplicationInfo();
            InitializeComponent();
            MinHeight = Height;
            MinWidth = Width;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (applicationInfo.Status == SelectionStatus.Selected)
            {
                richTextBox.AppendText(applicationInfo.MainWindowTitle);
                charWindow = new CharacteristicsWindow();
            }
            else
            {
                richTextBox.AppendText(applicationInfo.StatusReasonDescription);
                PlaceButton.IsEnabled = false;
                CharacteristicButton.IsEnabled = false;
            }
        }

        private void PlaceButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            //charWindow.Hide();
            Script script = new Script(charWindow, applicationInfo.ProcessId);
            script.PlaceSymbols();
            Cursor = Cursors.Arrow;
        }

        private void CharacteristicButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Script script = new Script(charWindow, applicationInfo.ProcessId);
            script.SetCharacteristics();
            Cursor = Cursors.Arrow;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
