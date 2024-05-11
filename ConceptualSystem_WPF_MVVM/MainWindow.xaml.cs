using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConceptualSystem_WPF_MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var db = new ConceptTableCreator();
            db.CreateCreatedTablesTable();
            db.CreateConceptsTable();
            db.CreateCreatedFieldsTable();
          //  db.CreateConceptTable("person", new []{"name", "age" });
            db.CreateConceptTable("study", new []{"class", "grade" }, "person");
            
        }
    }
}
