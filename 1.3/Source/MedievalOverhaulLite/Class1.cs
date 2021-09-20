using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MedievalOverhaulLite
{
    class MedievalOverhaulLite : Mod
    {
        public const string MO_PackageID = "DankPyon.Medieval.Overhaul";
        public MedievalOverhaulLite(ModContentPack pack) : base(pack)
        {
            new Harmony("MedievalOverhaulLite.Mod").PatchAll();
            ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
            {
                modPackageID = MO_PackageID,
                sourceFile = @"Erin's Mountain Animals/HideAndLeather.xml",
                patchesToSkip = new List<int> { 0 }
            });
            ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
            {
                modPackageID = MO_PackageID,
                sourceFile = @"HidesRelatedPatches/Replace_Leather_Stats.xml",
                patchesToSkip = new List<int> { 0, 1, 2, 3, 4, 5 }
            });
            ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
            {
                modPackageID = MO_PackageID,
                sourceFile = @"HidesRelatedPatches/Add_Hide.xml",
                patchesToSkip = new List<int> { 0, 1, 2, 3, 4 }
            });
            ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
            {
                modPackageID = MO_PackageID,
                sourceFile = @"Replace_MineableMetals.xml",
                patchesToSkip = new List<int> { 0, 1, 2, 3, 4, 5, 6  }
            });
            ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
            {
                modPackageID = MO_PackageID,
                sourceFile = @"Replace_Cotton.xml",
                patchesToSkip = new List<int> { 0 }
            });
            ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
            {
                modPackageID = MO_PackageID,
                sourceFile = @"Change_Resesarch.xml",
                patchesToSkip = new List<int> { 1 }
            });
            List<string> animalPatches = new List<string>
            {
                @"Vanilla Animals Expanded — Arid Shrubland/Races_Expanded_AridShrubland.xml",
                @"Vanilla Animals Expanded — Australia/Races_Expanded_Australia.xml",
                @"Vanilla Animals Expanded — Boreal Forest - Copy/Races_Expanded_BorealForest.xml",
                @"Vanilla Animals Expanded — Cats and Dogs/Races_Expanded_Cats_and_Dogs.xml",
                @"Vanilla Animals Expanded — Desert/Races_Expanded_Desert.xml",
                @"Vanilla Animals Expanded — Endangered/Races_Expanded_Endangered.xml",
                @"Vanilla Animals Expanded — Extreme Desert/Races_Expanded_ExtremeDesert.xml",
                @"Vanilla Animals Expanded — Ice Sheet/Races_Expanded_IceSheet.xml",
                @"Vanilla Animals Expanded — Temperate Forest/Races_Expanded_TemperateForest.xml",
                @"Vanilla Animals Expanded — Tropical Rainforest/Races_Expanded_TropicalRainforest.xml",
            };

            foreach (var patch in animalPatches)
            {
                ModContentPack_Patches.patchesToSkip.Add(new ModPatchSkip
                {
                    modPackageID = MO_PackageID,
                    sourceFile = patch,
                    patchesToSkip = new List<int> { 0 }
                });
            }
        }
    }

    public class ModPatchSkip
    {
        public string sourceFile;
        public string modPackageID;
        public List<int> patchesToSkip;
    }

    [HarmonyPatch(typeof(ModContentPack), "Patches", MethodType.Getter)]
    public static class ModContentPack_Patches
    {
        public static HashSet<ModPatchSkip> patchesToSkip = new HashSet<ModPatchSkip>();
        [HarmonyPriority(int.MaxValue)]
        public static IEnumerable<PatchOperation> Postfix(IEnumerable<PatchOperation> __result, ModContentPack __instance)
        {
            var list = __result.ToList();
            Dictionary<string, List<PatchOperation>> sourceFiles = new Dictionary<string, List<PatchOperation>>();
            List<PatchOperation> operationsToSkip = new List<PatchOperation>();
            foreach (var patchOperation in list)
            {
                var sourceFile = GetSourceFile(patchOperation);
                if (sourceFiles.ContainsKey(sourceFile))
                {
                    sourceFiles[sourceFile].Add(patchOperation);
                }
                else
                {
                    sourceFiles[sourceFile] = new List<PatchOperation> { patchOperation };
                }
            }

            foreach (var patchOperation in list)
            {
                foreach (var patchToSkip in patchesToSkip)
                {
                    if (patchToSkip.modPackageID == __instance.PackageIdPlayerFacing)
                    {
                        var sourceFile = GetSourceFile(patchOperation);
                        var id = sourceFiles[sourceFile].IndexOf(patchOperation);
                        //Log.Message("Iterating: ID: " + id + " - " + patchOperation + " - " + sourceFile);
                        if (patchToSkip.sourceFile == sourceFile)
                        {
                            if (patchToSkip.patchesToSkip.Contains(id))
                            {   
                                operationsToSkip.Add(patchOperation);
                            }
                        }
                    }
                }
            }
            return list.Where(x => !operationsToSkip.Contains(x));
        }

        private static string GetSourceFile(PatchOperation patchOperation)
        {
            var line = patchOperation.sourceFile;
            line = line.Replace("\\", "/");
            string toBeSearched = "/Patches/";
            int ix = line.IndexOf(toBeSearched);
            if (ix != -1)
            {
                return line.Substring(ix + toBeSearched.Length);
            }
            return "";
        }
    }
}
