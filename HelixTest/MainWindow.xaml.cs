using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using PropertyChanged;
using SharpDX.DXGI;
using Control = System.Windows.Controls.Control;
using System.Windows.Threading;

namespace HelixTest
{
    /// <summary>
    /// Interaction logic for appUI.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        public Viewport3DX viewport3d
        {
            get { return _viewport3d; } //defined in xaml
        }
        internal mainViewModel viewModel { get; private set; }

        /// <summary>
        /// Constructor for WPF window.
        /// </summary>
        public MainWindow()
        {
            viewModel = new mainViewModel();
            DataContext = viewModel;


            InitializeComponent();
        }

        //private void requestPanelUpdateHandler()
        //{
        //    if (CheckAccess())
        //    {
        //        //viewModel.updatePanelProperties();
        //        Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(o => viewModel.updateProperties));
        //    }
        //    //else
        //    //{
        //    //    Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => viewModel.updatePanelProperties()));
        //    //}
        //}

    }
}