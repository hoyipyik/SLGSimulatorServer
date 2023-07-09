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
    public static class Utils
    {
        public static readonly string databaseName = "SLGGame";
        public static readonly string tableName = "player";

        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static double RandomNumber(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }

        public static void DatabaseInitial()
        {
            DatabaseHandler db = new DatabaseHandler();
            db.ConnectionTest();
            db.CreateDatabase(databaseName);
            db.CreateTable(databaseName, tableName, Data.PlayerTypes.ToArray());
            db.CreateTable(databaseName, "city", Data.CityTypes.ToArray());
        }

        public static void LoggerInitial(string applicationRootPath, string binaryPath)
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(applicationRootPath, Path.Combine("bin_Win64", "log"));
            FileInfo configurationInfo = new FileInfo(Path.Combine(binaryPath, "log4net.config"));
            if (configurationInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configurationInfo);
            }
        }

        public static void Printer(List<Dictionary<string, object>> data)
        {
            foreach (var item in data)
            {
                foreach (var pair in item)
                {
                    log.Info(pair.Key + ": " + pair.Value);
                }
                log.Info("  ");
            }
        }

        public static void ErrorHandler(bool flag, string msg = "Operation")
        {
            if (flag)
            {
                log.Info(msg + " success");
            }
            else
            {
                log.Error(msg + " failed");
            }
        }
    }
}