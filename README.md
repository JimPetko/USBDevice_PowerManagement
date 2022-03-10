# Untitled

Created: March 9, 2022 11:57 PM/
Last Edited Time: March 10, 2022 12:08 AM/
Status: Complete/

# Summary

Simple to use application for management of power settings for USB devices in Windows. Uses .Net Framework 4.6 but can safely be upgraded to 4.8 

# Background

Selective Suspend with some devices will make it difficult to resume use of a connected USB device when Windows recovers from a sleep state.

# Goals

One or 2 clicks to dis/enable the power settings relevant to All devices in a category.

# Proposed Solution

Use the Windows Management Interface and Windows Registry to toggle the power management properties for each device.

### Risks

Increased Power consumption when being used on a laptop will reduce duration of battery life.

### Open Questions

What other devices on the system can benefit from having suspend properties be easier to manage.

# ToDo List

Try to speed up process by using newer MS infrastructure library