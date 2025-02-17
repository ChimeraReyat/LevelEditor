//Copyright © 2014 Sony Computer Entertainment America LLC. See License.txt.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Sce.Atf.Applications;
using Sce.Atf.VectorMath;
using Sce.Atf.Direct2D;

namespace Sce.Atf.Controls.Adaptable
{
    /// <summary>
    /// Adapter to draw a grid on a diagram and to perform layout constraints
    /// using that grid</summary>
    public class D2dGridAdapter : ControlAdapter, ILayoutConstraint
    {
        /// <summary>
        /// Gets or sets whether the grid is visible</summary>
        public bool Visible
        {
            get { return m_visible; }
            set
            {
                if (m_visible != value)
                {
                    m_visible = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the grid's contrast with the control's back color; should be in [0..1];
        /// default is 0.25.</summary>
        public float GridContrast
        {
            get { return m_gridContrast; }
            set
            {
                m_gridContrast = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the horizontal grid step size</summary>
        public int HorizontalGridSpacing
        {
            get { return m_horizontalGridSpacing; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                m_horizontalGridSpacing = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the vertical grid step size</summary>
        public int VerticalGridSpacing
        {
            get { return m_verticalGridSpacing; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                m_verticalGridSpacing = value;

                Invalidate();
            }
        }

        #region ILayoutConstraint Members

        /// <summary>
        /// Gets the displayable name of the constraint</summary>
        public string Name
        {
            get { return "Grid".Localize("Grid location constraint"); }
        }

        /// <summary>
        /// Gets or sets whether the constraint is enabled and the grid rendered</summary>
        /// <remarks>Setting the Enabled property also sets Visible, i.e., shows or hides the grid.
        /// Use ConstraintEnabled to enable or disable the constraint without any effect on rendering.</remarks>
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                m_enabled = value;
                m_visible = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the constraint is enabled (without any effect on grid rendering)</summary>
        public bool ConstraintEnabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        /// <summary>
        /// Applies constraint to bounding rectangle</summary>
        /// <param name="bounds">Unconstrained bounding rectangle</param>
        /// <param name="specified">Flags indicating which parts of bounding rectangle are meaningful</param>
        /// <returns>Constrained bounding rectangle</returns>
        public Rectangle Constrain(Rectangle bounds, BoundsSpecified specified)
        {
            if ((specified & BoundsSpecified.X) != 0)
            {
                bounds.X = (int)MathUtil.Snap(bounds.X, m_horizontalGridSpacing);

                if ((specified & BoundsSpecified.Width) != 0)
                {
                    bounds.Width = (int)MathUtil.Snap(bounds.Width, m_horizontalGridSpacing);
                    bounds.Width = Math.Max(bounds.Width, 0);
                }
            }

            if ((specified & BoundsSpecified.Y) != 0)
            {
                bounds.Y = (int)MathUtil.Snap(bounds.Y, m_verticalGridSpacing);

                if ((specified & BoundsSpecified.Height) != 0)
                {
                    bounds.Height = (int)MathUtil.Snap(bounds.Height, m_verticalGridSpacing);
                    bounds.Height = Math.Max(bounds.Height, 0);
                }
            }

            return bounds;
        }

        #endregion

        /// <summary>
        /// Binds the adapter to the adaptable control. Called in the order that the adapters
        /// were defined on the control.</summary>
        /// <param name="control">Adaptable control</param>
        protected override void Bind(AdaptableControl control)
        {
            m_transformAdapter = control.As<ITransformAdapter>();
            SetGridColor();
            var d2dControl = control as D2dAdaptableControl;
            d2dControl.DrawingD2d += control_DrawingD2d;
            control.BackColorChanged += control_BackColorChanged;
        }

        /// <summary>
        /// Unbinds the adapter from the adaptable control</summary>
        /// <param name="control">Adaptable control</param>
        protected override void Unbind(AdaptableControl control)
        {
            var d2dControl = control as D2dAdaptableControl;
            d2dControl.DrawingD2d -= control_DrawingD2d;
        }

        /// <summary>
        /// Sets grid color to adapted control's background color</summary>
        protected virtual void SetGridColor()
        {
            Color gridColor = AdaptedControl.BackColor;
            float intensity = ((float)gridColor.R + (float)gridColor.G + (float)gridColor.B) / 3;
            float shading = (intensity < 128) ? m_gridContrast : -m_gridContrast;
            m_gridColor = ColorUtil.GetShade(gridColor, 1 + shading);
        }

        private void control_BackColorChanged(object sender, EventArgs e)
        {
            SetGridColor();
        }

        // draw dragged events
        private void control_DrawingD2d(object sender, EventArgs e)
        {
            if (m_visible)
            {
                var d2dControl = AdaptedControl as D2dAdaptableControl;
                Matrix3x2F xform = d2dControl.D2dGraphics.Transform;
                d2dControl.D2dGraphics.Transform = Matrix3x2F.Identity;
                Matrix transform = m_transformAdapter.Transform;
                RectangleF clientRect = AdaptedControl.ClientRectangle;
                RectangleF canvasRect = GdiUtil.InverseTransform(transform, clientRect);

                D2dAntialiasMode oldAA = d2dControl.D2dGraphics.AntialiasMode;
                d2dControl.D2dGraphics.AntialiasMode = D2dAntialiasMode.Aliased;
                // draw horizontal lines
                ChartUtil.DrawHorizontalGrid(transform, canvasRect, m_verticalGridSpacing, m_gridColor, d2dControl.D2dGraphics);

                // draw vertical lines
                ChartUtil.DrawVerticalGrid(transform, canvasRect, m_horizontalGridSpacing, m_gridColor, d2dControl.D2dGraphics);

                d2dControl.D2dGraphics.Transform = xform;
                d2dControl.D2dGraphics.AntialiasMode = oldAA;
            }
        }

        private void Invalidate()
        {
            if (AdaptedControl != null)
                AdaptedControl.Invalidate();
        }

        private ITransformAdapter m_transformAdapter;
        private Color m_gridColor;
        private float m_gridContrast = 0.25f;
        private int m_horizontalGridSpacing = 32;
        private int m_verticalGridSpacing = 32;
        private bool m_enabled = true;
        private bool m_visible = true;
    }
}
