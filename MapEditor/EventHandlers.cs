using Mirror;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs;
using UnityEngine;
using Exiled.Permissions.Extensions;
using static MapEditor.MapEditorModels;

namespace MapEditor
{
    public class EventHandlers
    {


        public static void RemoteAdminCommandEvent(SendingRemoteAdminCommandEventArgs ev)
        {


            switch (ev.Name.ToLower())
            {
                case "mapeditor":
                    ev.IsAllowed = false;
                    if (!ev.Sender.CheckPermission("mapeditor"))
                    {
                        ev.CommandSender.RaReply("MapEditor#No permission.", true, true, string.Empty);
                        return;
                    }
                    if (ev.Arguments.Count == 0)
                    {
                        ev.CommandSender.RaReply("MapEditor#Commands:", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR UNLOAD <NAME> - Unloads map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR LOAD <NAME> - Load map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CREATE <NAME> - Create map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR DELETE <NAME> - Delete map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CREATEOBJECT DOORLCZ/DOORHCZ/DOORENT/WORKSTATION - Create object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR DELETEOBJECT - Delete object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SELECTOBJECT - Select object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CLONEOBJECT - Clone object.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR CANCEL - Cancels map editing.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR EDIT <NAME> - Edits map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SAVE - Save map.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETPOS <X> <Y> <Z> - Set object position.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETROT <X> <Y> <Z> - Set object rotation.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETSCALE <X> <Y> <Z> - Set object scale.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETLOCK true/false - Set door lock.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR SETPERMS <perm> - Set door perms.", true, true, string.Empty);
                        ev.CommandSender.RaReply("MapEditor# - MAPEDITOR BRING - Bring object to player position.", true, true, string.Empty);
                        return;
                    }
                    else if (ev.Arguments.Count != 0)
                    {
                        switch(ev.Arguments[0].ToLower())
                        {
                            case "unload":
                                Editor.UnloadMap(ev.CommandSender, ev.Arguments[1]);
                                break;
                            case "load":
                                Editor.LoadMap(ev.CommandSender, ev.Arguments[1]);
                                break;
                            case "create":
                                Editor.CreateMap(ev.CommandSender, ev.Arguments[1]);
                                break;
                            case "delete":
                                Editor.DeleteMap(ev.CommandSender, ev.Arguments[1]);
                                break;
                            case "createobject":
                                Editor.CreateObject(ev.CommandSender, ev.Arguments[1]);
                                break;
                            case "deleteobject":
                                Editor.DeleteObject(ev.CommandSender);
                                break;
                            case "selectobject":
                                Editor.SelectObject(ev.CommandSender);
                                break;
                            case "cancel":
                                Editor.StopMapEditing(ev.CommandSender);
                                break;
                            case "edit":
                                Editor.EditMap(ev.CommandSender, ev.Arguments[1]);
                                break;
                            case "save":
                                Editor.SaveMap(ev.CommandSender);
                                break;
                            case "setpos":
                                Editor.SetPositionObject(ev.CommandSender, float.Parse(ev.Arguments[1]), float.Parse(ev.Arguments[2]), float.Parse(ev.Arguments[3]));
                                break;
                            case "setrot":
                                Editor.SetRotationObject(ev.CommandSender, float.Parse(ev.Arguments[1]), float.Parse(ev.Arguments[2]), float.Parse(ev.Arguments[3]));
                                break;
                            case "setscale":
                                Editor.SetScaleObject(ev.CommandSender, float.Parse(ev.Arguments[1]), float.Parse(ev.Arguments[2]), float.Parse(ev.Arguments[3]));
                                break;
                            case "cloneobject":
                                Editor.CloneObject(ev.CommandSender);
                                break;
                            case "setlock":
                                Editor.SetLockObject(ev.CommandSender, bool.Parse(ev.Arguments[1]));
                                break;
                            case "setperms":
                                Editor.SetPermsObject(ev.CommandSender, ushort.Parse(ev.Arguments[1]));
                                break;
                            case "bring":
                                Editor.BringObject(ev.CommandSender);
                                break;
                        }
                        return;
                    }
                    break;
            }
        }

        public static void PlayerLeaveEvent(LeftEventArgs ev)
        {
            if (Editor.playerEditors.ContainsKey(ev.Player.UserId))
                Editor.playerEditors.Remove(ev.Player.UserId);
        }

        public static void RoundRestartEvent()
        {
            Editor.playerEditors = new Dictionary<string, PlayerEditorStatus>();
            Editor.maps.Clear();
        }
    }
}
