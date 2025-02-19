~~Coyotte time but for ground ?~~
    ~~- Having a slight margin, like 30-50 ms in which friction is not applied after landing~~

~~Jump should override grounded state to prevent friction to apply~~

~~Handle properly the velocity cache.~~

Prevent Crouch clipping

~~Implement Slide~~

~~Add Slide force~~
~~Make so velocity is not projected on floor plane when sliding (slides faster downward, slower upward)~~

"Hold" Property on crouch should be bound to slide's.

Lerping is now frame rate independant, maybe use a similar method for the drag ?

~~Slide Force Decay~~

~~Dash~~

Can get a lot of height with the dash, should maybe nerf that.

~~Velocity cache still has a small bug - Since we set the velocity to real velocity, if you move backward, you're out of collision, trigger the cache so enter collision, backward, out of collision etc.. A fix is to only return the real velocity when you get out of the wall, but since the body is a capsule, it's possible that the first move collides into the wall, but move and slide manages to go past the wall ..~~

~~Directionnal horizontal dash when pressing walk keys~~

Clean actions code with dedicated class holding static instances - see PI_Direction