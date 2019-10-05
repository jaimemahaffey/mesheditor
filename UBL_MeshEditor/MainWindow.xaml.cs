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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UBL_MeshEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

           MeshGeometry3D cubeGeometry = new MeshGeometry3D();
           Point3DCollection points = new Point3DCollection();

           points.Add(new Point3D(0,0,0));
           points.Add(new Point3D(1,0,0));
           points.Add(new Point3D(0,1,0));
           points.Add(new Point3D(1,1,0));
           points.Add(new Point3D(0,0,1));
           points.Add(new Point3D(1,0,1));
           points.Add(new Point3D(0,1,1));
           points.Add(new Point3D(1, 1, 1));
           cubeGeometry.Positions = points;

           Int32Collection triangleIndices = new Int32Collection
           {
               2, 3, 1,  2, 1, 0,  7, 1, 3,  7, 5, 1,  6, 5, 7,  6, 4, 5,  6, 2, 0,  2, 0, 4,  2, 7, 3,  2, 6, 7,  0, 1, 5,  0, 5, 4,
           };

            cubeGeometry.TriangleIndices = triangleIndices;


           model.Geometry = cubeGeometry;
        }
    }
}
