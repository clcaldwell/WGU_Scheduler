using System.Windows.Controls;

using Scheduler.ViewModel;

namespace Scheduler.View
{
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            this.DataContext = new MenuViewModel();
            InitializeComponent();
        }
    }
}
