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

namespace towers_of_hanoi.SettingsControls
{
    /// <summary>
    /// Interaction logic for FloatSlider.xaml
    /// </summary>
    public partial class FloatSlider : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double),
            typeof(FloatSlider),
            new PropertyMetadata(0.0, OnValueChanged)
            );
        public double Value
        {
            get => (double)GetValue(ValueProperty);
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
            "MinValue", typeof(double),
            typeof(FloatSlider),
            new PropertyMetadata(0.0, OnMinimumChanged)
            );
        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double),
            typeof(FloatSlider),
            new PropertyMetadata(-1.0, OnMaximumChanged)
            );
        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public event EventHandler ValueChanged = delegate { };

        public FloatSlider()
        {
            InitializeComponent();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FloatSlider control && control.InputSlider != null)
            {
                control.InputSlider.Value = (double)(e.NewValue);
            }
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FloatSlider control && control.InputSlider != null && control.OutputMinimum != null)
            {
                control.InputSlider.Minimum = (double)(e.NewValue);
                control.OutputMinimum.Text = string.Format("{0:0.00}", e.NewValue);
            }
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FloatSlider control && control.InputSlider != null && control.OutputMaximum != null)
            {
                control.InputSlider.Maximum = (double)(e.NewValue);
                control.OutputMaximum.Text = string.Format("{0:0.00}", e.NewValue);
            }
        }

        private void InputSliderValueChanged(object sender, EventArgs e)
        {
            Value = (float)(InputSlider.Value);
            InputTextBox.Text = string.Format("{0:0.00}", InputSlider.Value);
        }
    }
}
