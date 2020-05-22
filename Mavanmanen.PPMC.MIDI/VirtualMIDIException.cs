using System;

namespace Mavanmanen.PPMC.MIDI
{
    public enum VirtualMIDIExceptionReasonCode
    {
        ERROR_PATH_NOT_FOUND = 3,
        ERROR_INVALID_HANDLE = 6,
        ERROR_TOO_MANY_CMDS = 56,
        ERROR_TOO_MANY_SESS = 69,
        ERROR_INVALID_NAME = 123,
        ERROR_MOD_NOT_FOUND = 126,
        ERROR_BAD_ARGUMENTS = 160,
        ERROR_ALREADY_EXISTS = 183,
        ERROR_OLD_WIN_VERSION = 1150,
        ERROR_REVISION_MISMATCH = 1306,
        ERROR_ALIAS_EXISTS = 1379
    }

    public class VirtualMIDIException : Exception
    {
        public VirtualMIDIExceptionReasonCode ReasonCode { get; set; }
        public override string Message { get; }


        public VirtualMIDIException(int reasonCode)
        {
            if (!Enum.IsDefined(typeof(VirtualMIDIExceptionReasonCode), reasonCode))
            {
                Message = "Unspecified virtualMIDI-error: " + reasonCode;
            }

            ReasonCode = (VirtualMIDIExceptionReasonCode) reasonCode;
 
            switch (ReasonCode)
            {
                case VirtualMIDIExceptionReasonCode.ERROR_OLD_WIN_VERSION:
                    Message = "Your Windows-version is too old for dynamic MIDI-port creation.";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_INVALID_NAME:
                    Message = "You need to specify at least 1 character as MIDI-portname!";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_ALREADY_EXISTS:
                    Message = "The name for the MIDI-port you specified is already in use!";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_ALIAS_EXISTS:
                    Message = "The name for the MIDI-port you specified is already in use!";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_PATH_NOT_FOUND:
                    Message = "Possibly the teVirtualMIDI-driver has not been installed!";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_MOD_NOT_FOUND:
                    Message = "The teVirtualMIDIxx.dll could not be loaded!";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_REVISION_MISMATCH:
                    Message = "The teVirtualMIDIxx.dll and teVirtualMIDI.sys driver differ in version!";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_TOO_MANY_SESS:
                    Message = "Maximum number of ports reached";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_INVALID_HANDLE:
                    Message = "Port not enabled";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_TOO_MANY_CMDS:
                    Message = "MIDI-command too large";
                    break;

                case VirtualMIDIExceptionReasonCode.ERROR_BAD_ARGUMENTS:
                    Message = "Invalid flags specified";
                    break;
            }
        }
    }
}