using Web.Domain.Controllers;
using Web.Domain.nWebGraph.nSessionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nGetCommandListCommand
{
    public interface IGetCommandListReceiver : ICommandReceiver
    {
        void ReceiveGetCommandListData(cListenerEvent _ListenerEvent, IController _Controller, cGetCommandListCommandData _ReceivedData);
    }
}
