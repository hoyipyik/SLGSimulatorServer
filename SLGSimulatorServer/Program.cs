//using System;
//using System.Collections.Generic;

//namespace MyGameServer
//{
//    class Program
//    {
//        static void Main()
//        {
//            DatabaseHandler databaseHandler = new DatabaseHandler();
//            DatabaseHandler databaseHandlerObj = databaseHandler;
//            bool dbStateFlag = databaseHandlerObj.ConnectionTest();
//            MyGameServer.log.Info("Connection Test: " + dbStateFlag);
//            dbStateFlag = databaseHandlerObj.CreateDatabase("test");
//            MyGameServer.log.Info("Database Creation: " + dbStateFlag);
//            KeyValuePair<string, string>[] kvPairs = Data.GetTypes().ToArray();
//            dbStateFlag = databaseHandlerObj.CreateTable("test", "knights", kvPairs);
//            MyGameServer.log.Info("Table Creation Test: " + dbStateFlag);
//            var insertSData = Data.GetData().ToArray();
//            dbStateFlag = databaseHandlerObj.InsertData("test", "knights", insertSData);
//            MyGameServer.log.Info("Data Insert Test: " + dbStateFlag);
//            var gottenData = databaseHandlerObj.QueryUniversal("test", "knights", "x,y,hp", "name", "Philips II");
//            // var gottenData = databaseHandlerObj.QueryAll("test", "knights");
//            MyGameServer.log.Info("Query Data: ");
//            Utils.PrintDataFromDatabase(gottenData);
//            dbStateFlag = databaseHandlerObj.DeleteUniversal("test", "knights", "id", "7");
//            MyGameServer.log.Info("Data Delete Test: " + dbStateFlag);
//            dbStateFlag = databaseHandlerObj.UpgradeUniversal("test", "knights", "id", "11", "hp", "0");
//            MyGameServer.log.Info("Data Upgrade Test: " + dbStateFlag);
//            // Console.ReadKey();
//        }

//    }
//}
