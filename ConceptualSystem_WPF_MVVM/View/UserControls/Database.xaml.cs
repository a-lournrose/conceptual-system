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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Logic;

namespace ConceptualSystem_WPF_MVVM.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Database.xaml
    /// </summary>
    public partial class Database : UserControl
    {
        private readonly ConceptsRepository _conceptsRepository = new ConceptsRepository();
        public Database()
        {
            InitializeComponent();
        }
        
    }
}
