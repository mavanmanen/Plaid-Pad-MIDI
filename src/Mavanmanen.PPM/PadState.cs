using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Mavanmanen.PPM.Annotations;

namespace Mavanmanen.PPM
{
    public class PadState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private byte _encoderLeft;
        private byte _encoderRight;
        private bool[] _buttons = new bool[16];
        
        public byte EncoderLeft
        {
            get => _encoderLeft;
            set
            {
                if (value == _encoderLeft) return;
                _encoderLeft = value;
                OnPropertyChanged();
            }
        }

        public byte EncoderRight
        {
            get => _encoderRight;
            set
            {
                if (value == _encoderRight) return;
                _encoderRight = value;
                OnPropertyChanged();
            }
        }

        public bool[] Buttons
        {
            get => _buttons;
            set
            {
                if (Equals(value, _buttons)) return;
                _buttons = value;
                OnPropertyChanged();
            }
        }

        public void SetButton(int index, bool value)
        {
            Buttons[index] = value;
            OnPropertyChanged(nameof(Buttons));
        }

        public void LoadFromByteData(IEnumerable<byte> rawData)
        {
            var data = rawData.Skip(2).ToArray();

            EncoderLeft = data[0];
            EncoderRight = data[1];
            Buttons = data.Skip(2).Take(16).Select(b => b != 0).ToArray();
        }
    }
}
