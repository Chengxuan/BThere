﻿#pragma checksum "F:\Downloads\WindowsPhone\WindowsPhone\ARSampleApp\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "857FDD537EB82B2CE498501A50AE8A90"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using SharpGIS.AR.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ARSampleApp {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Shapes.Rectangle videoRectangle;
        
        internal System.Windows.Media.VideoBrush viewfinderBrush;
        
        internal System.Windows.Media.CompositeTransform videoBrushTransform;
        
        internal SharpGIS.AR.Controls.ARPanel arPanel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/ARSampleApp;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.videoRectangle = ((System.Windows.Shapes.Rectangle)(this.FindName("videoRectangle")));
            this.viewfinderBrush = ((System.Windows.Media.VideoBrush)(this.FindName("viewfinderBrush")));
            this.videoBrushTransform = ((System.Windows.Media.CompositeTransform)(this.FindName("videoBrushTransform")));
            this.arPanel = ((SharpGIS.AR.Controls.ARPanel)(this.FindName("arPanel")));
        }
    }
}

