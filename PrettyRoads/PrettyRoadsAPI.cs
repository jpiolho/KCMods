using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JPiolho.KingdomsAndCastles.PrettyRoads.API
{
    public static class PrettyRoadsAPI
    {
        private static class Delegates
        {
            public delegate uint GetVersion__Delegate();
            public static GetVersion__Delegate GetVersion { get; internal set; }

            public delegate void RegisterRoad__Delegate(int buildingHash, Dictionary<string, Transform> models, string roadEntriesJson);
            public static RegisterRoad__Delegate RegisterRoad { get; internal set; }
        }

        public static bool IsInstalled { get; private set; }

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


        public static uint GetVersion()
        {
            return Delegates.GetVersion();
        }

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
            Default
        }

        public RoadEntry() { }
        public RoadEntry(Pieces piece)
        {
            this.Piece = piece;
        }

        #region Fields

        public Presets? Preset { get; set; }
        public Pieces Piece { get; set; }
        public string Model { get; set; }
        public float? OffsetAngle { get; set; }
        public RoadVariationMethod VariationMethod { get; set; }

        #endregion

        #region Methods

        public RoadEntry ForPiece(RoadEntry.Pieces piece)
        {
            this.Piece = piece;
            return this;
        }
        public RoadEntry SetOffsetAngle(float angle)
        {
            this.OffsetAngle = angle;
            return this;
        }
        public RoadEntry UseModel(string modelName)
        {
            this.Model = modelName;
            return this;
        }
        public RoadEntry AsPreset(RoadEntry.Presets preset)
        {
            this.Preset = preset;
            return this;
        }

        #endregion
    }

    #region Variation Methods

    public abstract class RoadVariationMethod
    {
    }

    public class RandomModelVariationMethod : RoadVariationMethod
    {
        #region Fields

        public bool IncludeDefault { get; set; }
        public string[] Models { get; set; }

        #endregion

        #region Methods

        public RandomModelVariationMethod SetIncludeDefault(bool includeDefault)
        {
            this.IncludeDefault = includeDefault;
            return this;
        }
        public RandomModelVariationMethod WithModels(ICollection<string> models)
        {
            return WithModels(models.ToArray<string>());
        }
        public RandomModelVariationMethod WithModels(params string[] models)
        {
            this.Models = models;
            return this;
        }

        #endregion
    }
    public class RandomRotationVariationMethod : RoadVariationMethod
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Presets
        {
            Default360,
            Default0or180,
            Default0or90
        }

        #region Fields

        public Presets? Preset { get; set; }
        public float[] AllowedRotations { get; set; }

        #endregion

        #region Methods

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
