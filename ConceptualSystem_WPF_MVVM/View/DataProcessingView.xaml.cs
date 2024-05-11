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
using System.Windows.Shapes;

namespace ConceptualSystem_WPF_MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для DataProcessingView.xaml
    /// </summary>
    public partial class DataProcessingView : Window
    {
        private static DataProcessingView instance;
        public DataProcessingView()
        {
            InitializeComponent();
        }
        public static DataProcessingView GetInstance()
        {
            if (instance == null)
            {
                instance = new DataProcessingView();
            }
            return instance;
        }
        public static void CloseInstance()
        {
            if (instance != null)
            {
                instance.Close();
                instance = null;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
