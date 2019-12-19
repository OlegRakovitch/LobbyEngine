using System;

namespace RattusEngine
{
    public interface IContext
    {
        IStorage Storage { get; }
        User GetUser();
    }
}