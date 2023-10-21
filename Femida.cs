//Version 0.1
//Name: Femida
//Description: Femida is an anticheat for Terraria for server admins.


// ███████╗███████╗███╗   ███╗██╗██████╗  █████╗ 
// ██╔════╝██╔════╝████╗ ████║██║██╔══██╗██╔══██╗
// █████╗  █████╗  ██╔████╔██║██║██║  ██║███████║   By WoodMan with <3
// ██╔══╝  ██╔══╝  ██║╚██╔╝██║██║██║  ██║██╔══██║
// ██║     ███████╗██║ ╚═╝ ██║██║██████╔╝██║  ██║
// ╚═╝     ╚══════╝╚═╝     ╚═╝╚═╝╚═════╝ ╚═╝  ╚═╝
                                              

// Modules

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;


namespace Femida
{
    [ApiVersion(2, 1)] 
    public class Femida : TerrariaPlugin
    {
        public Femida(Main game)
            : base(game)
        {
            Order = -4; // -4 here
        }

        public override void Initialize()
        {
			Commands.ChatCommands.Add(new Command("tshock.admin.ban", OfflineBan, "fban"));
        }

   //     public override string Version
   //     {
			//get { return "1.0"; }           // No such thing
   //     }

        public override string Name
		{
            get { return "Femida"; }
        }

        public override string Author
		{
            get { return "woodman, Updated by Maxthegreat99"; }
        }

        public override string Description
		{
            get { return "Terraria Anticheat"; }
        }

        private static void OfflineBan(CommandArgs args)
        {

            if (args.Parameters.Count < 1 || args.Parameters[0].ToLower() == "help")
			{
				args.Player.SendInfoMessage("Syntax: /fban  add \"name\" [reason]");
				return;
			}

			if (args.Parameters[0].ToLower() != "add")
			{
				#region Default commands

				args.Player.SendInfoMessage("use /ban.");
                return;

                #endregion Default commands
            }

            if (args.Parameters.Count >= 2)
			{
				#region Add ban

				string plStr = args.Parameters[1];

				var player = TShock.UserAccounts.GetUserAccountByName(plStr); 

				if (player == null)
				{
					args.Player.SendErrorMessage("Invalid username!");
					return;
				}

				else
				{
					string reason = args.Parameters.Count > 2
									? String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2))
									: "no reason.";

					bool force = !args.Player.RealPlayer;

					string adminUserName = args.Player.Name;

					adminUserName = String.IsNullOrWhiteSpace(adminUserName) ? args.Player.Name : adminUserName;

					if (force || !TShock.Groups.GetGroupByName(player.Group).HasPermission(Permissions.immunetoban ))
					{
						List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(player.KnownIps);

						string ip = KnownIps[KnownIps.Count - 1];

						string uuid = player.UUID;

						string playerName = player.Name;

						string identf = $"{Identifier.Account}{playerName}";

						DateTime expiration = DateTime.MaxValue;

						AddBanResult addBanResult = TShock.Bans.InsertBan(identf, reason, playerName, DateTime.UtcNow, expiration);

						var players = TShockAPI.TSPlayer.FindByNameOrID(player.Name);
						
						if (players.Count == 1) players[0].Disconnect(string.Format("Banned!!!: {0}", reason));

						Console.WriteLine(string.Format("{0} was banned by {1}. Reason: '{2}'", playerName, adminUserName, reason));
						
						string verb = force ? "force-" : "";
						
						if (String.IsNullOrWhiteSpace(adminUserName))
							TSPlayer.All.SendInfoMessage((string.Format("Player {0} was banned. Reason: '{1}'", playerName, reason.ToLower())));
						else
							TSPlayer.All.SendInfoMessage(string.Format("{0} was banned by {1}. Reason: '{2}'", playerName, adminUserName, reason));
					}
					else
					{
						args.Player.SendErrorMessage("You cannot ban an admin!");
					}

				}
				return;
				#endregion Add ban
				
            }
			args.Player.SendInfoMessage("Syntax: /fban  add \"name\" [reason]");
        }
    }
}
