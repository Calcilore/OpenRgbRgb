using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OpenRGB.NET;
using ReLogic.Peripherals.RGB;
using Color = OpenRGB.NET.Color;

namespace OpenRgbRgb.OpenRgb;

public class OpenRgbKeyboard : RgbKeyboard {
    private Color[] _colors;
    private readonly List<(int, Color)> _renderColors = [];

    private readonly OpenRgbDevice _device;
    private readonly Zone _zone;
    private readonly int _zoneStartIndex;

    public OpenRgbKeyboard(OpenRgbDevice device, Zone zone, int zoneStartIndex, DeviceColorProfile profile) : base(RgbDeviceVendor.Unknown, GetFragment(zone), profile) {
        _device = device;
        _zoneStartIndex = zoneStartIndex;
        _zone = zone;
        _colors = new Color[zone.LedCount];
    }

    private static Fragment GetFragment(Zone zone) {
        MatrixMap matrixMap = zone.MatrixMap!;
        return Fragment.FromGrid(new Rectangle(0, 0, (int)matrixMap.Width, (int)matrixMap.Height));
    }

    private static Color Vector2Color(Vector4 color) {
        return new Color((byte)(color.X * 255), (byte)(color.Y * 255), (byte)(color.Z * 255));
    }

    // this seems to only be called to set hotkey colors
    public override void Render(IEnumerable<RgbKey> keys) {
        List<RgbKey> keyss = keys.ToList();
        
        foreach (RgbKey key in keyss) {
            // get per device index for key
            if (!_device.KeyToIndex.TryGetValue(key.Key, out int index)) {
                continue;
            }
            
            // convert to zone index and check if index is in this zone
            index -= _zoneStartIndex;
            if (index < 0 || index >= _zone.LedCount) {
                continue;
            }
            
            // save color to list to render in Present()
            Microsoft.Xna.Framework.Color xnaColor = ProcessLedColor(key.CurrentColor);
            _renderColors.Add((index, new Color(xnaColor.R, xnaColor.G, xnaColor.B)));
        }
    }
    
    public override void Present() {
        // get background colors
        for (int i = 0; i < LedCount; i++) {
            uint index = _zone.MatrixMap!.Matrix[i / _zone.MatrixMap!.Width, i % _zone.MatrixMap!.Width];
            if (index != uint.MaxValue) {
                _colors[index] = Vector2Color(GetProcessedLedColor(i));
            }
        }

        // get hotkey colors
        foreach ((int index, Color color) in _renderColors) {
            _colors[index] = color;
        }
        
        _renderColors.Clear();
        _device.Client.UpdateZoneLeds(_device.Device.Index, _zone.Index, _colors);
    }
}