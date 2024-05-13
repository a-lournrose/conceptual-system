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
using Logic.Enums;
using Logic.Models;

namespace ConceptualSystem_WPF_MVVM.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Concepts.xaml
    /// </summary>
    public partial class Concepts : UserControl
    {
        private readonly ConceptsRepository _conceptsRepository = new ConceptsRepository();
        public List<Concept> ConceptsList { get; set; }

        public Concepts()
        {
            InitializeComponent();
            LoadConcepts();
        }

        private async Task LoadConcepts()
        {
            ConceptsList = await _conceptsRepository.GetAllConcepts();
            conceptsList.ItemsSource = ConceptsList.Select(x => x.view_name);
            ExistsConceptsComboBox.ItemsSource = ConceptsList.Select(x => x.view_name);
        }

        private async void CreateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var name = ConceptName.Text;
            var type = (RelationType)ConceptRelationType.SelectedIndex;
            var relatedConcept = ExistsConceptsComboBox.SelectedItem as string;
            var relatedConceptId = ConceptsList.FirstOrDefault(x => x.view_name == relatedConcept)?.id;
            await _conceptsRepository.CreateRelationConcepts(name, type, relatedConceptId);
            LoadConcepts();
        }

        private void ConceptsList_OnSelected(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
