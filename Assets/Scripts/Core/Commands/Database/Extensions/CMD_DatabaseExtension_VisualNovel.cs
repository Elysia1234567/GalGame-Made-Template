using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_VisualNovel : CMD_DatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setplayername", new Action<string>(SetPlayerNameVariable));
        }

        private static void SetPlayerNameVariable(string data)
        {
            VISUALNOVEL.VNGameSave.activeFile.playerName = data;
        }
    }
}