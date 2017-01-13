﻿#pragma checksum "..\..\..\Home.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B243A9DC18DF3F069365FE35FFD777CF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace IRProject_GUI {
    
    
    /// <summary>
    /// Home
    /// </summary>
    public partial class Home : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox corpus_path;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox save_path;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox stemming;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox no_stemming;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBox;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button start_button;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ClockTextBlock;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Time;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button reset_button;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock load_path;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Load_btn;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Home.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Display_dic;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/IRProject-GUI;component/home.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Home.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.corpus_path = ((System.Windows.Controls.TextBox)(target));
            
            #line 19 "..\..\..\Home.xaml"
            this.corpus_path.LostFocus += new System.Windows.RoutedEventHandler(this.corpus_path_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 20 "..\..\..\Home.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Browse_Corpuse);
            
            #line default
            #line hidden
            return;
            case 3:
            this.save_path = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\..\Home.xaml"
            this.save_path.LostFocus += new System.Windows.RoutedEventHandler(this.save_path_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 25 "..\..\..\Home.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Browse_Dest);
            
            #line default
            #line hidden
            return;
            case 5:
            this.stemming = ((System.Windows.Controls.CheckBox)(target));
            
            #line 28 "..\..\..\Home.xaml"
            this.stemming.Click += new System.Windows.RoutedEventHandler(this.stemming_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.no_stemming = ((System.Windows.Controls.CheckBox)(target));
            
            #line 29 "..\..\..\Home.xaml"
            this.no_stemming.Click += new System.Windows.RoutedEventHandler(this.nostemming_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.comboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.start_button = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\Home.xaml"
            this.start_button.Click += new System.Windows.RoutedEventHandler(this.Start_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ClockTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.Time = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.reset_button = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\Home.xaml"
            this.reset_button.Click += new System.Windows.RoutedEventHandler(this.Reset_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.load_path = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.Load_btn = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\Home.xaml"
            this.Load_btn.Click += new System.Windows.RoutedEventHandler(this.Load_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.Display_dic = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\Home.xaml"
            this.Display_dic.Click += new System.Windows.RoutedEventHandler(this.Display_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
