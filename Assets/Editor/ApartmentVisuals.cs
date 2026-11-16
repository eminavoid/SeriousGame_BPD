#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SeriousGameBPD.EditorTools
{
    /// <summary>
    /// Texturas que se repiten y asignación de los shaders del departamento.
    /// Las paredes del FBX casi no traen UV, así que el shader proyecta en mundo o en el objeto.
    /// </summary>
    public static class ApartmentVisuals
    {
        const string TextureFolder = "Assets/Materials/Apartment/Textures/Surface";
        const string MaterialFolder = "Assets/Materials/Apartment/Fbx";
        const int Size = 512;

        enum Kind
        {
            Surface,
            Glass,
            Screen
        }

        struct Profile
        {
            public Kind Kind;
            public string Albedo;
            public string Normal;
            public float Tile;
            public float Hard;
            public float Smooth;
            public float Metal;
            public float Bump;
            public float Macro;
            public float Fresnel;
            public float Mapping;
            public Vector4 Planar;
            public Vector4 ScreenSize;
            public Color Fallback;
            public bool ForceWhite;
            public bool ForceDark;
        }

        static readonly Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>
        {
            { "M_Alfombra", Surf("Rug", "Rug_N", 0.5f, 1f, 0.08f, 0f, 0.55f, 0f, 1f, new Vector4(0.5f, 0.77f, 0.5f, 0.5f), true, false, 1f, 1f, 1f) },
            { "M_Ceramica_Blanca", Surf("Tile", "Tile_N", 1.35f, 0.35f, 0.55f, 0f, 0.85f, 0.08f, 0f, Vector4.zero, false, false, 0.95f, 0.95f, 0.93f) },
            { "M_Metal_Gastado", Surf("Metal", "Metal_N", 3.2f, 0.75f, 0.48f, 0.72f, 0.8f, 0.12f, 0f, Vector4.zero, false, false, 0.55f, 0.57f, 0.6f) },
            { "M_Plastico_Negro", Surf("Plaster", "Plaster_N", 5f, 0.4f, 0.42f, 0f, 0.25f, 0.05f, 0f, Vector4.zero, false, false, 0.12f, 0.12f, 0.13f) },
            { "M_Vidrio", Glass(0.92f, 0.55f, 0.75f, 0.86f, 0.92f) },
            { "M_Madera", Surf("Wood", "Wood_N", 1.15f, 0.9f, 0.28f, 0f, 1f, 0.35f, 0f, Vector4.zero, false, false, 0.64f, 0.48f, 0.32f) },
            { "M_Lino", Surf("Bed_Sheet", "Bed_Sheet_N", 2f, 0f, 0.08f, 0f, 0.45f, 0.06f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Lino_Manta", Surf("Bed_Blanket", "Bed_Blanket_N", 1.5f, 0f, 0.07f, 0f, 0.7f, 0.05f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Celular_Funda", Surf("Fabric", "Fabric_N", 7f, 0.15f, 0.3f, 0f, 0.3f, 0.05f, 0f, Vector4.zero, false, false, 0.82f, 0.36f, 0.24f) },
            { "M_Pantalla", Screen(new Vector4(0.08f, 0.165f, 0f, 0f)) },
            { "M_Madera_Oscura", Surf("Wood", "Wood_N", 1.25f, 0.95f, 0.26f, 0f, 1f, 0.3f, 0f, Vector4.zero, false, false, 0.42f, 0.3f, 0.2f) },
            { "M_Cuaderno", Surf("Journal_Cover", "Journal_Cover_N", 1f, 0.85f, 0.18f, 0f, 0.3f, 0.02f, 1f, new Vector4(5.9f, 4.35f, 0.5f, 0.5f), true, false, 1f, 1f, 1f) },
            { "M_Papel", Surf("Journal_Page", "Journal_Page_N", 1f, 1f, 0.08f, 0f, 0.12f, 0f, 1f, new Vector4(5.9f, 4.35f, 0.5f, 0.5f), true, false, 1f, 1f, 1f) },
            { "M_Marco_Blanco", Surf("Plaster", "Plaster_N", 2.4f, 1f, 0.32f, 0f, 0.3f, 0.1f, 0f, Vector4.zero, false, false, 0.9f, 0.89f, 0.87f) },
            { "M_Espejo", Glass(0.97f, 0.95f, 0.18f, 0.19f, 0.2f) },
            { "M_Aluminio", Surf("Metal", "Metal_N", 4.5f, 0.5f, 0.55f, 0.78f, 0.65f, 0.08f, 0f, Vector4.zero, false, false, 0.72f, 0.73f, 0.75f) },
            { "M_Pared_Bano", Surf("Tile", "Tile_N", 1.6f, 1f, 0.28f, 0f, 0.7f, 0.05f, 0f, Vector4.zero, false, false, 0.64f, 0.69f, 0.73f) },
            { "M_Pared_Estar", Surf("Plaster", "Plaster_N", 0.55f, 1f, 0.08f, 0f, 0.55f, 0.45f, 0f, Vector4.zero, false, false, 0.65f, 0.69f, 0.62f) },
            { "M_Pared_Dormitorio", Surf("Plaster", "Plaster_N", 0.5f, 1f, 0.08f, 0f, 0.5f, 0.4f, 0f, Vector4.zero, false, false, 0.9f, 0.82f, 0.73f) },
            { "M_Piso_Ceramico", Surf("Tile", "Tile_N", 0.9f, 1f, 0.42f, 0.02f, 0.75f, 0.08f, 0f, Vector4.zero, false, false, 0.76f, 0.78f, 0.79f) },
            { "M_Piso_Porcelanato", Surf("Porcelanato_Cocina", "Porcelanato_Cocina_N", 0.55f, 1f, 0.4f, 0.02f, 0.65f, 0.04f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Piso_Madera", Surf("Wood", "Wood_N", 0.85f, 1f, 0.24f, 0f, 1f, 0.28f, 0f, Vector4.zero, false, false, 0.54f, 0.4f, 0.28f) },
            { "M_Tela_Oscura", Surf("Couch_Fabric", "Couch_Fabric_N", 2f, 0f, 0.07f, 0f, 0.5f, 0.08f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Taza", Surf("Tile", "Tile_N", 6f, 0.2f, 0.5f, 0f, 0.45f, 0.05f, 0f, Vector4.zero, false, false, 0.84f, 0.64f, 0.25f) },
            { "M_Mesada", Surf("Counter_Stone", "Counter_Stone_N", 1.35f, 0.7f, 0.42f, 0.02f, 0.35f, 0.06f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Mesa", Surf("TableGrain", "TableGrain_N", 1.05f, 0.85f, 0.32f, 0f, 0.85f, 0.1f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Ceramica_Bacha", Surf("Ceramica_Bacha", "Ceramica_Bacha_N", 0.85f, 0.2f, 0.48f, 0f, 0.45f, 0.04f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Cama_Madera", Surf("Bed_Wood", "Bed_Wood_N", 1.15f, 0.9f, 0.28f, 0f, 0.8f, 0.08f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_MesaDeLuz_Cuerpo", Surf("Nightstand_Paint", "Nightstand_Paint_N", 1.8f, 0.85f, 0.22f, 0f, 0.45f, 0.05f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_MesaDeLuz_Tapa", Surf("Nightstand_Top", "Nightstand_Top_N", 1.4f, 0.9f, 0.3f, 0f, 0.7f, 0.12f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Comoda_Cuerpo", Surf("Dresser_Body", "Dresser_Body_N", 1.05f, 0.9f, 0.24f, 0f, 0.75f, 0.1f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Comoda_Tapa", Surf("Dresser_Top", "Dresser_Top_N", 1.15f, 0.9f, 0.28f, 0f, 0.7f, 0.12f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Sillon_Madera", Surf("Couch_Wood", "Couch_Wood_N", 1.25f, 0.9f, 0.26f, 0f, 0.8f, 0.14f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
            { "M_Mesada_Bajo", Surf("Counter_Cabinet", "Counter_Cabinet_N", 1.1f, 0.9f, 0.26f, 0f, 0.75f, 0.14f, 0f, Vector4.zero, true, false, 1f, 1f, 1f) },
        };

        [MenuItem("SeriousGame BPD/Apply Apartment Visuals")]
        public static void ApplyFromMenu()
        {
            ApplyBatch();
        }

        public static void ApplyBatch()
        {
            GenerateTextures();
            foreach (var pair in Profiles)
                GetSharedMaterial(pair.Key);
            StyleExisting("Assets/Materials/Apartment/Ceiling.mat", Surf("Plaster", "Plaster_N", 0.42f, 1f, 0.06f, 0f, 0.4f, 0.5f, 0f, Vector4.zero, false, false, 0.9f, 0.9f, 0.88f));
            StyleExisting("Assets/Materials/Apartment/Linen.mat", Surf("Fabric", "Fabric_N", 2.2f, 0f, 0.1f, 0f, 0.35f, 0.1f, 0f, Vector4.zero, false, false, 0.85f, 0.8f, 0.72f));
            StyleExisting("Assets/Materials/Apartment/Dark.mat", Surf("Plaster", "Plaster_N", 8f, 1f, 0.35f, 0f, 0.2f, 0.05f, 0f, Vector4.zero, false, false, 0.15f, 0.15f, 0.16f));
            AssetDatabase.SaveAssets();
            ReportShader("Apartment/Surface");
            ReportShader("Apartment/Glass");
            ReportShader("Apartment/Screen");
            Debug.Log("Apartment visuals: texturas y shaders aplicados.");
        }

        static void ReportShader(string name)
        {
            var shader = Shader.Find(name);
            if (shader == null)
            {
                Debug.LogError("No se encontró " + name);
                return;
            }

            var messages = ShaderUtil.GetShaderMessages(shader);
            var errors = 0;
            foreach (var message in messages)
            {
                if (message.severity != UnityEditor.Rendering.ShaderCompilerMessageSeverity.Error)
                    continue;
                errors++;
                Debug.LogError(name + ": " + message.message + " (" + message.file + ":" + message.line + ")");
            }

            Debug.Log(name + " mensajes de error: " + errors);
        }

        public static bool Knows(string key)
        {
            return Profiles.ContainsKey(key);
        }

        public static Material GetSharedMaterial(string key)
        {
            if (!Profiles.TryGetValue(key, out var profile))
                return null;

            EnsureMaterialFolder();
            var path = MaterialFolder + "/" + key + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            var created = mat == null;
            if (created)
            {
                var shader = Shader.Find(ShaderName(profile.Kind));
                if (shader == null)
                    return null;
                mat = new Material(shader) { name = key };
                AssetDatabase.CreateAsset(mat, path);
            }

            Style(mat, profile, created);
            return mat;
        }

        static void StyleExisting(string path, Profile profile)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
                return;
            Style(mat, profile, false);
        }

        static void Style(Material mat, Profile profile, bool created)
        {
            var shader = Shader.Find(ShaderName(profile.Kind));
            if (shader == null)
            {
                Debug.LogError("No se compiló el shader " + ShaderName(profile.Kind));
                return;
            }

            var color = created || !mat.HasProperty("_BaseColor")
                ? profile.Fallback
                : mat.GetColor("_BaseColor");
            if (profile.ForceWhite)
                color = Color.white;
            if (profile.ForceDark)
                color = profile.Fallback;

            mat.shader = shader;
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_SpecColor"))
                mat.SetColor("_SpecColor", profile.Metal > 0.5f ? color : new Color(0.04f, 0.04f, 0.04f, 1f));
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", profile.Smooth);
            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", profile.Metal);
            if (mat.HasProperty("_BumpScale"))
                mat.SetFloat("_BumpScale", profile.Bump);
            if (mat.HasProperty("_Tile"))
                mat.SetFloat("_Tile", profile.Tile);
            if (mat.HasProperty("_Blend"))
                mat.SetFloat("_Blend", 6f);
            if (mat.HasProperty("_Mapping"))
                mat.SetFloat("_Mapping", profile.Mapping);
            if (mat.HasProperty("_PlanarScale"))
                mat.SetVector("_PlanarScale", profile.Planar);
            if (mat.HasProperty("_HardNormals"))
                mat.SetFloat("_HardNormals", profile.Hard);
            if (mat.HasProperty("_Macro"))
                mat.SetFloat("_Macro", profile.Macro);
            if (mat.HasProperty("_Fresnel"))
                mat.SetFloat("_Fresnel", profile.Fresnel);
            if (mat.HasProperty("_PlanarSize"))
                mat.SetVector("_PlanarSize", profile.ScreenSize);
            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", profile.Kind == Kind.Screen ? new Color(1.15f, 1.2f, 1.15f) : Color.black);

            if (profile.Kind == Kind.Surface)
            {
                mat.SetTexture("_BaseMap", LoadTex(profile.Albedo));
                mat.SetTexture("_BumpMap", LoadTex(profile.Normal));
            }
            else if (profile.Kind == Kind.Screen)
            {
                mat.SetTexture("_BaseMap", LoadTex("Screen"));
            }

            EditorUtility.SetDirty(mat);
        }

        static Texture2D LoadTex(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(TextureFolder + "/" + name + ".png");
        }

        static string ShaderName(Kind kind)
        {
            switch (kind)
            {
                case Kind.Glass: return "Apartment/Glass";
                case Kind.Screen: return "Apartment/Screen";
                default: return "Apartment/Surface";
            }
        }

        static Profile Surf(string albedo, string normal, float tile, float hard, float smooth, float metal, float bump, float macro, float mapping, Vector4 planar, bool forceWhite, bool forceDark, float r, float g, float b)
        {
            return new Profile
            {
                Kind = Kind.Surface,
                Albedo = albedo,
                Normal = normal,
                Tile = tile,
                Hard = hard,
                Smooth = smooth,
                Metal = metal,
                Bump = bump,
                Macro = macro,
                Mapping = mapping,
                Planar = planar,
                ForceWhite = forceWhite,
                ForceDark = forceDark,
                Fallback = new Color(r, g, b, 1f),
            };
        }

        static Profile Glass(float smooth, float fresnel, float r, float g, float b)
        {
            return new Profile
            {
                Kind = Kind.Glass,
                Smooth = smooth,
                Fresnel = fresnel,
                ForceDark = true,
                Fallback = new Color(r, g, b, 1f),
            };
        }

        static Profile Screen(Vector4 size)
        {
            return new Profile
            {
                Kind = Kind.Screen,
                ScreenSize = size,
            };
        }

        static void EnsureMaterialFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Apartment"))
                AssetDatabase.CreateFolder("Assets/Materials", "Apartment");
            if (!AssetDatabase.IsValidFolder(MaterialFolder))
                AssetDatabase.CreateFolder("Assets/Materials/Apartment", "Fbx");
        }

        static void GenerateTextures()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Apartment"))
                AssetDatabase.CreateFolder("Assets/Materials", "Apartment");
            if (!AssetDatabase.IsValidFolder("Assets/Materials/Apartment/Textures"))
                AssetDatabase.CreateFolder("Assets/Materials/Apartment", "Textures");
            if (!AssetDatabase.IsValidFolder(TextureFolder))
                AssetDatabase.CreateFolder("Assets/Materials/Apartment/Textures", "Surface");

            var wood = new float[Size * Size];
            var plaster = new float[Size * Size];
            var tile = new float[Size * Size];
            var fabric = new float[Size * Size];
            var metal = new float[Size * Size];
            var paper = new float[Size * Size];
            var rugH = new float[Size * Size];
            var rug = new Color[Size * Size];
            var screen = new Color[Size * Size];
            var flat = new float[Size * Size];

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var i = y * Size + x;
                    var u = x / (float)Size;
                    var v = y / (float)Size;
                    wood[i] = WoodHeight(u, v);
                    plaster[i] = PlasterHeight(u, v);
                    tile[i] = TileHeight(u, v);
                    fabric[i] = FabricHeight(u, v);
                    metal[i] = MetalHeight(u, v);
                    paper[i] = PaperHeight(u, v);
                    rugH[i] = RugHeight(u, v);
                    rug[i] = RugColor(u, v, rugH[i]);
                    screen[i] = ScreenColor(u, v);
                    flat[i] = 1f;
                }
            }

            SaveHeight("Wood", wood, false, 1f);
            SaveHeight("Plaster", plaster, false, 1f);
            SaveHeight("Tile", tile, false, 1f);
            SaveHeight("Fabric", fabric, false, 1f);
            SaveHeight("Metal", metal, false, 1f);
            SaveHeight("Paper", paper, false, 1f);
            SaveColor("Rug", rug, false);
            SaveColor("Screen", screen, true);
            SaveNormal("Wood_N", wood, 1.05f);
            SaveNormal("Plaster_N", plaster, 0.55f);
            SaveNormal("Tile_N", tile, 0.85f);
            SaveNormal("Fabric_N", fabric, 0.35f);
            SaveNormal("Metal_N", metal, 0.5f);
            SaveNormal("Rug_N", rugH, 0.45f);
            SaveNormal("Flat_N", flat, 0.01f);
        }

        static float WoodHeight(float u, float v)
        {
            const int planks = 6;
            var across = v * planks;
            var id = Mathf.FloorToInt(across);
            var seamT = across - id;
            var seam = Mathf.SmoothStep(0f, 0.14f, seamT) * Mathf.SmoothStep(0f, 0.14f, 1f - seamT);
            var grain = Fbm(u * 2f + id * 0.17f, id * 0.13f);
            var tone = 0.8f + Hash(id, 2) * 0.2f;
            var knotU = Mathf.Repeat(u * 2f, 1f);
            var knotV = seamT;
            var knot = Mathf.Abs(knotU - 0.4f) + Mathf.Abs(knotV - 0.55f);
            var knotMask = knot < 0.14f ? Mathf.Lerp(0.62f, 1f, knot / 0.14f) : 1f;
            return tone * Mathf.Lerp(0.86f, 1f, grain) * Mathf.Lerp(0.78f, 1f, seam) * knotMask;
        }

        static float PlasterHeight(float u, float v)
        {
            var broad = Fbm(u, v);
            var fine = TileNoise(u, v, 40);
            return Mathf.Lerp(0.82f, 1f, broad) * Mathf.Lerp(0.94f, 1f, fine);
        }

        static float TileHeight(float u, float v)
        {
            const int tiles = 4;
            var fx = Mathf.Repeat(u * tiles, 1f);
            var fy = Mathf.Repeat(v * tiles, 1f);
            var tx = Mathf.FloorToInt(u * tiles);
            var ty = Mathf.FloorToInt(v * tiles);
            var tone = 0.9f + Hash(tx + 4, ty + 9) * 0.1f;
            var speck = TileNoise(u * 2f, v * 2f, 12);
            var groutX = Mathf.SmoothStep(0f, 0.16f, fx) * Mathf.SmoothStep(0f, 0.16f, 1f - fx);
            var groutY = Mathf.SmoothStep(0f, 0.16f, fy) * Mathf.SmoothStep(0f, 0.16f, 1f - fy);
            var face = groutX * groutY;
            var tileFace = tone * Mathf.Lerp(0.96f, 1f, speck);
            return Mathf.Lerp(0.72f, tileFace, face);
        }

        static float FabricHeight(float u, float v)
        {
            var x = Mathf.FloorToInt(u * 28f);
            var y = Mathf.FloorToInt(v * 28f);
            var weave = ((x / 2) + (y / 2)) % 2 == 0;
            var n = Fbm(u * 1.5f, v * 1.5f);
            return (weave ? 0.92f : 1f) * Mathf.Lerp(0.94f, 1f, n);
        }

        static float MetalHeight(float u, float v)
        {
            var streak = TileNoise(0.15f, v * 5f, 10);
            var fine = TileNoise(u * 8f, v * 0.35f, 20);
            var scratch = Mathf.Repeat(u * 19f + v * 2.5f, 1f) < 0.015f ? 0.62f : 1f;
            return Mathf.Lerp(0.7f, 1f, streak) * Mathf.Lerp(0.86f, 1f, fine) * scratch;
        }

        static float PaperHeight(float u, float v)
        {
            var line = Mathf.Repeat(v * 18f, 1f) < 0.055f ? 0.7f : 1f;
            var fiber = Mathf.Lerp(0.93f, 1f, Fbm(u, v));
            return line * fiber;
        }

        static float RugHeight(float u, float v)
        {
            var border = u < 0.08f || u > 0.92f || v < 0.08f || v > 0.92f;
            var d = Mathf.Abs(u - 0.5f) + Mathf.Abs(v - 0.5f);
            var motif = d > 0.16f && d < 0.24f ? 0.7f : 1f;
            var pile = Mathf.Lerp(0.85f, 1f, Fbm(u * 3f, v * 3f));
            return (border ? 0.55f : 0.9f) * motif * pile;
        }

        static Color RugColor(float u, float v, float height)
        {
            var border = new Color(0.34f, 0.13f, 0.09f);
            var field = new Color(0.62f, 0.24f, 0.16f);
            var motif = new Color(0.86f, 0.62f, 0.36f);
            var edge = u < 0.07f || u > 0.93f || v < 0.07f || v > 0.93f;
            var frame = !edge && (u < 0.13f || u > 0.87f || v < 0.13f || v > 0.87f);
            var d = Mathf.Abs(u - 0.5f) + Mathf.Abs(v - 0.5f);
            var diamond = d > 0.16f && d < 0.23f;
            var color = edge || frame ? border : diamond ? motif : field;
            return color * Mathf.Lerp(0.82f, 1f, height);
        }

        static Color ScreenColor(float u, float v)
        {
            var bg = new Color(0.05f, 0.08f, 0.09f);
            var bar = new Color(0.1f, 0.14f, 0.15f);
            var card = new Color(0.16f, 0.32f, 0.28f);
            var card2 = new Color(0.12f, 0.16f, 0.18f);
            var ink = new Color(0.86f, 0.9f, 0.88f);
            var color = v > 0.9f ? bar : bg;
            if (Inside(u, v, 0.1f, 0.58f, 0.9f, 0.86f))
                color = card;
            if (Inside(u, v, 0.16f, 0.64f, 0.55f, 0.7f))
                color = ink;
            if (Inside(u, v, 0.1f, 0.36f, 0.78f, 0.52f))
                color = card2;
            if (Inside(u, v, 0.14f, 0.4f, 0.48f, 0.46f))
                color = new Color(0.45f, 0.55f, 0.52f);
            if (Inside(u, v, 0.12f, 0.08f, 0.88f, 0.16f))
                color = new Color(0.2f, 0.42f, 0.36f);
            return color;
        }

        static bool Inside(float u, float v, float x0, float y0, float x1, float y1)
        {
            return u >= x0 && u <= x1 && v >= y0 && v <= y1;
        }

        static void SaveHeight(string name, float[] height, bool clamp, float scale)
        {
            var pixels = new Color[height.Length];
            for (var i = 0; i < height.Length; i++)
            {
                var v = Mathf.Clamp01(height[i] * scale);
                pixels[i] = new Color(v, v, v, 1f);
            }

            Write(name, pixels, false, !clamp);
        }

        static void SaveColor(string name, Color[] pixels, bool clamp)
        {
            Write(name, pixels, false, !clamp);
        }

        static void SaveNormal(string name, float[] height, float strength)
        {
            var pixels = new Color[Size * Size];
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var hL = height[y * Size + (x - 1 + Size) % Size];
                    var hR = height[y * Size + (x + 1) % Size];
                    var hD = height[((y - 1 + Size) % Size) * Size + x];
                    var hU = height[((y + 1) % Size) * Size + x];
                    var dx = (hL - hR) * strength;
                    var dy = (hD - hU) * strength;
                    var n = new Vector3(dx, dy, 1f).normalized;
                    pixels[y * Size + x] = new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f, 1f);
                }
            }

            Write(name, pixels, true, true);
        }

        static void Write(string name, Color[] pixels, bool normal, bool repeat)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGB24, false);
            tex.SetPixels(pixels);
            tex.Apply();
            var path = TextureFolder + "/" + name + ".png";
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            importer.sRGBTexture = !normal;
            importer.mipmapEnabled = true;
            importer.wrapMode = repeat ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Trilinear;
            importer.anisoLevel = 16;
            if (normal)
                importer.convertToNormalmap = false;
            importer.SaveAndReimport();
        }

        static float Fbm(float u, float v)
        {
            return TileNoise(u, v, 4) * 0.5f
                   + TileNoise(u, v, 8) * 0.25f
                   + TileNoise(u, v, 16) * 0.15f
                   + TileNoise(u, v, 32) * 0.1f;
        }

        static float TileNoise(float u, float v, int cells)
        {
            var x = u * cells;
            var y = v * cells;
            var x0 = Mathf.FloorToInt(x);
            var y0 = Mathf.FloorToInt(y);
            var fx = Smooth(x - x0);
            var fy = Smooth(y - y0);
            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var a = Hash(Mod(x0, cells), Mod(y0, cells));
            var b = Hash(Mod(x1, cells), Mod(y0, cells));
            var c = Hash(Mod(x0, cells), Mod(y1, cells));
            var d = Hash(Mod(x1, cells), Mod(y1, cells));
            return Mathf.Lerp(Mathf.Lerp(a, b, fx), Mathf.Lerp(c, d, fy), fy);
        }

        static float Smooth(float t)
        {
            return t * t * (3f - 2f * t);
        }

        static int Mod(int value, int cells)
        {
            var m = value % cells;
            return m < 0 ? m + cells : m;
        }

        static float Hash(int x, int y)
        {
            unchecked
            {
                var n = (uint)(x * 374761393 + y * 668265263);
                n = (n ^ (n >> 13)) * 1274126177u;
                return (n & 16777215u) / 16777215f;
            }
        }
    }
}
#endif
