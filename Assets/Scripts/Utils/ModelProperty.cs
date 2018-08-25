using System;

public class ModelProperty<T>
{
    protected T PropertyValue;
    public virtual T Property
    {
        get { return PropertyValue; }
        set
        {
            PropertyValue = value;
            PropertyChanged?.Invoke(PropertyValue);
        }
    }
    
    public Action<T> PropertyChanged;
}
