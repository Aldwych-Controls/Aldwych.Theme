using System;
using System.Windows.Input;
using ReactiveUI;
namespace SampleApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand FakeCommand { get; set; }

        public MainWindowViewModel()
        {
            FakeCommand = ReactiveUI.ReactiveCommand.Create(() => { Console.WriteLine("Clicked"); });
        }
    }
}
