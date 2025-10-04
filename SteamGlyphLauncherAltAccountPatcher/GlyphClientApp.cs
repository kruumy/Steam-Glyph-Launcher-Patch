using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamGlyphLauncherAltAccountPatcher
{
    public class GlyphClientApp : INotifyPropertyChanged
    {
        public enum Status : uint
        {
            WaitingForFilePath,
            InvalidFilePath,
            ValidFilePathNotPatched,
            ValidFilePathPatched,
            SuccessfullyPatched,
            SuccessfullyUnpatched,
            PatchFailed,
            UnpatchFailed
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string filePath;
        private Status lastStatus = Status.WaitingForFilePath;

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                if( !IsValid )
                {
                    LastStatus = Status.InvalidFilePath;
                    filePath = null;
                }
                else
                {
                    LastStatus = IsPatched ? Status.ValidFilePathPatched : Status.ValidFilePathNotPatched;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPatched)));
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(filePath) && File.Exists(filePath) == true && Path.GetFileName(filePath).Equals("GlyphClientApp.exe", StringComparison.OrdinalIgnoreCase);

        private readonly byte[] unpatchedBytes = { 0x75, 0x7A, 0x8B, 0xCE };
        private readonly byte[] patchedBytes = { 0xEB, 0x7A, 0x8B, 0xCE };

        public Status LastStatus
        {
            get => lastStatus;
            set
            {
                lastStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastStatus)));
            }
        }
        public bool IsPatched
        {
            get
            {
                if ( !IsValid )
                {
                    return false;
                }

                using ( var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) )
                {
                    return StreamExtensions.SeekToByteArray(stream, patchedBytes);
                }
            }
        }

        public bool Patch()
        {
            if( !IsValid )
            {
                return false;
            }

            using ( var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite) )
            {
                if ( stream.SeekToByteArray(unpatchedBytes) )
                {
                    stream.Write(patchedBytes, 0, patchedBytes.Length);
                    stream.Flush();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPatched)));
                    LastStatus = Status.SuccessfullyPatched;
                    return true;
                }
                else
                {
                    LastStatus = Status.PatchFailed;
                    return false;
                }
            }
        }

        public bool Unpatch()
        {
            if ( !IsValid )
            {
                return false;
            }

            using ( var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite) )
            {
                if ( stream.SeekToByteArray(patchedBytes) )
                {
                    stream.Write(unpatchedBytes, 0, unpatchedBytes.Length);
                    stream.Flush();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPatched)));
                    LastStatus = Status.SuccessfullyUnpatched;
                    return true;
                }
                else
                {
                    LastStatus = Status.UnpatchFailed;
                    return false;
                }
            }
        }
    }
}
