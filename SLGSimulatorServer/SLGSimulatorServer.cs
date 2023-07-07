using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using ExitGames.Logging;
using System.IO;
using ExitGames.Logging.Log4Net;
using log4net.Config;

namespace SLGSimulatorServer
{
    public class SLGSimulatorServer : ApplicationBase
    {
       

        protected override void Setup()
        {
            Utils.LoggerInitial(this.ApplicationRootPath, this.BinaryPath);
            Utils.DatabaseInitial();
            Utils.log.Info("Set up complete *********");
        }

        protected override void TearDown()
        {
            Utils.log.Info("Server stopped");
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            Utils.log.Info("New peer added ! " + initRequest);
            return new ClientPeer(initRequest);
        }
    }
}
