& dotnet build ./GetSystemStats
& dotnet build ./GetSystemStatsTest
& cargo build --manifest-path ./system-stats/Cargo.toml

if ($IsMacOs) {
    if($(Test-Path -Path 'system-stats/target/debug/libsystem_stats.dylib')) {
        # GetSystemStats Binary
        Copy-Item `
            -Path 'system-stats/target/debug/libsystem_stats.dylib' `
            -Destination './GetSystemStats/bin/Debug/netstandard2.1/'
    
            # GetSystemStatstests
        Copy-Item `
            -Path 'system-stats/target/debug/libsystem_stats.dylib' `
            -Destination './GetSystemStatsTest/bin/Debug/net5.0/'
    }
}

if ($IsLinux) {
    if ($(Test-Path -Path 'system-stats/target/debug/libsystem_stats.so')) {
        # GetSystemStats Binary
        Copy-Item `
            -Path 'system-stats/target/debug/libsystem_stats.so' `
            -Destination './GetSystemStats/bin/Debug/netstandard2.1/'

            # GetSystemStatstests
        Copy-Item `
            -Path 'system-stats/target/debug/libsystem_stats.so' `
            -Destination './GetSystemStatsTest/bin/Debug/net5.0/'
    }
}

if ($IsWindows) {
    if ($(Test-Path -Path 'system-stats/target/debug/libsystem_stats.dll')) {
        # GetSystemStats Binary
        Copy-Item `
            -Path 'system-stats/target/debug/libsystem_stats.dll' `
            -Destination './GetSystemStats/bin/Debug/netstandard2.1/'

            # GetSystemStatstests
        Copy-Item `
            -Path 'system-stats/target/debug/libsystem_stats.dll' `
            -Destination './GetSystemStatsTest/bin/Debug/net5.0/'
    }
}
