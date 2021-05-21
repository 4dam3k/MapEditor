using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor.Models
{
    public interface IMapObject
    {
        RoomType Room { get; set; }
        ObjectPosition Position { get; set; }
        ObjectPosition Rotation { get; set; }
        ObjectPosition Scale { get; set; }
        GameObject GetOrginalObject();
        bool IsEditMode { get; set; }
        MapObject LinkedMapObject { get; set; }
        void ShowEdit();
        void UnShowEdit();
    }
}
