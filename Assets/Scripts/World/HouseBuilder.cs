using System;
using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.World
{
    /// <summary>
    /// Arma la geometría del departamento. Lo usa el menú de editor para crear el prefab.
    /// En Play no se llama: la escena House instancia Assets/Prefabs/Apartment.prefab.
    /// </summary>
    public static class HouseBuilder
    {
        public class Palette
        {
            public Material Floor;
            public Material Ceiling;
            public Material BedWall;
            public Material LiveWall;
            public Material BathWall;
            public Material Wood;
            public Material Linen;
            public Material Metal;
            public Material Dark;
            public Material Glass;
            public Material Phone;
            public Material Couch;
        }

        public class BuiltHouse
        {
            public Transform Root;
            public Vector3 Spawn = new Vector3(2.6f, 1.05f, 3.8f);
            public float SpawnYaw = 90f;
            public Interactable Phone;
            public Interactable Door;
            public Interactable Mirror;
            public Interactable Sink;
            public Interactable Bed;
            public Interactable Laptop;
            public Interactable Window;
            public Interactable Couch;
            public Interactable Fridge;
            public Interactable Coffee;
            public Transform PhoneVisual;
            public Transform DoorVisual;
            public Light Sun;
        }

        public static Palette CreatePalette(Func<string, Color, Material> create)
        {
            return new Palette
            {
                Floor = create("Floor", new Color(0.38f, 0.30f, 0.24f)),
                Ceiling = create("Ceiling", new Color(0.82f, 0.81f, 0.78f)),
                BedWall = create("BedWall", new Color(0.58f, 0.44f, 0.42f)),
                LiveWall = create("LiveWall", new Color(0.34f, 0.42f, 0.44f)),
                BathWall = create("BathWall", new Color(0.48f, 0.54f, 0.58f)),
                Wood = create("Wood", new Color(0.42f, 0.28f, 0.18f)),
                Linen = create("Linen", new Color(0.62f, 0.55f, 0.48f)),
                Metal = create("Metal", new Color(0.55f, 0.58f, 0.62f)),
                Dark = create("Dark", new Color(0.12f, 0.13f, 0.15f)),
                Glass = create("Glass", new Color(0.45f, 0.62f, 0.72f)),
                Phone = create("Phone", new Color(0.08f, 0.09f, 0.12f)),
                Couch = create("Couch", new Color(0.28f, 0.32f, 0.36f)),
            };
        }

        public static BuiltHouse Build(Transform parent, Palette palette)
        {
            var built = new BuiltHouse();
            var root = new GameObject("Apartment").transform;
            root.SetParent(parent, false);
            built.Root = root;

            const float h = 2.65f;
            Box("Floor", root, new Vector3(6.8f, -0.5f, 5.2f), new Vector3(14.2f, 1.0f, 11.0f), palette.Floor);
            Box("Ceiling", root, new Vector3(6.8f, h + 0.08f, 5.2f), new Vector3(13.8f, 0.16f, 10.6f), palette.Ceiling);

            WallZ(root, 0f, 0f, 10.35f, h, palette.BedWall);
            WallZ(root, 0f, 11.65f, 13.6f, h, palette.LiveWall);
            WallZ(root, 6.5f, 5.2f, 13.6f, h, palette.LiveWall);
            WallZ(root, 6.2f, 0f, 1.0f, h, palette.BathWall);
            WallZ(root, 6.2f, 2.75f, 5.2f, h, palette.BathWall);
            WallZ(root, 10.4f, 0f, 4.4f, h, palette.BathWall);

            WallX(root, 0f, 0f, 10.4f, h, palette.BedWall);
            WallX(root, 13.6f, 0f, 6.5f, h, palette.LiveWall);
            WallX(root, 4.4f, 6.2f, 10.4f, h, palette.BathWall);
            WallX(root, 5.2f, 0f, 2.0f, h, palette.LiveWall);
            WallX(root, 5.2f, 4.25f, 6.2f, h, palette.LiveWall);

            var bed = Box("Bed", root, new Vector3(1.55f, 0.32f, 1.45f), new Vector3(2.05f, 0.64f, 1.75f), palette.Linen);
            built.Bed = MakeInteractable(bed, WorldAnchorIds.Bed, "Acostarte / cerrar el día",
                "El colchón espera. Todavía no es de noche.");

            Box("Nightstand", root, new Vector3(2.85f, 0.28f, 0.52f), new Vector3(0.5f, 0.56f, 0.5f), palette.Wood);
            built.PhoneVisual = Box("Phone", root, new Vector3(2.85f, 0.62f, 0.52f), new Vector3(0.09f, 0.03f, 0.18f), palette.Phone).transform;
            FatCollider(built.PhoneVisual.gameObject, 0.4f);
            built.Phone = MakeInteractable(built.PhoneVisual.gameObject, WorldAnchorIds.Phone, "Mirar el celular",
                "La pantalla está quieta. Por ahora.");

            var window = Box("Window", root, new Vector3(0.12f, 1.45f, 3.3f), new Vector3(0.08f, 1.05f, 1.3f), palette.Glass);
            built.Window = MakeInteractable(window, WorldAnchorIds.Window, "Mirar por la ventana",
                "Afuera el día ya empezó sin vos. Autos, alguien riendo, un perro. El mundo no espera.");

            var mirror = Box("Mirror", root, new Vector3(3.85f, 1.55f, 8.35f), new Vector3(0.06f, 0.95f, 0.7f), palette.Glass);
            FatCollider(mirror, 0.45f);
            built.Mirror = MakeInteractable(mirror, WorldAnchorIds.Mirror, "Mirarte en el espejo",
                "Por un segundo no sabés bien quién sos hoy.");

            var sink = Box("Sink", root, new Vector3(3.25f, 0.45f, 8.35f), new Vector3(0.7f, 0.9f, 0.5f), palette.Metal);
            built.Sink = MakeInteractable(sink, WorldAnchorIds.Sink, "Agua fría (TIPP)",
                "Agua fría en las muñecas. El cuerpo puede bajar un cambio.");

            var couch = Box("Couch", root, new Vector3(7.15f, 0.38f, 4.7f), new Vector3(1.9f, 0.76f, 0.85f), palette.Couch);
            built.Couch = MakeInteractable(couch, WorldAnchorIds.Couch, "Sentarte un rato",
                "El sillón invita a desaparecer un rato. A veces ayuda. A veces es un pozo.");

            Box("Table", root, new Vector3(9.15f, 0.38f, 2.95f), new Vector3(1.25f, 0.76f, 0.85f), palette.Wood);
            var laptop = Box("Laptop", root, new Vector3(9.15f, 0.82f, 2.95f), new Vector3(0.38f, 0.05f, 0.28f), palette.Dark);
            FatCollider(laptop, 0.5f);
            built.Laptop = MakeInteractable(laptop, WorldAnchorIds.Laptop, "Abrir el notebook",
                "La tapa cerrada. El trabajo puede esperar un poco.");

            var fridge = Box("Fridge", root, new Vector3(12.75f, 0.9f, 4.85f), new Vector3(0.7f, 1.8f, 0.7f), palette.Metal);
            built.Fridge = MakeInteractable(fridge, WorldAnchorIds.Fridge, "Abrir la heladera",
                "Luz, un resto de comida, nada que arregle el día. Igual es algo concreto.");

            var coffee = Box("Coffee", root, new Vector3(12.7f, 1.05f, 3.55f), new Vector3(0.28f, 0.38f, 0.28f), palette.Dark);
            FatCollider(coffee, 0.4f);
            built.Coffee = MakeInteractable(coffee, WorldAnchorIds.Coffee, "Hacer café",
                "El café ayuda un poco. No arregla la marea, pero calienta las manos.");

            built.DoorVisual = Box("FrontDoor", root, new Vector3(11.0f, 1.1f, 0.12f), new Vector3(1.08f, 2.2f, 0.1f), palette.Wood).transform;
            FatCollider(built.DoorVisual.gameObject, 0.35f);
            built.Door = MakeInteractable(built.DoorVisual.gameObject, WorldAnchorIds.Door, "Mirar la puerta",
                "Nadie. Solo la madera y tu alarma.");

            var sunGo = new GameObject("Sun");
            sunGo.transform.SetParent(root, false);
            sunGo.transform.localRotation = Quaternion.Euler(50f, -30f, 0f);
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.96f, 0.9f);
            sun.intensity = 1.55f;
            sun.shadows = LightShadows.Soft;
            built.Sun = sun;

            var fillGo = new GameObject("FillLight");
            fillGo.transform.SetParent(root, false);
            fillGo.transform.localPosition = new Vector3(6f, 2.2f, 4f);
            var fill = fillGo.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.range = 14f;
            fill.intensity = 2.4f;
            fill.color = new Color(0.85f, 0.82f, 0.78f);

            return built;
        }

        static void WallX(Transform parent, float x, float z0, float z1, float height, Material mat)
        {
            float z = (z0 + z1) * 0.5f;
            float len = Mathf.Abs(z1 - z0);
            if (len < 0.05f) return;
            Box($"WallX_{x:0.0}_{z0:0.0}", parent, new Vector3(x, height * 0.5f, z), new Vector3(0.16f, height, len), mat);
        }

        static void WallZ(Transform parent, float z, float x0, float x1, float height, Material mat)
        {
            float x = (x0 + x1) * 0.5f;
            float len = Mathf.Abs(x1 - x0);
            if (len < 0.05f) return;
            Box($"WallZ_{z:0.0}_{x0:0.0}", parent, new Vector3(x, height * 0.5f, z), new Vector3(len, height, 0.16f), mat);
        }

        static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = size;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null && mat != null)
                renderer.sharedMaterial = mat;
            return go;
        }

        static Interactable MakeInteractable(GameObject go, string id, string prompt, string thought)
        {
            var interactable = go.GetComponent<Interactable>();
            if (interactable == null)
                interactable = go.AddComponent<Interactable>();
            interactable.AnchorId = id;
            interactable.Prompt = prompt;
            interactable.IdleThought = thought;
            interactable.CanInteract = true;
            return interactable;
        }

        static void FatCollider(GameObject go, float extra)
        {
            var box = go.GetComponent<BoxCollider>();
            if (box == null) box = go.AddComponent<BoxCollider>();
            box.size = new Vector3(
                1f + extra / Mathf.Max(0.05f, go.transform.localScale.x),
                1f + extra / Mathf.Max(0.05f, go.transform.localScale.y),
                1f + extra / Mathf.Max(0.05f, go.transform.localScale.z));
        }
    }
}
