using System;
using System.Collections.Generic;

public class ModelProperty<T>
{
    private T _property;
    public T Property
    {
        get { return _property; }
        set
        {
            if (Compare(_property,value)) return;
            _property = value;
            PropertyChanged?.Invoke(_property);
        }
    }
    
    public event Action<T> PropertyChanged;
    
    private bool Compare(T x, T y)
    {
        return EqualityComparer<T>.Default.Equals(x, y);
    }
}
