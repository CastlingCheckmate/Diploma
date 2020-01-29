using System.Windows;
using System.Windows.Input;

using Diploma.UI.Auxiliary.MessageBox;
using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.ViewModels.Base;

namespace Diploma.UI.ViewModels.Windows
{

    internal sealed class MessageBoxViewModel : WindowViewModel
    {

        private readonly MessageBoxTypes _type;
        private readonly string _header;
        private readonly string _text;
        private readonly MessageBoxButtons _buttons;
        private MessageBoxResult _result;

        private ICommand _okButtonCommand;
        private ICommand _cancelButtonCommand;
        private ICommand _yesButtonCommand;
        private ICommand _noButtonCommand;
        private ICommand _closeButtonCommand;
        private ICommand _okButtonOrYesButtonMultiCommand;

        public MessageBoxViewModel(MessageBoxTypes type, string header, string text, MessageBoxButtons buttons)
        {
            _type = type;
            _header = header;
            _text = text;
            _buttons = buttons;
        }

        public override void OnBind()
        {
            base.OnBind();
            NotifyPropertyChanged(nameof(Type), nameof(Header), nameof(Text));
            View?.Focus();
        }

        public ICommand OkButtonCommand =>
            _okButtonCommand ?? (_okButtonCommand = new RelayCommand(_ => OkButtonClick(), _ => IsOkButtonVisible));

        public ICommand CancelButtonCommand =>
            _cancelButtonCommand ?? (_cancelButtonCommand = new RelayCommand(_ => CancelButtonClick(), _ => IsCancelButtonVisible));

        public ICommand YesButtonCommand =>
            _yesButtonCommand ?? (_yesButtonCommand = new RelayCommand(_ => YesButtonClick(), _ => IsYesButtonVisible));

        public ICommand NoButtonCommand =>
            _noButtonCommand ?? (_noButtonCommand = new RelayCommand(_ => NoButtonClick(), _ => IsNoButtonVisible));

        public ICommand CloseButtonCommand =>
            _closeButtonCommand ?? (_closeButtonCommand = new RelayCommand(_ => CloseButtonClick()));

        public ICommand OkButtonOrYesButtonMultiCommand =>
            _okButtonOrYesButtonMultiCommand ?? (_okButtonOrYesButtonMultiCommand = new RelayMultiCommand(
                RelayMultiCommandModes.WhenAnyCanExecute,
                new (RelayCommand, object)[]
                {
                    (new RelayCommand(_ => OkButtonClick(), _ => IsOkButtonVisible), null),
                    (new RelayCommand(_ => YesButtonClick(), _ => IsYesButtonVisible), null)
                }));

        public MessageBoxTypes Type =>
            _type;

        public string Header =>
            _header;

        public string Text =>
            _text;

        public bool IsOkButtonVisible =>
            _buttons == MessageBoxButtons.Ok || _buttons == MessageBoxButtons.OkCancel;

        public bool IsCancelButtonVisible =>
            _buttons == MessageBoxButtons.OkCancel;

        public bool IsYesButtonVisible =>
            _buttons == MessageBoxButtons.YesNo;

        public bool IsNoButtonVisible =>
            _buttons == MessageBoxButtons.YesNo;

        public MessageBoxResult Result =>
            _result;

        private void OkButtonClick()
        {
            _result = MessageBoxResult.OK;
            View?.Close();
        }

        private void CancelButtonClick()
        {
            _result = MessageBoxResult.Cancel;
            View?.Close();
        }

        private void YesButtonClick()
        {
            _result = MessageBoxResult.Yes;
            View?.Close();
        }

        private void NoButtonClick()
        {
            _result = MessageBoxResult.No;
            View?.Close();
        }

        private void CloseButtonClick()
        {
            _result = MessageBoxResult.None;
            View?.Close();
        }

    }

}