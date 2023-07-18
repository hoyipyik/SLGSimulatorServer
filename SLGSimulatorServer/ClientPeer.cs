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
            // remove relevant peer from peer list
            SLGSimulatorServer.PeerList.Remove(this);
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
                    //System.Threading.Thread.Sleep(5000);
                    // send upgrade event to client
                    EventData eventData = Service.UpgradeTrigger();
                    //SendEvent(eventData, new SendParameters());
                    SendEventToAll(eventData);
                    break;
                case (byte)Service.MessageCode.upgradeInfo:
                    res = Service.UpgradeInfoHandler(operationRequest.Parameters);
                    SendOperationResponse(res, sendParameters);
                    EventData newEventData = Service.UpgradeTrigger();
                    //SendEvent(newEventData, new SendParameters());
                    SendEventToAll(newEventData);
                    break;
                default:
                    break;
            }
        }

        private void SendEventToAll(EventData eventData)
        {
            Utils.log.Info("Send event to all "+ SLGSimulatorServer.PeerList.ToArray().Length);
            foreach (var peer in SLGSimulatorServer.PeerList)
            {
                peer.SendEvent(eventData, new SendParameters());
            }
        }
    }
}
