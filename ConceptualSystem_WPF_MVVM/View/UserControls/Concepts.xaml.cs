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

namespace ConceptualSystem_WPF_MVVM.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Concepts.xaml
    /// </summary>
    public partial class Concepts : UserControl
    {
        public Concepts()
        {
            InitializeComponent();

            var conceptsWords = new List<string>
            {
                "Паспорт",
                "Человек",
                "Компьютер",
                "Ноутбук",
            };
            conceptsList.ItemsSource = conceptsWords;

            var structureConceptWords = new List<string>
            {
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
                "Фамилия",
                "Имя",
                "Отчество",
                "Возраст",
            };
            structureConceptList.ItemsSource = structureConceptWords;

        }
    }
}
