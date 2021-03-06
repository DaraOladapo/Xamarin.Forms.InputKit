﻿using Plugin.InputKit.Shared.Abstraction;
using Plugin.InputKit.Shared.Configuration;
using System;
using Xamarin.Forms;

namespace Plugin.InputKit.Shared.Controls
{
    public class AdvancedSlider : StackLayout, IValidatable
    {
        public static GlobalSetting GlobalSetting { get; private set; } = new GlobalSetting
        {
            FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)),
            TextColor = (Color)Label.TextColorProperty.DefaultValue,
            Color = Color.Accent,
        };
        Slider slider = new Slider { ThumbColor = GlobalSetting.Color, MinimumTrackColor = GlobalSetting.Color };
        Label lblTitle = new Label { FontSize = GlobalSetting.FontSize, FontFamily = GlobalSetting.FontFamily, Margin = new Thickness(20, 0), InputTransparent = true, FontAttributes = FontAttributes.Bold, TextColor = GlobalSetting.TextColor, };
        Label lblValue = new Label { FontSize = GlobalSetting.FontSize, FontFamily = GlobalSetting.FontFamily, InputTransparent = true, TextColor = GlobalSetting.TextColor, };
        Label lblMinValue = new Label { FontSize = GlobalSetting.FontSize, FontFamily = GlobalSetting.FontFamily, TextColor = GlobalSetting.TextColor, };
        Label lblMaxValue = new Label { FontSize = GlobalSetting.FontSize, FontFamily = GlobalSetting.FontFamily, TextColor = GlobalSetting.TextColor, };
        private string _valueSuffix;
        private string _valuePrefix;
        private Color _textColor;
        private double _stepValue = 1;

        public AdvancedSlider()
        {
            this.Children.Add(lblTitle);
            this.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    lblMinValue,
                    new Grid
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Margin = new Thickness(0,30,0,0),
                        Children =
                        {
                            slider,
                            lblValue,
                        }
                    },
                    lblMaxValue,
                }
            });

            slider.ValueChanged += Slider_ValueChanged;
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            //if (e.NewValue == e.OldValue || e.NewValue == slider.Value) return;


            if (e.NewValue % StepValue != 0)
            {
                slider.Value = Math.Round(e.NewValue / StepValue) * StepValue;
                return;
            }
            SetValue(ValueProperty, slider.Value);
            UpdateValueText();
            UpdateView();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateValueText();
            UpdateView();
            UpdateMinMaxValueText();
        }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Value of slider which user selected
        /// </summary>
        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty,value); }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Title of slider, It'll be shown tp of slider
        /// </summary>
        public string Title { get => lblTitle.Text; set { lblTitle.Text = value; lblTitle.IsVisible = !String.IsNullOrEmpty(value); } }
        ///---------------------------------------------------------------------
        /// <summary>
        /// It will be displayed start of value 
        /// </summary>
        public string ValueSuffix { get => _valueSuffix; set { _valueSuffix = value; UpdateValueText(); } }
        ///---------------------------------------------------------------------
        /// <summary>
        /// It'll be displayed end of value
        /// </summary>
        public string ValuePrefix { get => _valuePrefix; set { _valuePrefix = value; UpdateValueText(); } }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Minimum value, user can slide
        /// </summary>
        public double MinValue { get => slider.Minimum; set { slider.Minimum = value; UpdateMinMaxValueText(); } }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Maximum value, user can slide
        /// </summary>
        public double MaxValue { get => slider.Maximum; set { slider.Maximum = value; UpdateMinMaxValueText(); } }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Slider Increase number
        /// </summary>
        public double StepValue { get => _stepValue; set { _stepValue = value; UpdateValueText(); UpdateView(); } }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Visibility of Min value and Max value at right and left
        /// </summary>
        public bool DisplayMinMaxValue
        {
            get => lblMinValue.IsVisible && lblMaxValue.IsVisible;

            set { lblMaxValue.IsVisible = value; lblMinValue.IsVisible = value; }
        }
        ///---------------------------------------------------------------------
        /// <summary>
        /// Text color of labels
        /// </summary>
        public Color TextColor
        {
            get => _textColor; set
            {
                _textColor = value;
                lblMaxValue.TextColor = value;
                lblMinValue.TextColor = value;
                lblTitle.TextColor = value;
                lblValue.TextColor = value;
            }
        }
        ///---------------------------------------------------------------------
        /// <summary>
        /// This is not available for this control
        /// </summary>
        public bool IsRequired { get; set; }
        ///---------------------------------------------------------------------
        /// <summary>
        /// this always true, because this control value can not be null
        /// </summary>
        public bool IsValidated => true;
        ///---------------------------------------------------------------------
        /// <summary>
        /// It's not available for this control
        /// </summary>
        public string ValidationMessage { get; set; }

        #region BindableProperties
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(AdvancedSlider), 0.0, BindingMode.TwoWay, propertyChanged: (bo, ov, nv) => (bo as AdvancedSlider).slider.Value = (double)nv);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(AdvancedSlider), Color.Gray, propertyChanged: (bo, ov, nv) => (bo as AdvancedSlider).TextColor = (Color)nv);
        #endregion
        ///---------------------------------------------------------------------
        /// <summary>
        /// doesn't effect
        /// </summary>
        public event EventHandler ValidationChanged;

        ///---------------------------------------------------------------------
        /// <summary>
        /// It's not available for this control
        /// </summary>
        public void DisplayValidation() { }
        void UpdateMinMaxValueText()
        {
            lblMinValue.Text = this.MinValue.ToString();
            lblMaxValue.Text = this.MaxValue.ToString() + "+ ";
        }
        void UpdateValueText()
        {
            lblValue.Text = $"{this.ValuePrefix} {this.Value} {this.ValueSuffix}";
        }
        void UpdateView()
        {
            lblValue.TranslateTo(
                this.Value * ((slider.Width - 40) / this.MaxValue), //pos X
                slider.TranslationY - lblValue.Height * 0.9, //pos Y
                40 //Latency
                );
            //lblValue.LayoutTo(new Rectangle(new Point(pos, slider.Y + lblValue.Height * 0.8), new Size(lblValue.Width, lblValue.Height)));
        }
    }
}
