using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SummationGenerator
{
    class BitViewModel
    {
        private ObservableCollection<Bit> BitList;

        public BitViewModel()
        {
            BitList = new ObservableCollection<Bit>();
        }

        public ObservableCollection<Bit> Bits
        {
            get { return BitList; }
            set { BitList = value; }
        }

        private ICommand mUpdater;
        public ICommand UpdateCommand
        {
            get
            {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set
            {
                mUpdater = value;
            }
        }

        private class Updater : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {

            }
        }
    }
}
