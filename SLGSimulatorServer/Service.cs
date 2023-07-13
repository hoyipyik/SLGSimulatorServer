using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

        public enum SignupData : byte
        {
            username = 0,
            password = 1,
            confirmPassword = 2,
        }

        public enum SignupResponse : byte
        {
            status = 0,
            message = 1,
        }

        public enum LoginData : byte
        {
            username = 0,
            password = 1,
        }

        public enum LoginResponse : byte
        {
            status = 0,
            message = 1,
            userData = 2,
            cityData = 3,
            username = 4,
            userId = 5,
            soldierNum = 6,
        }

        public enum AttackData : byte
        {
            userId = 0,
            targetCityId = 1,
        }

        public enum AttackResponse : byte
        {
            status = 0,
            message = 1,
        }

        public enum upgradeTriggerResponse: byte
        {
            status = 0,
            message = 1,
            playerData = 2,
            cityData = 3,
        }

        public enum UpgradeInfoData : byte
        {
            userId = 0,
            username = 1,
            soldierNum = 2,
        }

        public enum UpgradeInfoResponse : byte
        {
            status = 0,
            message = 1,
        }

        private static OperationResponse GenerateSignupResponse(bool status, string message)
        {
            return new OperationResponse((byte)MessageCode.signup, new Dictionary<byte, object>
            {
                { (byte)SignupResponse.status, status },
                { (byte)SignupResponse.message, message }
            });
        }

        private static OperationResponse GenerateUpgradeInfoResponse(bool status, string message)
        {
            return new OperationResponse((byte)MessageCode.upgradeInfo, new Dictionary<byte, object>
            {
                { (byte)UpgradeInfoResponse.status, status },
                { (byte)UpgradeInfoResponse.message, message }
            });
        }

        private static OperationResponse GenerateAttackResponse(bool status, string message)
        {
            return new OperationResponse((byte)MessageCode.attack, new Dictionary<byte, object>
            {
                { (byte)AttackResponse.status, status },
                { (byte)AttackResponse.message, message }
            });
        }

        private static OperationResponse GenerateLoginResponse(bool status, string message,
            List<Dictionary<string, object>> userData = null, List<Dictionary<string, object>> cityData = null,
            string username = null, int userId = -1, int soldierNum = 0)
        {
            return new OperationResponse((byte)MessageCode.login, new Dictionary<byte, object>
            {
                { (byte)LoginResponse.status, status },
                { (byte)LoginResponse.message, message },
                { (byte)LoginResponse.userData, userData },
                { (byte)LoginResponse.cityData, cityData },
                { (byte)LoginResponse.username, username },
                { (byte)LoginResponse.userId, userId },
                { (byte)LoginResponse.soldierNum, soldierNum }
            });
        }

        private static EventData GenerateEventData(bool status, string message, List<Dictionary<string, object>> playerData = null, List<Dictionary<string, object>> cityData = null)
        {
            return new EventData((byte)MessageCode.attack, new Dictionary<byte, object>
            {
                { (byte)upgradeTriggerResponse.status, status },
                { (byte)upgradeTriggerResponse.message, message },
                { (byte)upgradeTriggerResponse.playerData, playerData },
                { (byte)upgradeTriggerResponse.cityData, cityData }
            });
        }

        public static OperationResponse SignupHanlder(Dictionary<byte, object> param)
        {
            string username = (string)param[(byte)SignupData.username];
            string password = (string)param[(byte)SignupData.password];
            string confirmPassword = (string)param[(byte)SignupData.confirmPassword];
            if (username == null || password == null || confirmPassword == null || username == "" || password == "")
            {
                Utils.ErrorHandler(false, "Invalid parameters");
                return GenerateSignupResponse(false, "Invalid parameters");
            }
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
                    return GenerateSignupResponse(false, "Database connection failed");
                }
                List<Dictionary<string, object>> res = db.QueryUniversal(Utils.databaseName, Utils.tableName, "*", "username", username);
                if (res.Count != 0)
                {
                    Utils.ErrorHandler(false, "Username already exists");
                    return GenerateSignupResponse(false, "Username already exists");
                }
                flag = db.InsertData(Utils.databaseName, Utils.tableName, insertData.ToArray());
                if (!flag)
                {
                    Utils.ErrorHandler(flag, "Insert data failed");
                    return GenerateSignupResponse(false, "Insert data failed");
                }
                Utils.ErrorHandler(true, "Signup success");
                return GenerateSignupResponse(true, "Signup success");
            }
            else
            {
                Utils.ErrorHandler(false, "Password not match");
                return GenerateSignupResponse(false, "Password not match");
            }
        }

        public static OperationResponse LoginHandler(Dictionary<byte, object> param)
        {
            string username = (string)param[(byte)LoginData.username];
            string password = (string)param[(byte)LoginData.password];
            if (username == null || password == null)
            {
                Utils.ErrorHandler(false, "Invalid parameters");
                return GenerateLoginResponse(false, "Invalid parameters");
            }
            DatabaseHandler db = new DatabaseHandler();
            bool flag;
            flag = db.ConnectionTest();
            if (!flag)
            {
                Utils.ErrorHandler(flag, "Database connection failed");
                return GenerateLoginResponse(false, "Database connection failed");
            }
            List<Dictionary<string, object>> res = db.QueryUniversal(Utils.databaseName, Utils.tableName, "*", "username", username);
            // null check
            if (res.Count == 0)
            {
                Utils.ErrorHandler(false, "Username not exist");
                return GenerateLoginResponse(false, "Username not exist");
            }
            res[0].TryGetValue("password", out object realPassword);
            int userId = (int)res[0]["id"];
            int soldierNum = (int)res[0]["soldierNum"];
            if (realPassword.ToString() == password)
            {
                List<Dictionary<string, object>> fullUserData = db.QueryAll(Utils.databaseName, Utils.tableName);
                List<Dictionary<string, object>> fullCityData = db.QueryAll(Utils.databaseName, "city");
                Utils.ErrorHandler(true, "Login success");
                //Utils.Printer(fullUserData);
                //Utils.Printer(fullCityData);
                return GenerateLoginResponse(true, "Login success", fullUserData, fullCityData, username, userId, soldierNum);
            }
            else
            {
                Utils.ErrorHandler(false, "Password not match");
                return GenerateLoginResponse(false, "Password not match");
            }
        }

        public static OperationResponse AttackHandler(Dictionary<byte, object> param)
        {
            int userId = (int)param[(byte)AttackData.userId];
            int targetCityId = (int)param[(byte)AttackData.targetCityId];
            if (userId.ToString() == null || targetCityId.ToString() == null)
            {
                Utils.ErrorHandler(false, "Invalid parameters");
                return GenerateAttackResponse(false, "Invalid parameters");
            }
            DatabaseHandler db = new DatabaseHandler();
            bool flag;
            flag = db.ConnectionTest();
            if (!flag)
            {
                Utils.ErrorHandler(flag, "Database connection failed");
                return GenerateAttackResponse(false, "Database connection failed");
            }
            List<Dictionary<string, object>> res = db.QueryUniversal(Utils.databaseName, "city", "*", "id", targetCityId.ToString());
            // res null check
            if (res.Count == 0)
            {
                Utils.ErrorHandler(false, "City not exist");
                return GenerateAttackResponse(false, "City not exist");
            }
            res[0].TryGetValue("userId", out object originalOwnerId);
            // get user soldier number
            List<Dictionary<string, object>> userRes = db.QueryUniversal(Utils.databaseName, Utils.tableName, "*", "id", userId.ToString());
            // null check
            if (userRes.Count == 0)
            {
                Utils.ErrorHandler(false, "User not exist");
                return GenerateAttackResponse(false, "User not exist");
            }
            userRes[0].TryGetValue("soldierNum", out object userSoldierNum);
            userRes[0].TryGetValue("username", out object username);
            if (originalOwnerId == null)
            {
                db.UpgradeUniversal(Utils.databaseName, "city", "id", targetCityId.ToString(), "userId", userId.ToString());
                db.UpgradeUniversal(Utils.databaseName, "city", "id", targetCityId.ToString(), "username", username.ToString());
                Utils.ErrorHandler(true, "Occupy empty city success");
                return GenerateAttackResponse(true, "Occupy empty city success");
            }
            else
            {
                if (originalOwnerId.ToString() == userId.ToString())
                {
                    Utils.ErrorHandler(false, "Cannot attack your own city");
                    return GenerateAttackResponse(false, "Cannot attack your own city");
                }
                // get enemy soldier number
                List<Dictionary<string, object>> enemyRes = db.QueryUniversal(Utils.databaseName, Utils.tableName, "*", "id", originalOwnerId.ToString());
                // null check
                if (enemyRes.Count == 0)
                {
                    Utils.ErrorHandler(false, "Enemy not exist");
                    return GenerateAttackResponse(false, "Enemy not exist");
                }
                enemyRes[0].TryGetValue("soldierNum", out object enemySoldierNum);
                if ((int)userSoldierNum > (int)enemySoldierNum)
                {
                    db.UpgradeUniversal(Utils.databaseName, "city", "id", targetCityId.ToString(), "userId", userId.ToString());
                    db.UpgradeUniversal(Utils.databaseName, "city", "id", targetCityId.ToString(), "username", username.ToString());
                    db.UpgradeUniversal(Utils.databaseName, Utils.tableName, "id", userId.ToString(), "soldierNum", ((int)userSoldierNum - (int)enemySoldierNum).ToString());
                    db.UpgradeUniversal(Utils.databaseName, Utils.tableName, "id", originalOwnerId.ToString(), "soldierNum", 0.ToString());
                    Utils.ErrorHandler(true, "Occupy enemy city success");
                    return GenerateAttackResponse(true, "Occupy enemy city success");
                }
                else
                {
                    db.UpgradeUniversal(Utils.databaseName, Utils.tableName, "id", userId.ToString(), "soldierNum", 0.ToString());
                    Utils.ErrorHandler(false, "Occupy enemy city failed");
                    return GenerateAttackResponse(false, "Occupy enemy city failed");
                }
            }
        }

        public static OperationResponse UpgradeInfoHandler(Dictionary<byte, object> param)
        {
            int userId = (int)param[(byte)UpgradeInfoData.userId];
            string username = (string)param[(byte)UpgradeInfoData.username];
            int soldierNum = (int)param[(byte)UpgradeInfoData.soldierNum];
            if (username == null || userId.ToString() == null || soldierNum.ToString() == null)
            {
                Utils.ErrorHandler(false, "Invalid parameters");
                return GenerateUpgradeInfoResponse(false, "Invalid parameters");
            }
            DatabaseHandler db = new DatabaseHandler();
            bool flag;
            flag = db.ConnectionTest();
            if(!flag)
            {
                Utils.ErrorHandler(flag, "Database connection failed");
                return GenerateUpgradeInfoResponse(false, "Database connection failed");
            }

            flag = db.UpgradeUniversal(Utils.databaseName, "city", "userId", userId.ToString(), "username", username);
            if(!flag)
            {
                Utils.ErrorHandler(flag, "Upgrade username failed in city table");
                return GenerateUpgradeInfoResponse(false, "Upgrade username failed");
            }
            flag = db.UpgradeUniversal(Utils.databaseName, Utils.tableName, "id", userId.ToString(), "username", username);
            if(!flag)
            {
                Utils.ErrorHandler(flag, "Upgrade username failed in player table");
                return GenerateUpgradeInfoResponse(false, "Upgrade username failed");
            }
            flag = db.UpgradeUniversal(Utils.databaseName, Utils.tableName, "id", userId.ToString(), "soldierNum", soldierNum.ToString());
            if (!flag)
            {
                Utils.ErrorHandler(flag, "Upgrade soldierNum failed");
                return GenerateUpgradeInfoResponse(false, "Upgrade soldierNum failed");
            }
            Utils.ErrorHandler(true, "Upgrade success");
            return GenerateUpgradeInfoResponse(true, "Upgrade success");
        }

        public static EventData UpgradeTrigger()
        {
            DatabaseHandler db = new DatabaseHandler();
            bool flag;
            flag = db.ConnectionTest();
            if (!flag)
            {
                Utils.ErrorHandler(flag, "Database connection failed");
                return GenerateEventData(flag, "Database connection failed");
            }
            List<Dictionary<string, object>> userData = db.QueryAll(Utils.databaseName, Utils.tableName);
            List<Dictionary<string, object>> cityData = db.QueryAll(Utils.databaseName, "city");
            Utils.ErrorHandler(true, "Upgrade trigger success");
            return GenerateEventData(true, "Upgrade trigger success", userData, cityData);
        }
    }
}
