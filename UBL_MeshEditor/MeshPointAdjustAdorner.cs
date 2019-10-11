using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UBL_MeshEditor.Annotations;

namespace UBL_MeshEditor
{
    public class MeshPointAdjustAdorner : Adorner
    {
        private Button m_incButton;
        private Button m_decButton;

        public MeshPointAdjustAdorner([NotNull] UIElement adornedElement) : base(adornedElement)
        {
            
        }



        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }
}
