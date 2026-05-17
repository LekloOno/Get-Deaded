Fire contains an array of shots.  
When shots are shot, an event is risen with the number of shots as argument.  
When something is hit by the shots, the event is propagated from the shot to the fire, so Fire has its own Hit event.  
This event contains a ShotHitEventArgs, which might be ShotHitEventArgs.MISSED if it hits a wall or any target non considered as a hit.  
We might add a ShotHitEventArgs.ENV for environmental hits such as destructables, shields or whatever, to ignore them from statistics ?  

We will use these events to compute statistics, per fire mode.