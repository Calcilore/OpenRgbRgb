using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace OpenRgbRgb;

public class OpenRgbRgbModConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("General")]
    [DefaultValue(true)] [ReloadRequired]
    public bool EnableOpenRgb;

    [Header("DeviceProfile")]
    [DefaultValue(1.0f)] [Range(0f, 1f)] [ReloadRequired]
    public float RedMultiplier;
    
    [DefaultValue(1.0f)] [Range(0f, 1f)] [ReloadRequired]
    public float GreenMultiplier;
    
    [DefaultValue(1.0f)] [Range(0f, 1f)] [ReloadRequired]
    public float BlueMultiplier;

    [Header("Connection")]
    [DefaultValue("127.0.0.1")] [ReloadRequired]
    public string Ip;
    
    [DefaultValue(6742)] [Range(0, 65535)] [ReloadRequired]
    public int Port;
}