using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using moddingSuite.View.Enums;

namespace moddingSuite.View
{  
   [TemplatePart(Name = "PART_Container", Type = typeof(Canvas))]
   public class SpinningWheel : Control
   {
      private Canvas container = null;
      private Storyboard storyBoard = new Storyboard();
      private DoubleAnimation rotateAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1)));

      static SpinningWheel()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(SpinningWheel), new FrameworkPropertyMetadata(typeof(SpinningWheel)));         
      }

      public int DotCount
      {
         get { return (int)GetValue(DotCountProperty); }
         set { SetValue(DotCountProperty, value); }
      }

      public static readonly DependencyProperty DotCountProperty =
          DependencyProperty.Register("DotCount", typeof(int), typeof(SpinningWheel), new FrameworkPropertyMetadata(12, FrameworkPropertyMetadataOptions.AffectsMeasure, OnDotCountChanged));

      public Brush DotColor
      {
         get { return (Brush)GetValue(DotColorProperty); }
         set { SetValue(DotColorProperty, value); }
      }

      public static readonly DependencyProperty DotColorProperty =
          DependencyProperty.Register("DotColor", typeof(Brush), typeof(SpinningWheel), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 122, 204)), FrameworkPropertyMetadataOptions.AffectsRender)); // windows 7 blue

      public double DotRadius
      {
         get { return (double)GetValue(DotRadiusProperty); }
         set { SetValue(DotRadiusProperty, value); }
      }

      public static readonly DependencyProperty DotRadiusProperty =
          DependencyProperty.Register("DotRadius", typeof(double), typeof(SpinningWheel), new FrameworkPropertyMetadata(3.0, FrameworkPropertyMetadataOptions.AffectsArrange, OnDotRadiusChanged));
      
      public double Radius
      {
         get { return (double)GetValue(RadiusProperty); }
         set { SetValue(RadiusProperty, value); }
      }

      public static readonly DependencyProperty RadiusProperty =
          DependencyProperty.Register("Radius", typeof(double), typeof(SpinningWheel), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsMeasure ,OnRadiusChanged));


      public bool IsSpinning
      {
         get { return (bool)GetValue(IsSpinningProperty); }
         set { SetValue(IsSpinningProperty, value); }
      }

      public static readonly DependencyProperty IsSpinningProperty =
          DependencyProperty.Register("IsSpinning", typeof(bool), typeof(SpinningWheel), new UIPropertyMetadata(true, OnIsSpinningChanged));

      public double Speed
      {
         get { return (double)GetValue(SpeedProperty); }
         set { SetValue(SpeedProperty, value); }
      }

      public static readonly DependencyProperty SpeedProperty =
          DependencyProperty.Register("Speed", typeof(double), typeof(SpinningWheel), new UIPropertyMetadata(1.0, OnSpeedChanged));


      public RotateDirection Direction
      {
         get { return (RotateDirection)GetValue(DirectionProperty); }
         set { SetValue(DirectionProperty, value); }
      }

      public static readonly DependencyProperty DirectionProperty =
          DependencyProperty.Register("Direction", typeof(RotateDirection), typeof(SpinningWheel), new UIPropertyMetadata(RotateDirection.CW, OnDirectionChanged));

      public bool SymmetricalArrange
      {
         get { return (bool)GetValue(SymmetricalArrangeProperty); }
         set { SetValue(SymmetricalArrangeProperty, value); }
      }

      public static readonly DependencyProperty SymmetricalArrangeProperty =
          DependencyProperty.Register("SymmetricalArrange", typeof(bool), typeof(SpinningWheel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange, OnRadiusChanged));
      
      private static void OnDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
      { 
         var wheel = obj as SpinningWheel;
         if (wheel != null && e.NewValue != null && wheel.storyBoard != null)
         {
            var prevState = wheel.IsSpinning;

            wheel.ToggleSpinning(false);
            wheel.rotateAnimation.To *= -1;            
            wheel.ToggleSpinning(prevState);
         }
      }
      
      private static void OnIsSpinningChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
      {
         var wheel = obj as SpinningWheel;
         if (wheel != null && e.NewValue != null && wheel.storyBoard != null)
         {            
            wheel.ToggleSpinning((bool)e.NewValue);
         }
      }

      private void ToggleSpinning(bool value)
      {
         if (value)
         {
            this.storyBoard.Begin();
         }
         else 
         {
            this.storyBoard.Stop();
         }
      }

      private static void OnDotCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
      {
         var wheel = obj as SpinningWheel;
         if (wheel != null && wheel.container != null && e.NewValue != null)
         {
            wheel.container.Children.RemoveRange(0, (int)e.OldValue);

            wheel.GenerateDots();
         }
      }

      private static void OnRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
      { 
         var wheel = obj as SpinningWheel;
         if (wheel != null && wheel.container != null && e.NewValue != null)
         {
            UpdateEllipses(wheel.container.Children, (c, ellipse) => wheel.SetEllipsePosition(ellipse, c));
         }
      }

      private static void UpdateEllipses(IEnumerable ellipses, Action<int, Ellipse> updateMethod)
      {
         if (updateMethod != null && ellipses != null)
         {
            int i = 1;
            foreach (var child in ellipses)
            {               
               var ellipse = (child as Ellipse);
               if (ellipse != null)
               {
                  updateMethod(i++, ellipse);
               }
            }
         }
      }

      private static void OnDotRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
      { 
         var wheel = obj as SpinningWheel;
         if (wheel != null && wheel.container != null && e.NewValue != null)
         {
            var newRadius = (double)e.NewValue;            
            UpdateEllipses(wheel.container.Children, (c, ellipse) =>
               {
                  ellipse.Width = newRadius * 2;
                  ellipse.Height = newRadius * 2;

                  wheel.SetEllipsePosition(ellipse, c);
               });
         }
      }             

      private static void OnSpeedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
      {
         var wheel = obj as SpinningWheel;
         if (wheel != null && wheel.storyBoard != null)
         {
            // don't ask
            wheel.storyBoard.SetSpeedRatio((double)e.NewValue);
            wheel.rotateAnimation.SpeedRatio = (double)e.NewValue;
         }
      }

      public override void OnApplyTemplate()
      {
         base.OnApplyTemplate();

         this.container = this.GetTemplateChild("PART_Container") as Canvas; 

         this.InitializeControl();
      }

      private void InitializeControl()
      {        
         this.GenerateDots();
         this.CreateAnimation();

         this.ToggleSpinning(this.IsSpinning);            
      }

      private Ellipse CreateEllipse(int counter)
      {
         var ellipse = new Ellipse();
         ellipse.Fill = this.DotColor;
         ellipse.Width = this.DotRadius * 2;
         ellipse.Height = this.DotRadius * 2;
         ellipse.Opacity = (double)counter / (double)this.DotCount;

         this.SetEllipsePosition(ellipse, counter);

         return ellipse;
      }
      
      private Point CalculatePosition(double radian)
      {         
         var x = 0 + this.Radius * Math.Cos(radian);
         var y = 0 + this.Radius * Math.Sin(radian);

         return new Point(x - this.DotRadius, y - this.DotRadius);
      }

      private void SetEllipsePosition(Ellipse ellipse, int ellipseCounter)
      {
         var maxCount = this.SymmetricalArrange ? this.DotCount : (2 * this.Radius * Math.PI) / (2 * this.DotRadius + 2);

         var position = this.CalculatePosition(ellipseCounter * 2 * Math.PI / maxCount);
         Canvas.SetLeft(ellipse, position.X);
         Canvas.SetTop(ellipse, position.Y);
      }

      private void GenerateDots()
      {         
         for (int i = 0; i < this.DotCount; i++)
         {            
            var ellipse = this.CreateEllipse(i);
            
            this.container.Children.Add(ellipse);            
         }
      }

      private void CreateAnimation()
      {         
         this.rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;
         this.rotateAnimation.SpeedRatio = this.Speed;
         if (this.Direction == RotateDirection.CCW)
         {
            this.rotateAnimation.To *= -1;
         }

         Storyboard.SetTarget(this.rotateAnimation, this.container);
         Storyboard.SetTargetProperty(this.rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));         
                  
         this.storyBoard.Children.Add(this.rotateAnimation);         
      }
   }
}
