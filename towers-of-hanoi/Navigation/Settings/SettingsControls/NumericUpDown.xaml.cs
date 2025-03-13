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

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int),
            typeof(NumericUpDown),
            new PropertyMetadata(0, OnValueChanged)
            );
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(int),
            typeof(NumericUpDown)
            );
        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int),
            typeof(NumericUpDown)
            );
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public event EventHandler ValueChanged = delegate { };

        public NumericUpDown()
        {
            InitializeComponent();
            Value = 0;
            MinValue = -10;
            MaxValue = 10;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericUpDown control && control.Output != null)
            {
                control.Output.Text = e.NewValue.ToString();
            }
        }

        private void OutputTextChanged(object sender, TextChangedEventArgs e)
        {
            string sanitised = "";
            foreach (char character in Output.Text)
            {
                if (character >= '0' && character <= '9')
                {
                    sanitised += character;
                }
            }
            Output.Text = sanitised;
            if (Int32.TryParse(sanitised, out int value) && value <= MinValue && value >= MaxValue)
            {
                Value = value;
            }
        }

        private void OutputLostFocus(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(Output.Text, out int value))
            {
                if (value >= MaxValue)
                {
                    Value = MaxValue;
                }
                else if (value <= MinValue)
                {
                    Value = MinValue;
                }
                else
                {
                    Value = value;
                }
            }
        }

        public void UpClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            if (Value + 1 <= MaxValue)
            {
                Value += 1;
            }
        }

        public void DownClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            if (Value - 1 >= MinValue)
            {
                Value -= 1;
            }
        }
    }
}
