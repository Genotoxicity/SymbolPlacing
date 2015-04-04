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
using System.Windows.Shapes;

namespace SymbolPlacing
{
    /// <summary>
    /// Interaction logic for CharacteristicsWindow.xaml
    /// </summary>
    public partial class CharacteristicsWindow : Window
    {
        private List<PlaceSymbol> symbols;
        private bool itemChanging;

        public CharacteristicsWindow()
        {
            InitializeComponent();
        }

        public void Display(List<PlaceSymbol> symbols)
        {
            SymbolsListBox.Items.Clear();
            this.symbols = symbols;
            symbols.ForEach(s => SymbolsListBox.Items.Add(s.Name));
            Show();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (!itemChanging)
            {
                int selectedIndex = SymbolsListBox.SelectedIndex;
                if (selectedIndex>=0)
                {
                    if (NotDefinedRadioButton.IsChecked==true)
                        symbols[selectedIndex].SetCharacteristic(0);
                    if (ErrorRadioButton.IsChecked == true)
                        symbols[selectedIndex].SetCharacteristic(1);
                    if (ConnectedRadioButton.IsChecked == true)
                        symbols[selectedIndex].SetCharacteristic(2);
                    if (DisabledRadioButton.IsChecked == true)
                        symbols[selectedIndex].SetCharacteristic(3);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void SymbolsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemChanging = true;
            int selectedIndex = SymbolsListBox.SelectedIndex;
            if (selectedIndex>=0)
            {
                int stateIndex = symbols[selectedIndex].GetStateIndex();
                switch (stateIndex)
                {
                    case 0:
                        NotDefinedRadioButton.IsChecked = true;
                        break;
                    case 1:
                        ErrorRadioButton.IsChecked = true;
                        break;
                    case 2:
                        ConnectedRadioButton.IsChecked = true;
                        break;
                    case 3:
                        DisabledRadioButton.IsChecked = true;
                        break;
                    default:
                        NotDefinedRadioButton.IsChecked = false;
                        ConnectedRadioButton.IsChecked = false;
                        DisabledRadioButton.IsChecked = false;
                        ErrorRadioButton.IsChecked = false;
                        break;
                }
            }
            itemChanging = false;
        }


    }
}
