using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core.View
{
    public interface IStartPanel
    {
        event EventHandler StartButtonClickedEvent;
        string GetAppKey();
        string GetSecretKey();
    }
}
