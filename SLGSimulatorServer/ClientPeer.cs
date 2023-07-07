using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLGSimulatorServer
{
    public class ClientPeer : Photon.SocketServer.ClientPeer
    {

        public ClientPeer(InitRequest initRequest) : base(initRequest)
        {

        }
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Utils.log.Info("Client disconnected. " + reasonCode + " " + reasonDetail);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case (byte)Service.MessageCode.signup:
                    OperationResponse res = Service.SignupHanlder(operationRequest.Parameters);
                    SendOperationResponse(res, sendParameters);
                    break;
                case (byte)Service.MessageCode.login:
                    Service.LoginHandler(operationRequest.Parameters);
                    break;
                case (byte)Service.MessageCode.attack:
                    Service.AttackHandler(operationRequest.Parameters);
                    break;
                case (byte)Service.MessageCode.upgradeInfo:
                    Service.UpgradeInfoHandler(operationRequest.Parameters);
                    break;
                default:
                    break;
            }
        }
    }
}
