using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Mavanmanen.PPM.Annotations;

namespace Mavanmanen.PPM
{
    /// <summary>
    /// Represents the current state of the Plaid Pad.
    /// </summary>
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

        /// <summary>
        /// Value of the left rotary encoder, between 0 and 127.
        /// </summary>
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

        /// <summary>
        /// Value of the right rotary encoder, between 0 and 127.
        /// </summary>
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

        /// <summary>
        /// Current state of a button.
        /// </summary>
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

        /// <summary>
        /// Set the value of a button.
        /// </summary>
        /// <param name="index">Index of the button.</param>
        /// <param name="value">The value to set.</param>
        public void SetButton(int index, bool value)
        {
            Buttons[index] = value;
            OnPropertyChanged(nameof(Buttons));
        }

        /// <summary>
        /// Load the state from the raw data received from the Plaid Pad.
        /// </summary>
        /// <param name="rawData">Raw data that was received from the plaid pad.</param>
        public void LoadFromByteData(IEnumerable<byte> rawData)
        {
            var data = rawData.Skip(2).ToArray();

            EncoderLeft = data[0];
            EncoderRight = data[1];
            Buttons = data.Skip(2).Take(16).Select(b => b != 0).ToArray();
        }
    }
}
