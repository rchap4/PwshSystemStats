/*
* Copyright RChapman 2021
* This file is part of PWshSystemStats.
*
* PWshSystemStats is free software: you can redistribute it and/or modify
* it under the terms of the Affero GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version
* PWshSystemStats is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* Affero GNU General Public License for more details
* You should have received a copy of the Affero GNU General Public License
* along with PWshSystemStats.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InteropHelpers
{
    public class SystemStatInteropCall
    {
        
        // For MacOS
        #if OSX
            const string libPath = @"libsystem_stats.dylib";
        #endif

        // For Linux
        #if Linux
            const string libPath = @"libsystem_stats.so";
        #endif

        // Windows
        #if Windows
            const string libPath = @"libsystem_stats.dll"
        #endif

        [DllImport(libPath)]
        public static extern UInt64 get_used_memory_info();

        [DllImport(libPath)]
        public static extern UInt64 get_total_memory_info();

        [DllImport(libPath)]
        public static extern SystemInfoHandle get_system_info();

        [DllImport(libPath)]
        public static extern void free_system_info_results(IntPtr s);
        
        [DllImport(libPath)]
        public static extern double get_processor_usage();

        [DllImport(libPath)]
        public static extern CoreUsageHandle get_core_usage();

        [DllImport(libPath)]
        public static extern void free_core_usage(IntPtr s);

        [DllImport(libPath)]
        public static extern LoadAverageHandle get_load_avg();
        
        [DllImport(libPath)]
        public static extern void free_load_avg(IntPtr s);

        [DllImport(libPath)]
        public static extern UInt64 get_free_memory();

        //[DllImport(libPath)]
        //public static extern DiskInfoHandle get_disk_info();
        [DllImport(libPath)]
        public static extern DiskInfoHandle disk_info_new();

        [DllImport(libPath)]
        public static extern void disk_info_query(DiskInfoHandle diskInfo);

        [DllImport(libPath)]
        public static extern UInt64 disk_info_get_disk_freespace(DiskInfoHandle diskInfo, UInt64 index);

        [DllImport(libPath)]
        public static extern UInt64 disk_info_get_disk_totalspace(DiskInfoHandle diskInfo, UInt64 index);

        [DllImport(libPath)]
        public static extern void free_disk_info(IntPtr diskInfo);

        [DllImport(libPath)]
        public static extern UInt32 disk_info_get_disk_usage_diskcount(DiskInfoHandle diskInfo);

    }

    public class DiskInfoHandle: SafeHandle
    {
        public DiskInfoHandle(): base(IntPtr.Zero, true) {}

        public override bool IsInvalid 
        {
            get { return this.handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if(! this.IsInvalid)
            {
                //Console.WriteLine("Releasing Disk Handle");
                SystemStatInteropCall.free_disk_info(handle);
            }
            return true;
        }

    }
    public class LoadAverageHandle: SafeHandle
    {
        public LoadAverageHandle(): base(IntPtr.Zero, true) {}

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct LoadAverage
        {
            public readonly double one;
            public readonly double five;
            public readonly double fifteen;
        }
        public override bool IsInvalid
        {
            get { return this.handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if(! this.IsInvalid)
            {
                //Console.WriteLine("Releasing Load Average");
                SystemStatInteropCall.free_load_avg(handle);
            }
            return true;
        }

        public LoadAverage AsStruct() 
        {
            return Marshal.PtrToStructure<LoadAverage>(handle);
        }

    }

    public class CoreUsageHandle: SafeHandle
    {
        public CoreUsageHandle(): base(IntPtr.Zero, true){}

        public override bool IsInvalid
        {
            get { return this.handle == IntPtr.Zero; }
        }

        public string AsString()
        {
            int len = 0;
            while (Marshal.ReadByte(handle, len) != 0) { ++len; }
            byte[] buffer = new byte[len];
            Marshal.Copy(handle, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        protected override bool ReleaseHandle()
        {
            if (!this.IsInvalid)
            {
                //Console.WriteLine("Releasing Core Usage");
                SystemStatInteropCall.free_core_usage(handle);
            }
            return true;
        }
    }

    public class SystemInfoHandle: SafeHandle
    {

        public SystemInfoHandle() : base(IntPtr.Zero, true) {}

        public override bool IsInvalid
        {
            get { return this.handle == IntPtr.Zero; }
        }

        public string AsString()
        {
            int len = 0;
            while (Marshal.ReadByte(handle, len) != 0) { ++len; }
            byte[] buffer = new byte[len];
            Marshal.Copy(handle, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        protected override bool ReleaseHandle()
        {
            if (!this.IsInvalid)
            {
                //Console.WriteLine("Releasing System Info");
                SystemStatInteropCall.free_system_info_results(handle);
            }

            return true;
        }
    }
}