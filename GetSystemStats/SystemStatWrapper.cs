using InteropHelpers;
using System;

namespace GetSystemStats
{
    public class SystemStatWarapper : IDisposable
    {
        private bool disposed = false;
        LoadAverageHandle LoadAverageHandle;
        
        SystemInfoHandle SystemInfoHandle;
        
        CoreUsageHandle CoreUsageHandle;
        
        public UInt64 GetTotalMemory()
        {
            return SystemStatInteropCall.get_total_memory_info();
        }

        public UInt64 GetUsedMemory()
        {
            return SystemStatInteropCall.get_used_memory_info();
        }

        public UInt64 GetAvailableMemory()
        {
            return SystemStatInteropCall.get_free_memory();
        }

        public double GetProcessorUsage()
        {
            return SystemStatInteropCall.get_processor_usage();
        }

        public SystemInfoHandle GetSystemInfo()
        {
            SystemInfoHandle = SystemStatInteropCall.get_system_info();
            return SystemInfoHandle;
        }

        public LoadAverageHandle GetLoadAverage()
        {
            LoadAverageHandle = SystemStatInteropCall.get_load_avg();
            return LoadAverageHandle;
        }

        public CoreUsageHandle GetCoreUsage()
        {
            CoreUsageHandle = SystemStatInteropCall.get_core_usage();
            return CoreUsageHandle;
        }

        public void Dispose()
        {
            LoadAverageHandle.Dispose();
            SystemInfoHandle.Dispose();
            CoreUsageHandle.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    // technically dispose managed objects here...I don't have any
                }
                if(this.LoadAverageHandle != null && !this.LoadAverageHandle.IsInvalid)
                {
                    LoadAverageHandle.Dispose();
                    //this.disposed = true;
                }

                if(this.SystemInfoHandle != null && !this.SystemInfoHandle.IsInvalid)
                {
                    SystemInfoHandle.Dispose();
                    //this.disposed = true;
                }

                if(this.CoreUsageHandle != null && !this.CoreUsageHandle.IsInvalid)
                {
                    CoreUsageHandle.Dispose();
                    // this.disposed = ture;
                }
                this.disposed = true;

            }
        }
    }

    public class DiskStatWrapper : IDisposable
    {
        private DiskInfoHandle _diskInfoHandle;

        private bool disposed = false;
        

        public DiskStatWrapper()
        {
            _diskInfoHandle = SystemStatInteropCall.disk_info_new();
        }

        public void QueryDiskInfo()
        {
            SystemStatInteropCall.disk_info_query(_diskInfoHandle);
        }

        public (UInt64[], UInt64[]) GetDiskInfo()
        {
            UInt32 diskCount = SystemStatInteropCall.disk_info_get_disk_usage_diskcount(_diskInfoHandle);
            UInt64[] diskFreespace = new UInt64[diskCount];
            UInt64[] diskTotalspace = new UInt64[diskCount];

            for(int i = 0; i < diskCount; i++)
            {
                diskFreespace[i] = GetDiskInfoFreespace((UInt64)i);
                diskTotalspace[i] = GetDiskInfoTotalSpace((UInt64)i);
            }
            
            return (diskFreespace, diskTotalspace);

        }

        private UInt64 GetDiskInfoFreespace(UInt64 diskIndex)
        {
            return
                SystemStatInteropCall.disk_info_get_disk_freespace(_diskInfoHandle,
                                                                   diskIndex);
        }

        private UInt64 GetDiskInfoTotalSpace(UInt64 diskIndex)
        {
            return 
                SystemStatInteropCall.disk_info_get_disk_totalspace(_diskInfoHandle,
                                                                    diskIndex);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    // technically dispose managed objects here...I don't have any
                }
                if(this._diskInfoHandle != null && ! this._diskInfoHandle.IsInvalid)
                {
                    _diskInfoHandle.Dispose();
                    this.disposed = true;
                }

            }
        }
    }

}