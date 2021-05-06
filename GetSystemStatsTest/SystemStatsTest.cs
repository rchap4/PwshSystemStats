using Xunit;
using GetSystemStats;
using System.Linq;

namespace GetSystemStatsTest
{
    public class SystemStatTests
    {

        [Fact]
        public void Memory_Stats()
        {
            // Arrange
            var wrapper = new SystemStatWarapper();

            // Act
            var totalMemory = wrapper.GetTotalMemory();
            var usedMemory = wrapper.GetUsedMemory();
            var freeMemory = wrapper.GetAvailableMemory();

            // Assert
            Assert.True(totalMemory >= 0);
            Assert.True(usedMemory >= 0);
            Assert.True(freeMemory >= 0);

        }

        [Fact]
        public void Cpu_Stats()
        {
            // Arrange
            var wrapper = new SystemStatWarapper();

            // Act
            var processorUsage = wrapper.GetProcessorUsage();
            var loadAverage = wrapper.GetLoadAverage().AsStruct();
            var coreUsage = wrapper.GetCoreUsage().AsString();

            // Assert
            Assert.True(processorUsage >= 0);
            Assert.True(loadAverage.one >= 0);
            Assert.True(loadAverage.five >= 0);
            Assert.True(loadAverage.fifteen >= 0);
            Assert.True(coreUsage.Length >= 0);

        }

        [Fact]
        public void System_Info()
        {
            // Arrange
            var wrapper = new SystemStatWarapper();
            
            // Act
            var systemInfo = wrapper.GetSystemInfo().AsString();

            // Assert
            Assert.True(systemInfo.Length >= 0);

        }

        [Fact]
        public static void Disk_Info()
        {
            
            // Arrange
            var diskWrapper = new DiskStatWrapper();
            
            // Act
            diskWrapper.QueryDiskInfo();
            var diskFreespace = diskWrapper.GetDiskInfo().Item1.Select(d => new DiskUsage() { FreeSpace = d});
            var diskTotalspace = diskWrapper.GetDiskInfo().Item2.Select(d => new DiskUsage() { TotalSpace = d });
            
            // Assert
            var usage = diskFreespace.Zip(diskTotalspace);
            foreach(var d in usage)
            {
                Assert.True(d.First.FreeSpace > 0);
                Assert.True(d.Second.TotalSpace > 0);
            }
        }


    }
}
