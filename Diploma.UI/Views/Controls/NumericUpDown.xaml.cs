using System.Windows;
using System.Windows.Controls;

namespace Diploma.UI.Views.Controls
{

    public partial class NumericUpDown : UserControl
    {

        public int MinValue
        {
            get =>
                (int)GetValue(MinValueProperty);

            set =>
                SetValue(MinValueProperty, value);
        }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            nameof(MinValue), typeof(int), typeof(NumericUpDown));

        public int MaxValue
        {
            get =>
                (int)GetValue(MaxValueProperty);

            set =>
                SetValue(MaxValueProperty, value);
        }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            nameof(MaxValue), typeof(int), typeof(NumericUpDown));

        public int Increment
        {
            get =>
                (int)GetValue(IncrementProperty);

            set =>
                SetValue(IncrementProperty, value);
        }
        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
            nameof(Increment), typeof(int), typeof(NumericUpDown));

        public int Value
        {
            get =>
                (int)GetValue(ValueProperty);

            set =>
                SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(int), typeof(NumericUpDown));

        public NumericUpDown()
        {
            InitializeComponent();
        }

    }

}