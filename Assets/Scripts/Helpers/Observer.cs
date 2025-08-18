using System;

namespace Helpers
{
    public class Observer<T>
    {
        private T _value;
        public T Value
        {
            get => _value;

            set
            {
                OnValueChanging?.Invoke(value);
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public Observer() => Value = default;
        
        public event Action<T> OnValueChanging;
        public event Action<T> OnValueChanged;
        
        public static implicit operator T(Observer<T> e) => e.Value;
    }
}