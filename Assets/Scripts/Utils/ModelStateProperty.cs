
using System.Collections.Generic;

public class ModelStateProperty<T> : ModelProperty<T>
{
    public override T Property
    {
        get { return PropertyValue; }
        set
        {
            if (Compare(PropertyValue, value)) return;
            PropertyValue = value;
            PropertyChanged?.Invoke(PropertyValue);
        }
    }
    
    private bool Compare(T x, T y)
    {
        return EqualityComparer<T>.Default.Equals(x, y);
    }
}
