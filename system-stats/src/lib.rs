extern crate sysinfo;
extern crate libc;

use std::ffi::{CString};
use libc::c_char;

use sysinfo::{System, SystemExt, ProcessorExt, RefreshKind, DiskExt};
//use sysinfo::{ ProcessExt };
use std::thread::sleep;

#[repr(C)]
#[derive(Debug)]
pub struct LoadAvg {
    pub one: f64,
    pub five: f64,
    pub fifteen: f64,
}

#[repr(C)]
#[derive(Debug)]
pub struct DiskUsage {
    free_space: u64,
    total_space: u64,
}

#[repr(C)]
#[derive(Debug)]
pub struct DiskInfo {
    pub free_space: Vec<u64>,
    pub total_space: Vec<u64>,
    pub length: i32,

}

impl DiskInfo {
    fn new() -> DiskInfo {
        DiskInfo {
            free_space: Vec::new(),
            total_space: Vec::new(),
            length: 0,
        }
    }

    fn query_disk_usage(&mut self) {

        let sys = System::new_all();
        for disk in sys.get_disks() {
            let free_space = disk.get_available_space();
            let total_space = disk.get_total_space();
            self.free_space.push(free_space);
            self.total_space.push(total_space);
            self.length += 1;
        };
    }
    
    fn get_disk_usage_freespace(&self, index: usize) -> u64{
        self.free_space[index]        
    }

    fn get_disk_usage_totalspace(&self, index: usize) -> u64 {
        self.total_space[index]
    }
    
    fn get_disk_usage_diskcount(&self) -> i32 {
        self.length
    }
}

#[no_mangle]
pub extern "C" fn get_used_memory_info() -> u64 {
    let sys = System::new_all();
    
    sys.get_used_memory()
}

#[no_mangle]
pub extern "C" fn get_total_memory_info() -> u64 {
    let sys = System::new_all();
    sys.get_total_memory()
}

#[no_mangle]
pub extern "C" fn get_system_info() -> *mut c_char {

    let sys = System::new_all();
    let mut results = vec!();
    
    match sys.get_host_name() {
        Some(h) => results.push(h),
        _ => results.push("unknown hostname".to_string())
    }
    match sys.get_name() {
        Some(n) => results.push(n),
        _ => results.push("unknown".to_string())
    };

    match sys.get_os_version() {
        Some(v) => results.push(v),
        _ => results.push("version unknown".to_string())
    };
    

    let out = results.join(" - ");
    CString::new(out).unwrap().into_raw()
    
}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn free_system_info_results(s: *mut c_char) {

    // unsafe
    if s.is_null() {
        return;
    }
    CString::from_raw(s);
}

#[no_mangle]
pub extern "C" fn get_processor_usage() -> f64 {
    let mut sys = System::new();
    sys.refresh_cpu();
    sleep(std::time::Duration::from_millis(500));
    sys.refresh_cpu();
    let mut usage = vec!();
    for p in sys.get_processors() {
        usage.push(p.get_cpu_usage() as f64)
    }
    let total = usage.iter().fold(0.0, |u,v| {
        u + v
    });
    total / (sys.get_processors().len() as f64)
}

#[no_mangle]
pub extern "C" fn get_core_usage() -> *mut c_char {
    let mut sys = System::new_with_specifics(RefreshKind::new().with_cpu());
    sleep(std::time::Duration::from_millis(500));
    sys.refresh_cpu();

    let mut usage = vec!();
    for p in sys.get_processors() {
        //println!("CPU usage {:?}", p.get_cpu_usage());
        usage.push(p.get_cpu_usage().to_string())
    }

    let out = usage.join(" - ");
    CString::new(out).unwrap().into_raw()
} 

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn free_core_usage(s: *mut c_char) {

    // unsafe
    if s.is_null() {
        return;
    }
    CString::from_raw(s);
}

