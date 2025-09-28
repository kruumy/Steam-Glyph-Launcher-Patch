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
        public event PropertyChangedEventHandler PropertyChanged;

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                if( !IsValid )
                {
                    filePath = null;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPatched)));
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(filePath) && File.Exists(filePath) == true && Path.GetFileName(filePath).Equals("GlyphClientApp.exe", StringComparison.OrdinalIgnoreCase);

        private readonly byte[] unpatchedBytes = { 0x75, 0x7A, 0x8B, 0xCE };
        private readonly byte[] patchedBytes = { 0xEB, 0x7A, 0x8B, 0xCE };

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
                if ( StreamExtensions.SeekToByteArray(stream, unpatchedBytes) )
                {
                    stream.Write(patchedBytes, 0, patchedBytes.Length);
                    stream.Flush();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPatched)));
                    return true;
                }
                else
                {
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
                if ( StreamExtensions.SeekToByteArray(stream, patchedBytes) )
                {
                    stream.Write(unpatchedBytes, 0, unpatchedBytes.Length);
                    stream.Flush();

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPatched)));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
