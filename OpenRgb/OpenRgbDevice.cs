using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OpenRGB.NET;
using ReLogic.Peripherals.RGB;

namespace OpenRgbRgb.OpenRgb;

public class OpenRgbDevice {
    public readonly OpenRgbClient Client;
    public readonly Device Device;
    public readonly List<OpenRgbKeyboard> Keyboards = [];
    
    public readonly Dictionary<Keys, int> KeyToIndex = [];

    public OpenRgbDevice(OpenRgbClient client, Device device, OpenRgbRgbModConfig config) {
        Client = client;
        Device = device;
        
        foreach (Led led in device.Leds) {
            if (OpenRgbConversion.NameToKey.TryGetValue(led.Name, out Keys key)) {
                KeyToIndex[key] = led.Index;
            }
        }
        
        OpenRgbRgb.Instance.Logger.Info($"Keyboard has {KeyToIndex.Count} keys mapped ({device.Leds.Length} total)");

        DeviceColorProfile profile = new(new Vector3(config.RedMultiplier, config.GreenMultiplier, config.BlueMultiplier));
        int zoneOffset = 0;
        
        foreach (Zone zone in device.Zones) {
            Keyboards.Add(new OpenRgbKeyboard(this, zone, zoneOffset, profile));
            zoneOffset += (int)zone.LedCount;
        }
    }
}