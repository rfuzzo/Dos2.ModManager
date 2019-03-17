using Dos2.ModManager.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ninject;
using Ninject.Infrastructure;

namespace Dos2.ModManager.ViewModels
{
    public class DockableViewModel : ViewModel
    {
        #region Title
        private string _title;
        public virtual string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if(_title != value)
                {
                    _title = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion

        #region ContentId
        private string _contentId;
        public string ContentId
        {
            get
            {
                return _contentId;
            }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion

        #region IsActive
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion

        #region ViewModel
        private MainViewModel _parentViewModel;
        public MainViewModel ParentViewModel
        {
            get
            {
                return _parentViewModel;
            }
            set
            {
                if (_parentViewModel != value)
                {
                    _parentViewModel = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion

        public ICommand SelectCommand { get; }

        public DockableViewModel()
        {
            

            SelectCommand = new RelayCommand(() => IsActive = true);
        }

        
    }
}