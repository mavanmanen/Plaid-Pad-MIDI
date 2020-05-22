namespace Mavanmanen.PPM
{
    /// <summary>
    /// Used for communicating short messages with the Plaid Pad.
    /// </summary>
    public enum RawHIDMessage
    {
        /// <summary>
        /// Acknowledgement of a received message
        /// </summary>
        HID_ACK = 100,

        /// <summary>
        /// To indicate successful connection.
        /// </summary>
        HID_CONNECTED,

        /// <summary>
        /// To indicate a disconnection.
        /// </summary>
        HID_DISCONNECTED,

        /// <summary>
        /// Get the current state of the Plaid Pad.
        /// </summary>
        HID_GET_STATE,

        /// <summary>
        /// Save the local state to the Plaid Pad.
        /// </summary>
        HID_SAVE_STATE,

        /// <summary>
        /// Message contains the current state of the Plaid Pad.
        /// </summary>
        HID_STATE,

        /// <summary>
        /// Clear the state on the Plaid Pad.
        /// </summary>
        HID_CLEAR_STATE
    }
}
