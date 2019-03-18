using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

namespace Dos2.ModManager
{
    /// <summary>
    /// Represents an abstract object that provides notifications when properties are changed or are changing.
    /// Implements <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/>
    /// </summary>
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Invoke the PropertyChanged event using the caller property name with <see cref="CallerMemberNameAttribute"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected virtual void InvokePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region NotifyPropertyChanging
        public event PropertyChangingEventHandler PropertyChanging;
        /// <summary>
        /// Invoke the PropertyChanging event using the caller property name with <see cref="CallerMemberNameAttribute"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        protected virtual void InvokePropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        #endregion

        #region Explicit Methods
        // Not a fan of this style
        protected virtual bool ChangeProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if(EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            InvokePropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}