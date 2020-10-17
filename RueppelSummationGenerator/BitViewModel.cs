using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SummationGenerator
{
    class BitViewModel
    {

        public BitViewModel()
        {
            Bits = new ObservableCollection<Bit>();
        }

        public ObservableCollection<Bit> Bits { get; set; }
    }
}
