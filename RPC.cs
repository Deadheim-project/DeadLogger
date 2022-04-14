using HarmonyLib;
using System;
using System.IO;

namespace DeadLogger
{
    [HarmonyPatch]
    class RPC
    {
        public static void RPC_DeadLogger_SaveLog(long sender, ZPackage pkg)
        {
            if (!ZNet.instance.IsServer()) return;

            string[] pkgStringArray = pkg.ReadString().Split('|');
            string nick = pkgStringArray[0];
            string playerId = pkgStringArray[1];
            string description = pkgStringArray[2];

            string generalLog = DeadLogger.FileDirectory + "generalLog.txt";
            string fileName = nick + "_" + playerId + ".txt";

            string fileLocation = DeadLogger.FileDirectory + fileName;

            if (!Directory.Exists(DeadLogger.FileDirectory)) Directory.CreateDirectory(DeadLogger.FileDirectory);

            DateTime date = DateTime.UtcNow;
            date = date.AddHours(-3);
            string dateToSave = date.Hour + ":" + date.Minute + " " + date.Day + "/" + date.Month + "/" + date.Year;
            description += " " + dateToSave;

            File.AppendAllText(fileLocation, description + Environment.NewLine);
            File.AppendAllText(generalLog, nick + " " + playerId + " " + description + Environment.NewLine);
        }

        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStart
        {
            public static void Postfix()
            {
                if (ZRoutedRpc.instance == null)
                    return;

                ZRoutedRpc.instance.Register<ZPackage>("DeadLogger_SaveLog", new Action<long, ZPackage>(RPC_DeadLogger_SaveLog));      
            }
        }
    }
}
