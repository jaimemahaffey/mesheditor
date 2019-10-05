using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using UBL_MeshEditor.Annotations;

namespace UBL_MeshEditor
{
    public class MeshViewModel : NotifyObject
    {
        public MeshViewModel()
        {
            GradientStops = new GradientStopCollection
            {
                new GradientStop() {Color = Colors.Red, Offset = 0.0},
                new GradientStop() {Color = Colors.Blue, Offset = 0.5},
                new GradientStop() {Color = Colors.Green, Offset = 1.0}
            };
        }

        private RelayCommand m_loadDataCommand;
        private double m_maxValue;
        private double m_minValue;
        private readonly int m_numRows = 10;

        public ICommand LoadDataCommand => m_loadDataCommand ?? (m_loadDataCommand = new RelayCommand(LoadData));

        public string PointData
        {
            get => m_pointData;
            set
            {
                m_pointData = value;
                OnPropertyChanged();
            }
        }

        private double m_increment = 0.05;


        public ObservableCollection<MeshPointViewModel> MeshPoints
        {
            get => m_meshPoints;
            set
            {
                m_meshPoints = value;
                OnPropertyChanged();
            }
        }

        public double Increment
        {
            get => m_increment;
            set
            {
                m_increment = value;
                OnPropertyChanged();
            }
        }

        public double MaxValue
        {
            get => m_maxValue;
            set
            {
                m_maxValue = value;
                OnPropertyChanged();
            }
        }

        public double MinValue
        {
            get => m_minValue;
            set
            {
                m_minValue = value;
                OnPropertyChanged();
            }
        }

        public GradientStopCollection GradientStops
        {
            get => m_gsc;
            set
            {
                m_gsc = value;
                OnPropertyChanged();
            }
        }

        private string m_pointData = "";

        private ObservableCollection<MeshPointViewModel> m_meshPoints = new ObservableCollection<MeshPointViewModel>();

        private void LoadData(object obj)
        {
            var param = obj?.ToString();
            if (string.IsNullOrEmpty(param)) return;

            //if (string.Equals(param, "File"))
            //    LoadFromFile();
            //else
            //    LoadFromText();

            LoadFromText();
        }

        private void LoadFromText()
        {
            LoadData_Impl(PointData);
        }

        private void LoadFromFile()
        {
        }

        private void LoadData_Impl(string data)
        {
            MeshPoints.Clear();
            data = data.Trim();
            var i = 0;
            int j;

            using (var reader = new StringReader(data))
            {
                string line;


                while ((line = reader.ReadLine()) != null)
                {
                    var items = line.Split(' ');
                    j = 0;
                    foreach (var item in items)
                    {
                        var pointViewModel = new MeshPointViewModel()
                        {
                            X = j,
                            Y = m_numRows - i - 1,
                            Data = double.Parse(item.Trim('[', ']')),
                        };

                        pointViewModel.OriginalData = pointViewModel.Data;
                        MeshPoints.Add(pointViewModel);

                        j++;
                    }

                    i++;
                }

                UpdateRange();
                GenerateColors();


                OnPropertyChanged(nameof(MeshPoints));
            }
        }

        public string ExportDataAsGCode()
        {
            StringBuilder exportString = new StringBuilder();

            exportString.AppendLine("G29 I999");

            var sortedMesh = MeshPoints.OrderBy(f => f.X).ThenBy(f => f.Y).ToList();

            foreach (var meshPoint in sortedMesh)
            {
                var i = meshPoint.X;
                var j = meshPoint.Y;

                exportString.Append("M421");
                exportString.Append($" I{i} J{j} Z{Math.Round(meshPoint.Data, 3)}\r\n");
            }

            foreach(var f in MeshPoints)
            {
                f.OriginalData = f.Data;
            }

            return exportString.ToString();
        }

        private GradientStopCollection m_gsc;


        private void GenerateColors()
        {
            foreach (var meshPoint in MeshPoints)
                meshPoint.DataColor =
                    GradientStops.GetRelativeColor((meshPoint.Data - MinValue) / (MaxValue - MinValue));
        }

        private void UpdateRange()
        {
            MaxValue = MeshPoints.Max(f => f.Data);
            MinValue = MeshPoints.Min(f => f.Data);
        }

        public void IncrementValue(MeshPointViewModel pointViewModel)
        {
            pointViewModel.Data += Increment;
            UpdateRange();
            GenerateColors();
        }

        public void DecrementValue(MeshPointViewModel pointViewModel)
        {
            pointViewModel.Data -= Increment;
            UpdateRange();
            GenerateColors();
        }
    }

    public class MeshPointViewModel : NotifyObject
    {
        private int m_x;
        private int m_y;
        private double m_data;
        private double m_originalData;

        public int X
        {
            get => m_x;
            set
            {
                m_x = value;
                OnPropertyChanged();
            }
        }

        public int Y
        {
            get => m_y;
            set
            {
                m_y = value;
                OnPropertyChanged();
            }
        }

        public double Data
        {
            get => m_data;
            set
            {
                m_data = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        public Color DataColor
        {
            get => m_dataColor;
            set
            {
                m_dataColor = value;
                OnPropertyChanged();
            }
        }

        public double OriginalData
        {
            get => m_originalData;
            set
            {
                m_originalData = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsChanged));
            }
        }


        public bool IsChanged => OriginalData != Data;

        private Color m_dataColor;
    }

    public class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'.</summary>
    /// <remarks>Taken from http://stackoverflow.com/questions/3531772/binding-button-click-to-a-method </remarks>
    public class RelayCommand : ICommand
    {
        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.</summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.</summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            m_execute = execute;
            m_canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameters)
        {
            return m_canExecute == null ? true : m_canExecute(parameters);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameters)
        {
            m_execute(parameters);
        }

        #endregion

        #region Private Members

        private readonly Action<object> m_execute;
        private readonly Predicate<object> m_canExecute;

        #endregion
    }

    public static class GradientStopCollectionExtensions
    {
        public static Color GetRelativeColor(this GradientStopCollection gsc, double offset)
        {
            var point = gsc.SingleOrDefault(f => f.Offset == offset);
            if (point != null) return point.Color;

            var before = gsc.First(w => w.Offset == gsc.Min(m => m.Offset));
            var after = gsc.First(w => w.Offset == gsc.Max(m => m.Offset));

            foreach (var gs in gsc)
            {
                if (gs.Offset < offset && gs.Offset > before.Offset) before = gs;

                if (gs.Offset > offset && gs.Offset < after.Offset) after = gs;
            }

            var color = new Color
            {
                ScA = (float) ((offset - before.Offset) * (after.Color.ScA - before.Color.ScA) /
                               (after.Offset - before.Offset) + before.Color.ScA),
                ScR = (float) ((offset - before.Offset) * (after.Color.ScR - before.Color.ScR) /
                               (after.Offset - before.Offset) + before.Color.ScR),
                ScG = (float) ((offset - before.Offset) * (after.Color.ScG - before.Color.ScG) /
                               (after.Offset - before.Offset) + before.Color.ScG),
                ScB = (float) ((offset - before.Offset) * (after.Color.ScB - before.Color.ScB) /
                               (after.Offset - before.Offset) + before.Color.ScB)
            };


            return color;
        }
    }
}