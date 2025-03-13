The health system is based on a linked list of GC_Health which are resources. In might be a little heavy, since they would be dupplicated for each instance of entity.
A solution could be to keep the resource linked list but for data only. From this data linked list, we build the *logic-side* linked list, which are just simple c# classes.
It makes it slightly less modular since you would have to add a specific data container for each new type of health.

Some fields of some class are computed at run time for editability purposes, but it could be computed on ready.