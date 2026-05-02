using System;

namespace Utils
{
    public class BindableProperty<T> {
        private T _value;
        public T Value {
            get => _value;
            set {
                if (Equals(_value, value)) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
        public Action<T> OnValueChanged;

        public BindableProperty() { }
        public BindableProperty(T value)
        {
            _value = value;
        }
        
    }
}