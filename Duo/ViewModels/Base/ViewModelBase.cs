using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Duo.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// Event raised when a property on this object has a new value.
        public event PropertyChangedEventHandler? PropertyChanged;

        /// Raises the PropertyChanged event for the specified property.
        /// <param name="propertyName">
        /// The name of the property that changed. 
        /// This parameter is optional and can be provided automatically when invoked from compilers that support CallerMemberName.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// Sets a property's value and raises the PropertyChanged event if the value has changed.
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyName">
        /// The name of the property. This parameter is optional and can be provided automatically when invoked from compilers that support CallerMemberName.
        /// </param>
        /// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// Sets a property's value and raises the PropertyChanged event if the value has changed.
        /// Also allows for an action to be executed after the property is set.
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="onChanged">Action to be executed after the property value has been changed.</param>
        /// <param name="propertyName">
        /// The name of the property. This parameter is optional and can be provided automatically when invoked from compilers that support CallerMemberName.
        /// </param>
        /// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
