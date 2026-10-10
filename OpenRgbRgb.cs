using System.Collections.Generic;
using System.Reflection;
using OpenRgbRgb.OpenRgb;
using ReLogic.Peripherals.RGB;
using Terraria.Initializers;
using Terraria.ModLoader;

namespace OpenRgbRgb;

public class OpenRgbRgb : Mod {
    public static OpenRgbRgb Instance { get; private set; }

    public OpenRgbRgb() {
        Instance = this;
    }

    private static ChromaEngine GetChromaEngine() {
        FieldInfo field = typeof(ChromaInitializer).GetField(
            "_engine",
            BindingFlags.Static | BindingFlags.NonPublic
        );

        return (ChromaEngine)field!.GetValue(null);
    }
    
    public override void Load() {
        Logger.Info("Loading OpenRgbRgb");

        if (!ModContent.GetInstance<OpenRgbRgbModConfig>().EnableOpenRgb) {
            Logger.Info("Mod is disabled, doing nothing");
            return;
        }

        ChromaEngine engine = GetChromaEngine();
        engine.AddDeviceGroup("OpenRgb", new OpenRgbDeviceGroup());
        engine.EnableDeviceGroup("OpenRgb");
    }

    public override void Unload() {
        try {
            ChromaEngine engine = GetChromaEngine();
            engine.DisableDeviceGroup("OpenRgb");
            engine.RemoveDeviceGroup("OpenRgb");
        }
        catch (KeyNotFoundException) {
            // the device group does not exist (most likely the mod was disabled)
        }
    }
}