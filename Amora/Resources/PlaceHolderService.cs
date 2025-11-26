using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Amora.Resources
{
    // PlaceHolderService soll lediglich als Hilfsmethode, für die Placeholder-Funktion fungieren.
    // Ganz offensichtlich nicht selber geschrieben, aber war ein muss um PlaceHolder-Text in die Textbox und Passwordbox einzufügen
    public class PlaceHolderService
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(PlaceHolderService),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(Control control) => (string)control.GetValue(PlaceholderProperty);
        public static void SetPlaceholder(Control control, string value) => control.SetValue(PlaceholderProperty, value);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                control.Loaded += (s, ev) =>
                {
                    var layer = AdornerLayer.GetAdornerLayer(control);
                    if (layer != null)
                    {
                        var adorner = new PlaceholderAdorner(control, GetPlaceholder(control));
                        layer.Add(adorner);

                        if (control is TextBox tb)
                        {
                            // Initialer Zustand
                            adorner.Visibility = string.IsNullOrEmpty(tb.Text)
                                ? Visibility.Visible
                                : Visibility.Collapsed;

                            tb.TextChanged += (s2, ev2) =>
                            {
                                adorner.Visibility = string.IsNullOrEmpty(tb.Text)
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            };

                            // Neu: Beim Fokussieren Placeholder ausblenden
                            tb.GotFocus += (s3, ev3) =>
                            {
                                adorner.Visibility = Visibility.Collapsed;
                            };

                            // Neu: Beim Verlassen wieder anzeigen, falls leer
                            tb.LostFocus += (s4, ev4) =>
                            {
                                adorner.Visibility = string.IsNullOrEmpty(tb.Text)
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            };
                        }
                        else if (control is PasswordBox pb)
                        {
                            adorner.Visibility = string.IsNullOrEmpty(pb.Password)
                                ? Visibility.Visible
                                : Visibility.Collapsed;

                            pb.PasswordChanged += (s2, ev2) =>
                            {
                                adorner.Visibility = string.IsNullOrEmpty(pb.Password)
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            };

                            pb.GotFocus += (s3, ev3) =>
                            {
                                adorner.Visibility = Visibility.Collapsed;
                            };

                            pb.LostFocus += (s4, ev4) =>
                            {
                                adorner.Visibility = string.IsNullOrEmpty(pb.Password)
                                    ? Visibility.Visible
                                    : Visibility.Collapsed;
                            };
                        }
                    }
                };
            }
        }

    }



    public class PlaceholderAdorner : Adorner
    {
        private readonly TextBlock _textBlock;

        public PlaceholderAdorner(UIElement adornedElement, string placeholder) : base(adornedElement)
        {
            _textBlock = new TextBlock
            {
                Text = placeholder,
                Foreground = Brushes.Gray,
                FontSize = 14,
                Opacity = 0.7,
                IsHitTestVisible = false,
                TextAlignment = TextAlignment.Center
            };

            _textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            AddVisualChild(_textBlock);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _textBlock;

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Zentrierte Position berechnen
            double x = (finalSize.Width - _textBlock.DesiredSize.Width) / 2;
            double y = (finalSize.Height - _textBlock.DesiredSize.Height) / 2;

            _textBlock.Arrange(new Rect(new Point(x, y), _textBlock.DesiredSize));
            return finalSize;
        }
    }



}

