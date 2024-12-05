# MonitorUtil

## Purpose
This utility quickly moves all windows between monitors specified w args.

My intended usage is to bind to forward and back mouse buttons to be able to quickly
move contents of other monitors onto my primary monitor.  

Additionally, this is a step towards the habit of saving and publishing my personal 
work product.

## Notes
This code uses the Windows Win32 API and specifically `user32.dll`

Warning - There is static state in the `WindowSvc` which is acceptable only beacuse 
of the nature of the lifetime of this application.  It would be good to refactor out, 
time permitting.

## Additional learning
I don't have advanced knowledge of Win32 API and accessing via deligates in this 
manner.  I used ChatGPT to assist with developing this solution.  Additionally, I am 
feeling out how to use ChatGPT effectively with my development.  In this scenario, it 
was helpful to get to 75% solution quickly but did have issues moving Maximized windows.
All in all, it was a net positive in efficiency using AI for this small project.

## How to use
Take note of a notional monitor configuration such as the following:

         _________  
        |         |  
        |  Top    |  
        | Monitor |  
        |    1    |  
        |_________|  
   _________   _________   _________  
  |         | |         | |         |  
  | Monitor | | Monitor | | Monitor |  
  |    0    | |    2    | |    3    |  
  |_________| |_________| |_________|  

The monitors are 0 base indexed from left to right.  Instances(not in this example) 
where monitors have the exact same left most horizontal alignment are then indexed top to 
bottom.

### To Shuffle the bottom 3 monitors to the right.
`MonitorUtil.exe 0 2 3`

### To Shuffle the bottom 3 monitors to the left.
`MonitorUtil.exe 3 2 0`



