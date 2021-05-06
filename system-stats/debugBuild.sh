#!/bin/bash

# For *nix platforms but not needed as the end goal of
# the c# peices is to build a PowerShell Cmdlet, which means one
# should already have PowerShell 7 or greater installed on their
# platform. 

# cargo debug build and copy binary to
# over to c# project
PLATFORM=$(uname)
if [ -e 'Cargo.toml' ]
then
    cargo build
else
    echo "Cargo.toml not found are you in the right directory"
    exit -1
fi

if [[ -e 'target/debug/libsystem_stats.dylib' ]] && [[ $PLATFORM = 'Darwin' ]]
then
    cp 'target/debug/libsystem_stats.dylib' ../GetSystemStats/bin/Debug/netstandard2.1/
fi

if [[ -e 'target/debug/libsystem_stats.so' ]] && [[ $PLATFORM = 'Linux' ]]
then
     cp 'target/debug/libsystem_stats.so' ../GetSystemStats/bin/Debug/netstandard2.1/
fi


