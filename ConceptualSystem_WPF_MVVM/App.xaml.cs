using System.Configuration;
using System.Data;
using System.Windows;
using Logic;

namespace ConceptualSystem_WPF_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App ()
        {
            var conceptTableCreator = new ConceptTableCreator();
            conceptTableCreator.Migrate();
            
        }
    }

}
