using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeProduction
{
    public interface IRemoveSelf
    {
        void RemoveSelf();
        event EventHandler OnRemoveSelf;
    }
}
