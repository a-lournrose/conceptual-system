using ConceptualSystem_WPF_MVVM.View;
using ConceptualSystem_WPF_MVVM.View.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConceptualSystem_WPF_MVVM.ViewModels
{ 
    class MainViewModel : ViewModelBase
    {
        private UserControl _currentContent;

        public UserControl CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                OnPropertyChanged("CurrentContent");
            }
        }

        public ICommand ChangeContentCommand { get; private set; }

        public MainViewModel()
        {
            ChangeContentCommand = new ViewModelCommand(ChangeContent);
            ChangeContent("Concepts");
        }

        private void ChangeContent(object parameter)
        {
            switch (parameter as string)
            {
                case "Concepts":
                    CurrentContent = new Concepts();
                    DataProcessingView.CloseInstance();
                    break;
                case "Database":
                    CurrentContent = new Database();
                    DataProcessingView dataProcessingView = DataProcessingView.GetInstance();
                    if (!dataProcessingView.IsVisible)
                    {
                        dataProcessingView.Show();
                    }
                    break;
            }
        }
    }
}