#[no_mangle]
pub extern "C" fn get_load_avg() -> *mut LoadAvg {
    let mut sys = System::new_with_specifics(RefreshKind::new().with_cpu());
    sys.refresh_all();
    sys.refresh_all();
    let load = sys.get_load_average(); 
    
    let load_avg = LoadAvg {
        one: load.one,
        five: load.five,
        fifteen: load.fifteen
    };
    
    Box::into_raw(Box::new(load_avg))  
}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn free_load_avg(s: *mut LoadAvg) {

    if s.is_null() {
        return;
    }
    
    // unsafe
    Box::from_raw(s);
        
}

#[no_mangle]
pub extern "C" fn get_free_memory() -> u64 {
    let sys = System::new_all();
    sys.get_available_memory()
}

#[no_mangle]
pub extern "C" fn disk_info_new() -> *mut DiskInfo {

    Box::into_raw(Box::new(DiskInfo::new()))
}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn free_disk_info(disk_info: *mut DiskInfo) {
    if disk_info.is_null() {
        return;
    }

    // unsafe
    Box::from_raw(disk_info);

}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn disk_info_query(disk_info_ptr: *mut DiskInfo) {
    
    // unsafe
    let disk_info = {
        assert!(! disk_info_ptr.is_null());  //panic if null
        &mut *disk_info_ptr
    };
    disk_info.query_disk_usage();
}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn disk_info_get_disk_freespace(disk_info_ptr: *mut DiskInfo,
                                                     index: usize) -> u64 {

    // unsafe
    let disk_info = {
        assert!(! disk_info_ptr.is_null());  //panic if null
        &*disk_info_ptr
    };
    disk_info.get_disk_usage_freespace(index)    
}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn disk_info_get_disk_totalspace(disk_info_ptr: *mut DiskInfo,
                                                index: usize) -> u64 {

    // unsafe
    let disk_info = {
        assert!(! disk_info_ptr.is_null());  // panic if pointer is null
        &*disk_info_ptr
    };

    disk_info.get_disk_usage_totalspace(index)
}

/// # Safety
///
/// This function should not be called if the pointer passed
/// in is null or invalid.  Pass in a valid pointer to free it exactly once.
#[no_mangle]
pub unsafe extern "C" fn disk_info_get_disk_usage_diskcount(disk_info_ptr: *mut DiskInfo)
                                                     -> i32 {
    
    // unsafe
    let disk_info = {
        assert!(! disk_info_ptr.is_null());  //panic if null
        &*disk_info_ptr
    };
    disk_info.get_disk_usage_diskcount()
}

// Not really a useful function for this purpose
// #[no_mangle]
// pub extern "C" fn get_processes() -> () {
//     let sys = System::new_all();
//     for (pid, process) in sys.get_processes() {
//         println!("{} - {}", pid, process.name());
//     }
// }

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn check_value_types() {
        let proc_usage = get_processor_usage();
        assert_eq!((proc_usage >= 0.0), true);

        let used_memory = get_used_memory_info();
        assert_eq!((used_memory >= 1), true);

        let total_memory = get_total_memory_info();
        assert_eq!((total_memory >= 1), true);

        let free_memory = get_free_memory();
        assert_eq!((free_memory >= 1),true);
    }
    
    #[test]
    fn check_reference_types() {
        let core_usage = get_core_usage();
        assert_ne!(core_usage.is_null(),true);
        free_core_usage(core_usage);

        let system_info = get_system_info();
        assert_ne!(system_info.is_null(), true);
        free_system_info_results(system_info);
    

        let load_average = get_load_avg();
        assert_ne!(load_average.is_null(), true);
        free_load_avg(load_average as *mut LoadAvg);

        let mut disk_info = DiskInfo::new();
        disk_info.query_disk_usage();

        assert_eq!((disk_info.length > 0),true);

        assert_eq!((disk_info.free_space[0] > 0), true);
        assert_eq!((disk_info.total_space[0] > 0), true);
        //free_disk_info(disk_info as *mut DiskInfo);
        //free_disk_info(disk_info);


    }

}
