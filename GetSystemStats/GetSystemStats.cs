using System;
using System.Management.Automation;
//using System.Management.Automation.Runspaces;
using System.Linq;
using System.Collections.Generic;

namespace GetSystemStats
{
    
    [Cmdlet(VerbsCommon.Get,"SystemStats")]
    [OutputType(typeof(SystemStats))]
    public class GetSystemStats : Cmdlet
    {
        // Save me as the TODO is to make some
        // Stats optional or default to brief output with an option
        // for all stats.  
        // [Parameter(
        //     Mandatory = true,
        //     Position = 0,
        //     ValueFromPipeline = true,
        //     ValueFromPipelineByPropertyName = true)]
        // public int FavoriteNumber { get; set; }

        // [Parameter(
        //     Position = 1,
        //     ValueFromPipelineByPropertyName = true)]
        // [ValidateSet("Cat", "Dog", "Horse")]

        // LoadAverageHandle loadAverageHandle;
        // CoreUsageHandle coreUsageHandle;
        // SystemInfoHandle systemInfoHandle;

        SystemStats output = new SystemStats();
        
        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            using (SystemStatWarapper functions = new SystemStatWarapper())
            {

                var loadAverage = functions.GetLoadAverage().AsStruct();
                output.OneMinLoadAverage = loadAverage.one;
                output.FiveMinLoadAverage = loadAverage.five;
                output.FifthteenMinLoadAverage = loadAverage.fifteen;

                output.TotalMemory =
                    (functions.GetTotalMemory() /
                        (UInt64)Math.Round(Math.Pow(2,10)));

                output.SystemInfo =
                    functions.GetSystemInfo().AsString();
                output.ProcessorCoreUsage =
                    functions.GetCoreUsage().AsString();

                output.AvailableMemory =
                    (functions.GetAvailableMemory() /
                        (UInt64)Math.Round(Math.Pow(2,10)));

                output.UsedMemory =
                    (functions.GetUsedMemory() / 
                        (UInt64)Math.Round(Math.Pow(2,10)));
                
                output.AvgProcessorUsage =
                    functions.GetProcessorUsage();

                var diskWrapper = new DiskStatWrapper();
                diskWrapper.QueryDiskInfo();

                var freeSpace = diskWrapper.GetDiskInfo().Item1;
                var totalSpace = diskWrapper.GetDiskInfo().Item2;

                var usage = freeSpace.Zip(totalSpace,
                            (f,t) => new DiskUsage { FreeSpace = f, TotalSpace =t });
                
                output.DiskUsageInfo = usage;
            }
            
            WriteObject(output);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            //
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
        }

    }

    public class SystemStats
    {
        public string SystemInfo { get; set; }
        public UInt64 TotalMemory { get; set; }
        public UInt64 UsedMemory { get; set; }

        public UInt64 AvailableMemory { get; set; }

        public double AvgProcessorUsage { get; set; }

        public string ProcessorCoreUsage { get; set; }

        public double OneMinLoadAverage { get; set; }

        public double FiveMinLoadAverage { get; set; }

        public double FifthteenMinLoadAverage { get; set; }

        public IEnumerable<DiskUsage> DiskUsageInfo { get; set; }

    }

    public class DiskUsage
    {
        public UInt64 FreeSpace { get; set; }
        public UInt64 TotalSpace { get; set; }
    }

    

}
