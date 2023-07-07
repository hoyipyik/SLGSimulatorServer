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
            OperationResponse res;
            switch (operationRequest.OperationCode)
            {
                case (byte)Service.MessageCode.signup:
                    res = Service.SignupHanlder(operationRequest.Parameters);
                    SendOperationResponse(res, sendParameters);
                    break;
                case (byte)Service.MessageCode.login:
                    res = Service.LoginHandler(operationRequest.Parameters);
                    SendOperationResponse(res, sendParameters);
                    break;
                case (byte)Service.MessageCode.attack:
                    res = Service.AttackHandler(operationRequest.Parameters);
                    SendOperationResponse(res, sendParameters);
                    // await for 5 seconds to simulate the attack
                    System.Threading.Thread.Sleep(5000);
                    // send upgrade event to client
                    EventData eventData = Service.UpgradeTrigger();
                    SendEvent(eventData, new SendParameters());
                    break;
                case (byte)Service.MessageCode.upgradeInfo:
                    res = Service.UpgradeInfoHandler(operationRequest.Parameters);
                    SendOperationResponse(res, sendParameters);
                    break;
                default:
                    break;
            }
        }
    }
}
