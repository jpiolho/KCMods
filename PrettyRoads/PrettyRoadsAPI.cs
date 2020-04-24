using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JPiolho.KingdomsAndCastles.PrettyRoads.API
{
    /// <summary>
    /// API to interface with 'Pretty Roads' mod.
    /// </summary>
    public static class PrettyRoadsAPI
    {
        private static class Delegates
        {
            public delegate string GetVersion__Delegate();
            public static GetVersion__Delegate GetVersion { get; internal set; }

            public delegate void RegisterRoad__Delegate(int buildingHash, Dictionary<string, Transform> models, string roadEntriesJson);
            public static RegisterRoad__Delegate RegisterRoad { get; internal set; }
        }

        /// <summary>
        /// Returns true if 'Pretty Roads' mod is installed.
        /// </summary>
        public static bool IsInstalled { get; private set; }

        /// <summary>
        /// Initializes Pretty Roads API. This method should be called before using any API methods, but only if <see cref="IsInstalled"/> returns true.
        /// </summary>
        /// <param name="modGameObject">Should be the GameObject that contains all the mods. (Usually this will be this.gameObject from the calling method).</param>
        public static void Initialize(GameObject modGameObject)
        {
            var mod = modGameObject.GetComponent("PrettyRoadsMod");
            IsInstalled = mod != null;

            if (!IsInstalled)
                return;

            var api = (Type)mod.GetType().GetProperty("API", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            var apiMethods = api.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (var apiMethod in apiMethods)
            {
                switch (apiMethod.Name.ToUpperInvariant())
                {
                    case "GETVERSION":
                        Delegates.GetVersion = (Delegates.GetVersion__Delegate)apiMethod.CreateDelegate(typeof(Delegates.GetVersion__Delegate));
                        break;
                    case "REGISTERROAD":
                        Delegates.RegisterRoad = (Delegates.RegisterRoad__Delegate)apiMethod.CreateDelegate(typeof(Delegates.RegisterRoad__Delegate));
                        break;
                }
            }
        }

        /// <summary>
        /// Returns 'Pretty Roads' version, in the following format: &lt;Major&gt;.&lt;Minor&gt;.&lt;Patch&gt;
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            return Delegates.GetVersion();
        }

        /// <summary>
        /// Registers a road building to be handled by Pretty Roads.
        /// </summary>
        /// <param name="buildingHash">The unique hash for your road building.</param>
        /// <param name="models">A string - model key value dictionary that will be used to reference all the models in the entries.</param>
        /// <param name="entries">All the road entries definitions.</param>
        public static void RegisterRoad(int buildingHash, Dictionary<string, Transform> models, params RoadEntry[] entries)
        {
            var entriesJson = JsonConvert.SerializeObject(entries, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            Delegates.RegisterRoad(buildingHash, models, entriesJson);
        }

    }

    #region Classes

    public class RoadEntry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Pieces
        {
            Straight,
            Elbow,
            ThreeWay,
            FourWay,
            Single,
            DeadEnd,
            ElbowFilled,
            FourWayFilled,
            FourWayHalfFilled,
            FourWayQuarterFilled,
            InnerCorner,
            ThreeWayFilled,
            ThreeWayHalfFilled,
            ThreeWayQuarterFilled,
            ThreeWayQuarterFilled2
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Presets
        {
            /// <summary>
            /// Uses the default behavior from the game.
            /// </summary>
            Default
        }

        /// <summary>
        /// Creates a new RoadEntry that is empty.
        /// </summary>
        public RoadEntry() { }
        /// <summary>
        /// Creates a new RoadEntry for the specified piece.
        /// </summary>
        public RoadEntry(Pieces piece)
        {
            this.Piece = piece;
        }

        #region Fields

        /// <summary>
        /// When specified, all properties except <see cref="Piece"/> will be ignored and instead the values for them will be driven from a built-in preset. 
        /// This should be used whenever possible to avoid instantiating objects that would do the exact same.
        /// </summary>
        public Presets? Preset { get; set; }
        /// <summary>
        /// This indicates which Piece this RoadEntry object is for.
        /// </summary>
        public Pieces Piece { get; set; }
        /// <summary>
        /// Specifies which model this road entry should use.
        /// The string is a reference to a key in the <see cref="Model"/> parameter in `PrettyRoadsAPI::RegisterRoad`.
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// When specified, the model will have this value added to the Z angle.
        /// This is useful if you wish to reuse an existing model that just needs a rotation or if the angle the model you have is in another orientation.
        /// </summary>
        public float? OffsetAngle { get; set; }
        /// <summary>
        /// When specified, this variation method will be applied.
        /// </summary>
        public RoadVariationMethod VariationMethod { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the <see cref="Piece"/> property.
        /// Allows chain calling.
        /// </summary>
        public RoadEntry ForPiece(RoadEntry.Pieces piece)
        {
            this.Piece = piece;
            return this;
        }
        /// <summary>
        /// Sets the <see cref="OffsetAngle"/> property.
        /// Allows chain calling.
        /// </summary>
        public RoadEntry SetOffsetAngle(float angle)
        {
            this.OffsetAngle = angle;
            return this;
        }
        /// <summary>
        /// Sets the <see cref="Model"/> property.
        /// Allows chain calling.
        /// </summary>
        public RoadEntry UseModel(string modelName)
        {
            this.Model = modelName;
            return this;
        }
        /// <summary>
        /// Sets the <see cref="Preset"/> property.
        /// Allows chain calling.
        /// </summary>
        public RoadEntry AsPreset(RoadEntry.Presets preset)
        {
            this.Preset = preset;
            return this;
        }

        #endregion
    }

    #region Variation Methods

    /// <summary>
    /// Base class for a variation method.
    /// You should not use this.
    /// </summary>
    public abstract class RoadVariationMethod
    {
    }

    /// <summary>
    /// Variation method to choose a random model
    /// </summary>
    public class RandomModelVariationMethod : RoadVariationMethod
    {
        #region Fields

        /// <summary>
        /// If set to true, the original <see cref="RoadEntry.Model"/> specified in the RoadEntry will be included in the random pool.
        /// </summary>
        public bool IncludeDefault { get; set; }
        /// <summary>
        /// Array containing all the possible models.
        /// </summary>
        public string[] Models { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the <see cref="IncludeDefault"/> property.
        /// Allows chaining.
        /// </summary>
        public RandomModelVariationMethod SetIncludeDefault(bool includeDefault)
        {
            this.IncludeDefault = includeDefault;
            return this;
        }
        /// <summary>
        /// Sets the <see cref="Models"/> property.
        /// Allows chaining.
        /// </summary>
        public RandomModelVariationMethod WithModels(ICollection<string> models)
        {
            return WithModels(models.ToArray<string>());
        }
        /// <summary>
        /// Sets the <see cref="Models"/> property.
        /// Allows chaining.
        /// </summary>
        public RandomModelVariationMethod WithModels(params string[] models)
        {
            this.Models = models;
            return this;
        }

        #endregion
    }
    /// <summary>
    /// Variation method to choose a random rotation for the model.
    /// </summary>
    public class RandomRotationVariationMethod : RoadVariationMethod
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Presets
        {
            /// <summary>
            /// Possible angles: 0, 90, 180, 270.
            /// </summary>
            Default360,
            /// <summary>
            /// Possible angles: 0, 180.
            /// </summary>
            Default0or180,
            /// <summary>
            /// Possible angles: 0, 90.
            /// </summary>
            Default0or90
        }

        #region Fields

        /// <summary>
        /// When specified, ignores all other properties and instead the values for them will be driven from a built-in preset.
        /// This should be used whenever possible to avoid instantiating objects that would do the exact same.
        /// </summary>
        public Presets? Preset { get; set; }
        /// <summary>
        /// An array containing all possible Z angle rotations.
        /// </summary>
        public float[] AllowedRotations { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets <see cref="Preset"/> property.
        /// Allows chaining.
        /// </summary>
        public RandomRotationVariationMethod AsPreset(Presets preset)
        {
            this.Preset = preset;
            return this;
        }

        #endregion
    }

    #endregion

    #endregion
}
