# Pretty Roads API
This is an API file for integrating with 'Pretty Roads' mod for the game 'Kingdoms & Castles'. The mod can be found here:
https://steamcommunity.com/sharedfiles/filedetails/?id=2057959069

# Quick start
```csharp
using JPiolho.KingdomsAndCastles.PrettyRoads.API;

public class YourMod : MonoBehaviour
{
    public void PreScriptLoad(KCModHelper helper)
    {
        PrettyRoadsAPI.Initialize(this.gameObject);
    }

    public void OnScriptLoad(KCModHelper helper)
    {
        if(PrettyRoadsAPI.IsInstalled) {
            // Add your road register here
        }
    }
}

```

# Documentation
## PrettyRoadsAPI
```csharp
public static class PrettyRoadsAPI
```
API to interface with 'Pretty Roads' mod

### Properties
```csharp
public static bool IsInstalled
```
Returns true if 'Pretty Roads' mod is installed

### Methods
```csharp
public static void Initialize(GameObject modGameObject)
```
Initializes Pretty Roads API. This method should be called before using any API methods, but only if `IsInstalled` returns true. 
`modGameObject`: Should be the GameObject that contains all the mods. (Usually this will be `this.gameObject` from the calling method)

---

```csharp
public static uint GetVersion()
```
Returns 'Pretty Roads' version. For example: 100 = 1.0.0, 110 = 1.1.0

---

```csharp
public static void RegisterRoad(int buildingHash, Dictionary<string,Transform> models, params RoadEntry[] entries)
```
Registers a road building to be handled by Pretty Roads. 
`buildingHash`: The unique hash for your road building
`models`: A string - model key value dictionary that will be used to reference all the models in the entries.
`entries`: All the road entries definitions

## RoadEntry
```csharp
public class RoadEntry
```
### Constructors
```csharp
public RoadEntry()
```
Creates a new RoadEntry that is empty.

---

```csharp
public RoadEntry(Pieces piece)
```
Creates a new RoadEntry for the specified piece.

### Properties

```csharp
public Presets? Preset { get; set; }
```
When specified, all properties except `Piece` will be ignored and instead the values for them will be driven from a built-in preset. This should be used whenever possible to avoid instantiating objects that would do the exact same. 

---

```csharp
public Pieces Piece { get; set; }
```
This indicates which Piece this RoadEntry object is for

---

```csharp
public string Model { get; set; }
```
Specifies which model this road entry should use. The string is a reference to a key in the `model` parameter in `PrettyRoadsAPI::RegisterRoad `. 

---

```csharp
public float? OffsetAngle { get; set; }
```
When specified, the model will have this value added to the Z angle. This is useful if you wish to reuse an existing model that just needs a rotation or if the angle the model you have is in another orientation.

---

```csharp
public RoadVariationMethod VariationMethod { get; set; }
```
When specified, this variation method will be applied.

### Methods

```csharp
public RoadEntry ForPiece(RoadEntry.Pieces piece)
```
Sets the `Piece` property. Allows chain calling.

---

```csharp
public RoadEntry SetOffsetAngle(float angle)
```
Sets the `OffsetAngle` property. Allows chain calling.

---

```csharp
public RoadEntry UseModel(string modelName)
```
Sets the `Model` property. Allows chain calling.

---

```csharp
public RoadEntry AsPreset(RoadEntry.Presets preset)
```
Sets the `Preset` property. Allows chain calling.

## RoadEntry.Piece
```csharp
public enum Pieces
```
|Enum                  |Description|
|----------------------|:---------:|
|Straight              |           |
|Elbow                 |           |
|ThreeWay              |           |
|FourWay               |           |
|Single                |           |
|DeadEnd               |           |
|ElbowFilled           |           |
|FourWayFilled         |           |
|FourWayHalfFilled     |           |
|FourWayQuarterFilled  |           |
|InnerCorner           |           |
|ThreeWayFilled        |           |
|ThreeWayHalfFilled    |           |
|ThreeWayQuarterFilled |           |
|ThreeWayQuarterFilled2|           |

## RoadEntry.Presets
```csharp
public enum Presets
```
|Enum   |              Description              |
|-------|:-------------------------------------:|
|Default|Uses the default behavior from the game|

## RoadVariationMethod
```csharp
public abstract class RoadVariationMethod
```
Base class for a variation method

## RandomModelVariationMethod
```csharp
public class RandomModelVariationMethod : RoadVariationMethod
```
Variation method to choose a random model

### Properties
```csharp
public bool IncludeDefault { get; set; }
```
If set to true, the original `Model` specified in the RoadEntry will be included in the random pool.

---

```csharp
public string[] Models { get; set; }
```
Array containing all the possible models.

### Methods

```csharp
public RandomModelVariationMethod SetIncludeDefault(bool includeDefault)
```
Sets the `IncludeDefault` property. Allows chaining.

---

```csharp
public RandomModelVariationMethod WithModels(ICollection<string> models)
```
Sets the `Models` property. Allows chaining.

## RandomRotationVariationMethod
```csharp
public class RandomRotationVariationMethod : RoadVariationMethod
```
Variation method to choose a random rotation for the model

### Properties

```csharp
public RandomRotationVariationMethod.Presets? Preset { get; set; }
```
When specified, ignores all other properties and instead the values for them will be driven from a built-in preset. This should be used whenever possible to avoid instantiating objects that would do the exact same. 

---

```csharp
public float[] AllowedRotations { get; set; }
```
An array containing all possible Z angle rotations.

### Methods

```csharp
public RandomRotationVariationMethod AsPreset(Presets preset)
```
Sets `Preset` property. Allows chaining.


