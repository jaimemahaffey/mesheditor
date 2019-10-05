using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Win32;

namespace UBL_MeshEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window1 : RibbonWindow
    {
        public Window1()
        {
            InitializeComponent();
            if (DataContext != null)
                m_meshVm = DataContext as MeshViewModel;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                m_meshVm = new MeshViewModel();

                Random rand = new Random();
                Random rand2 = new Random();

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        m_meshVm.MeshPoints.Add(new MeshPointViewModel()
                        {
                            X = j,
                            Y = i,
                            Data = rand.NextDouble() * rand.Next(-1, 1)
                        });
                    }
                }
            }

            DataContextChanged += (sender, args) => { m_meshVm = DataContext as MeshViewModel; };
        }

        private MeshViewModel m_meshVm;
        private void OnIncreaseValueClicked(object sender, RoutedEventArgs e)
        {
            m_meshVm.IncrementValue(((Button)sender).DataContext as MeshPointViewModel);
        }

        private void OnDecreaseValueClicked(object sender, RoutedEventArgs e)
        {
            m_meshVm.DecrementValue(((Button)sender).DataContext as MeshPointViewModel);
        }

        private void OnPasteTextDataClick(object sender, RoutedEventArgs e)
        {
            var dataEntryDialog = new DataEntryWindow();
            dataEntryDialog.DataContext = this.DataContext;
            dataEntryDialog.Show();

            
        }


        private void UniformGridLoaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();

            var data = m_meshVm.ExportDataAsGCode();

            using (var streamWriter = new StreamWriter(saveFileDialog.OpenFile()))
            {
                streamWriter.WriteLine(data);
            }
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            //todo: save data
            Application.Current.Shutdown(0);
        }

        private void OnImportFileClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.ShowDialog(this);

            if (string.IsNullOrEmpty(openFileDialog.FileName)) return;

            using (var streamReader = new StreamReader(openFileDialog.OpenFile()))
            {
                m_meshVm.PointData = streamReader.ReadToEnd();
            }
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var color = (Color)value;
                SolidColorBrush brush = new SolidColorBrush(color);
                brush.Freeze();
                return brush;
            }
            else throw new InvalidOperationException("Incorrect cast attempted");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}