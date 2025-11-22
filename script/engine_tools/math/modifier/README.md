/[scripts](../../..)/[engine_tools](../..)/[math](..)/modifier

---

This folder contains generic structures to handle property modification explicitly, and some implementations.

- [PropertyModifier](#propertymodifier)
  - [Usage](#usage)
  - [Implementation](#implementation)
- [Additive Modifiers](#additive-modifiers)
  - [Usage](#usage-1)


# [PropertyModifier](MATH_PropertyModifiers.cs)

Defines a generic and abstract property modifier base structure, as we could imagine modifiers for more complex data.  

## Usage

You can add and remove individual modifier to this modifiers handler with `Add(T modifier)` and `Remove(T modifier)` or `modifiers + modifier`/`modifiers - modifier`.
```cs
public MATH_PropertyModifier<T> Add(T modifier);
public MATH_PropertyModifier<T> Remove(T modifier);
```

To compute the result of the stored modifiers, you should call the method `Result()` which will return a value of type `T`, which is typically a multiplier for the initial property.

> 💡 Maybe it would be more interesting to make this class a wrapper for an actual value, instead of an external modifier. For now, you need to have a property, and the property modifier handler, then call for example ``property*propertyModifiers.Result()``.  
> It could be a pure wrapper, containing an initial value, and the modifiers at the same place, and we would simply call ``property.Value()``, or even make it abstract by adding an implicit conversion !
>
> It would add a layer of complexity though, since the modifier isn't necessarily the same type of the property.
> For example, in PW_ConstantRecoil, we use `float` additive modifier, to scale a `Vector2` angle.  
> We will need two Generic types, and it might get unecessarily complex, although very easy from the user perspective.


## Implementation

Concrete implementation of a property modifier handler should simply implement the following method -
```cs
public abstract T Result();
```

This method represents how the multiple stored modifiers should be combined into a final result. See an example with [additive modifiers]().


# [Additive Modifiers](MATH_AdditiveModifiers.cs)

A concrete implementation of Property Modifiers, for **floating-point** additive modifiers.  
It means the result of aggregating the stored modifiers is simply the sum of all the modifiers + 1.

## Usage

The output of `Result()` is a **floating-point** coeficient, and should be used as `property*modifiers.Result()`.  
It means, for example :
- to **halve** a property, the sum of the modifiers should be `-0.5` (output of `Result()` will be `1-0.5 = 0.5`)
- to **double** a property, the sum of the modifiers should be `1`. (output of `Reslut()` will be `1+1 = 2`)