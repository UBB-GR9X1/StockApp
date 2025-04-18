using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.ViewModels;

namespace StockApp.ViewModels.Tests
{
    public class TestViewModel : ViewModelBase
    {
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private int _count;
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
    }

    [TestClass]
    public class ViewModelBaseTests
    {
        private TestViewModel _vm;
        private string? _lastRaisedProperty;

        [TestInitialize]
        public void Init()
        {
            _vm = new TestViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _lastRaisedProperty = e.PropertyName;
        }

        [TestMethod]
        public void SetProperty_NewValue_RaisesPropertyChanged()
        {
            _lastRaisedProperty = null;

            _vm.Text = "hello";

            Assert.AreEqual("Text", _lastRaisedProperty);
            Assert.AreEqual("hello", _vm.Text);
        }

        [TestMethod]
        public void SetProperty_SameValue_DoesNotRaisePropertyChanged()
        {
            _vm.Text = "same";
            _lastRaisedProperty = null;

            _vm.Text = "same";

            Assert.IsNull(_lastRaisedProperty);
        }

        [TestMethod]
        public void SetProperty_ReturnsTrueOnChangeAndFalseOtherwise()
        {
            int backing = 1;
            bool raised;

            raised = typeof(ViewModelBase)
                .GetMethod("SetProperty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .MakeGenericMethod(typeof(int))
                .Invoke(_vm, new object[] { backing, 2, "Count" }) as bool? == true;

            Assert.IsTrue(raised);

            raised = (bool)typeof(ViewModelBase)
                .GetMethod("SetProperty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .MakeGenericMethod(typeof(int))
                .Invoke(_vm, new object[] { 2, 2, "Count" })!;

            Assert.IsFalse(raised);
        }

        [TestMethod]
        public void OnPropertyChanged_CanRaiseArbitraryName()
        {
            _lastRaisedProperty = null;

            typeof(ViewModelBase)
                .GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .Invoke(_vm, new object[] { "CustomProp" });

            Assert.AreEqual("CustomProp", _lastRaisedProperty);
        }
    }
}