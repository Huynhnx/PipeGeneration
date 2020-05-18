using MVVMCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeGeneration.ViewModel
{
    public class Profile: ViewModelBase
    {
        public Profile(string filepath,string name)
        {
            _filepath = filepath;
            _displayName = name;
        }
        string _filepath;
        public string filepath
        {
            get
            {
                return _filepath;
            }
            set
            {
                if (value!= _filepath)
                {
                    _filepath = value;
                    RaisePropertyChanged("filepath");
                }
            }
        }
        string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (value != _displayName)
                {
                    _displayName = value;
                    RaisePropertyChanged("DisplayName");
                }
            }
        }
        public double pipeSize { get; set; }
        public double Offset { get; set; }
        public double CoverThickness { get; set; }
        private bool isCheck;
        public bool IsCheck
        {
            get { return isCheck; }
            set
            {
                if (value!= isCheck)
                {
                    isCheck = value;
                    RaisePropertyChanged("IsCheck");
                }
            }
        }

    }
    class SettingViewModel: ViewModelBase
    {
        public SettingViewModel()
        {
            Profiles = new ObservableCollection<Profile>();
            _profileSelected = new Profile("","Pipe");
            Profiles.Add(_profileSelected);
        }
        private ObservableCollection<Profile> _profiles { get; set; }
        public ObservableCollection<Profile> Profiles
        {
            get
            {
                return _profiles;
            }
            set
            {
                if (value!= _profiles)
                {
                    _profiles = value;
                    RaisePropertyChanged("Profiles");
                }
            }
        }
        private Profile _profileSelected;
        public Profile ProfileSelected
        {
            get
            {
                return _profileSelected;
            }
            set
            {
                if (_profileSelected != value)
                {
                    _profileSelected = value;
                    RaisePropertyChanged("ProfileSelected");
                }
            }
        }
        public RelayCommand routingfromLinesCmd { get; set; }
        public RelayCommand SelectionChange { get; set; }
    }
}
