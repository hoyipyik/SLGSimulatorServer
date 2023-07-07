using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLGSimulatorServer
{
    public static class Service
    {
        public enum MessageCode : byte
        {
            signup = 0,
            login = 1,
            attack = 2,
            upgradeInfo = 3,
        }

        public enum SignupData: byte
        {
            username = 0,
            password = 1,
            confirmPassword = 2,
        }

        public enum SignupResponse: byte
        {
            status = 0,
            message = 1,
        }

        public static OperationResponse SignupHanlder(Dictionary<byte, object> param)
        {
            string username = (string)param[(byte)SignupData.username];
            string password = (string)param[(byte)SignupData.password];
            string confirmPassword = (string)param[(byte)SignupData.confirmPassword];
            List<KeyValuePair<string, object>> insertData = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("username", username),
                new KeyValuePair<string, object>("password", password),
                new KeyValuePair<string, object>("soldierNum", 0)
            };

            if (password == confirmPassword)
            {
                DatabaseHandler db = new DatabaseHandler();
                bool flag;
                flag = db.ConnectionTest();
                if (!flag)
                {
                    Utils.ErrorHandler(flag, "Database connection failed");
                    return new OperationResponse((byte)MessageCode.signup, new Dictionary<byte, object>
                    {
                        { (byte)SignupResponse.status, false },
                        { (byte)SignupResponse.message, "Database connection failed" }
                    });
                }
                flag = db.InsertData(Utils.databaseName, Utils.tableName, insertData.ToArray());
                if (!flag)
                {
                    Utils.ErrorHandler(flag, "Insert data failed");
                    return new OperationResponse((byte)MessageCode.signup, new Dictionary<byte, object>
                    {
                        { (byte)SignupResponse.status, false },
                        { (byte)SignupResponse.message, "Insert data failed" }
                    });
                }
                return new OperationResponse((byte)MessageCode.signup, new Dictionary<byte, object>
                {
                    { (byte)SignupResponse.status, true },
                    { (byte)SignupResponse.message, "Signup success" }
                });
            }
            else
            {
                return new OperationResponse((byte)MessageCode.signup, new Dictionary<byte, object>
                {
                    { (byte)SignupResponse.status, false },
                    { (byte)SignupResponse.message, "Password and confirm password are not the same" }
                });
            }
        }

        public static void LoginHandler(Dictionary<byte, object> param)
        {

        }

        public static void AttackHandler(Dictionary<byte, object> param)
        {

        }

        public static void UpgradeInfoHandler(Dictionary<byte, object> param)
        {

        }
    }
}
