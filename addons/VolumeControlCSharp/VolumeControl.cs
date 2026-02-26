using Godot;
using Godot.Collections;

namespace Hedgestock
{
    [Icon("./VolumeControl.svg"), Tool, GlobalClass]
    public partial class VolumeControl : VBoxContainer
    {
        CheckBox MuteCheckBox;
        HSlider VolumeSlider;

        enum AudioBus { }

        int _bus;
        [Export]
        AudioBus Bus
        {
            get { return (AudioBus)_bus; }
            set
            {
                _bus = (int)value;
                if (MuteCheckBox != null)
                {
                    MuteCheckBox.ButtonPressed = !AudioServer.IsBusMute(_bus);
                    MuteCheckBox.Text = AudioServer.GetBusName(_bus) + " Volume";
                }
                if (VolumeSlider != null)
                    VolumeSlider.Value = AudioServer.GetBusVolumeLinear(_bus);
            }
        }

        public override void _ValidateProperty(Dictionary property)
        {
            base._ValidateProperty(property);
            if (property["name"].AsStringName() == PropertyName.Bus)
            {
                int busNumber = AudioServer.BusCount;
                property["hint_string"] = "";
                for (int i = 0; i < busNumber; i++)
                {
                    if (i > 0)
                        property["hint_string"] += ",";
                    var busName = AudioServer.GetBusName(i);
                    property["hint_string"] += busName;
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();
            if (MuteCheckBox == null)
            {
                MuteCheckBox = new()
                {
                    ButtonPressed = !AudioServer.IsBusMute(_bus),
                    Text = AudioServer.GetBusName(_bus) + " Volume",
                    Flat = true,
                    Name = "MuteCheckBox",
                };
                MuteCheckBox.Connect(CheckBox.SignalName.Toggled, Callable.From<bool>(MuteVolume));
                AddChild(MuteCheckBox, false, InternalMode.Front);
            }

            if (VolumeSlider == null)
            {
                VolumeSlider = new()
                {
                    MaxValue = 1,
                    Step = 0.01f,
                    Value = AudioServer.GetBusVolumeLinear(_bus),
                    Name = "VolumeSlider",
                };
                VolumeSlider.Connect(HSlider.SignalName.ValueChanged, Callable.From<float>(VolumeChanged));
                AddChild(VolumeSlider, false, InternalMode.Front);
            }
        }

        private void MuteVolume(bool on)
        {
            AudioServer.SetBusMute(_bus, !on);
        }

        private void VolumeChanged(float volume)
        {
            AudioServer.SetBusVolumeLinear(_bus, volume);
        }
    }
}