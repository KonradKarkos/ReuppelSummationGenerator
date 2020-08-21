using System;
using System.ComponentModel;

namespace SummationGenerator
{
    public class Bit : INotifyPropertyChanged
    {
        private string bitName { get; set; }
        private string bitState { get; set; }
        private int bitValue { get; set; }
        public string BitName
        {
            get
            {
                return bitName;
            }
            set
            {
                bitName = value;
                OnPropertyChanged("BitName");
            }
        }
        public string BitState
        {
            get
            {
                return bitState;
            }
            set
            {
                bitState = value;
                OnPropertyChanged("BitState");
            }
        }
        public int BitValue
        {
            get
            {
                return bitValue;
            }
            set
            {
                bitValue = value;
                OnPropertyChanged("BitValue");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
