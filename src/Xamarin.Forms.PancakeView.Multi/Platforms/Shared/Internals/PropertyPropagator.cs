using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms.PancakeView
{
    public class PropertyPropagator
    {
        string[] listenedProperties = new string[0];
        Action<string> propertyChangedNotifier = null;
        BindableObject propagationRootObject = null;
        List<KeyValuePair<string, object>> propagationProperties = new List<KeyValuePair<string, object>>();

        public PropertyPropagator(BindableObject bindableObject, Action<string> onPropertyChangedMethod, params string[] propertyToListenTo)
        {
            propertyChangedNotifier = onPropertyChangedMethod;
            propagationRootObject = bindableObject;
            listenedProperties = propertyToListenTo ?? listenedProperties;
        }

        public void AddPropertyToListenTo(params string[] propertyName)
        {
            listenedProperties = listenedProperties.Union(propertyName).ToArray();
        }

        public T GetValue<T>(BindableProperty property)
        {
            var value = propagationRootObject?.GetValue(property);

            if (value != null)
            {
                INotifyPropertyChanged bindableSubObject = (value as INotifyPropertyChanged);

                if (bindableSubObject != null)
                {
                    bindableSubObject.PropertyChanged -= PropagatorListener;
                    bindableSubObject.PropertyChanged += PropagatorListener;

                    if (!propagationProperties.Any(a => a.Key == property.PropertyName))
                        propagationProperties.Add(new KeyValuePair<string, object>(property.PropertyName, value));
                }
            }

            return (T)value;
        }

        public void SetValue<T>(BindableProperty property, ref T value)
        {
            var oldValue = propagationRootObject?.GetValue(property);

            if (oldValue != null)
            {
                INotifyPropertyChanged bindableSubObject = (value as INotifyPropertyChanged);

                if (bindableSubObject != null)
                    bindableSubObject.PropertyChanged -= PropagatorListener;
            }

            if (value != null)
            {
                INotifyPropertyChanged bindableSubObject = (value as INotifyPropertyChanged);
                if (bindableSubObject != null)
                {
                    bindableSubObject.PropertyChanged += PropagatorListener;

                    propagationProperties.RemoveAll(p => p.Key == property.PropertyName);
                    propagationProperties.Add(new KeyValuePair<string, object>(property.PropertyName, value));
                }
            }

            propagationRootObject.SetValue(property, value);
        }

        private void PropagatorListener(object sender, PropertyChangedEventArgs e)
        {
            if (listenedProperties?.Contains(e.PropertyName) ?? true)
                PropagationThrower(sender);
        }

        private void PropagationThrower(object sender)
        {
            if (propagationProperties.Any(p => p.Value == sender))
            {
                var prop = propagationProperties.FirstOrDefault(p => p.Value == sender);
                propertyChangedNotifier?.Invoke(prop.Key);
            }
        }
    }
}
