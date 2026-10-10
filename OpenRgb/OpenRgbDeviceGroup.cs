using System;
using System.Collections.Generic;
using OpenRGB.NET;
using ReLogic.Peripherals.RGB;
using Terraria.ModLoader;

namespace OpenRgbRgb.OpenRgb;

public class OpenRgbDeviceGroup : RgbDeviceGroup {
    private bool _isInitialized;
    private readonly List<RgbDevice> _devices = [];
    private OpenRgbClient _client;
    
    protected override void Initialize() {
        if (_isInitialized) {
            return;
        }

        _isInitialized = true;

        try {
            OpenRgbRgbModConfig config = ModContent.GetInstance<OpenRgbRgbModConfig>();
            _client = new OpenRgbClient(config.Ip, config.Port, name:"Terraria", timeoutMs:1000);
            _client.Connect();
            OpenRgbRgb.Instance.Logger.Info("Connected to OpenRGB");

            foreach (Device device in _client.GetAllControllerData()) {
                if (device.Type == DeviceType.Keyboard) {
                    // adds a terraria RgbKeyboard for each zone in each OpenRgb keyboard
                    _devices.AddRange(new OpenRgbDevice(_client, device, config).Keyboards);
                }
            }

            OpenRgbRgb.Instance.Logger.Info("OpenRGB initialized.");
        }
        catch (TimeoutException) {
            OpenRgbRgb.Instance.Logger.Warn("Failed to connect to OpenRGB, timeout (is the openrgb server running?)");
        }
    }

    protected override void Uninitialize() { }

    public override IEnumerator<RgbDevice> GetEnumerator() {
        return _devices.GetEnumerator();
    }
}