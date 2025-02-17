// Whenever we hit a wall, cache velocity

// vel += CachedVel
// Else --
// Test move and collide
// If collide 
//      If wall
//          If Not Cached yet - Cache velocity
//          If Climbable - climb
//          Else MoveAndSlide
//      Else MoveAndSlide
//
// climbable
//      cache remaining velocity
//      try climb