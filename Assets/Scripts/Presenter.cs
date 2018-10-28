using System;

public class Presenter<View> : IDisposable
{
    protected View view;

    public virtual void RegisterView(View view)
    {
        this.view = view;
    }

    public virtual void Dispose()
    {
    }
}