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
            public Material WoodDark;
            public Material Linen;
            public Material Pillow;
            public Material Blanket;
            public Material Rug;
            public Material Metal;
            public Material Dark;
            public Material Glass;
            public Material Phone;
            public Material Screen;
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
                Floor = create("Floor", new Color(0.45f, 0.32f, 0.22f)),
                Ceiling = create("Ceiling", new Color(0.86f, 0.85f, 0.82f)),
                BedWall = create("BedWall", new Color(0.62f, 0.46f, 0.42f)),
                LiveWall = create("LiveWall", new Color(0.40f, 0.48f, 0.50f)),
                BathWall = create("BathWall", new Color(0.62f, 0.68f, 0.70f)),
                Wood = create("Wood", new Color(0.50f, 0.33f, 0.20f)),
                WoodDark = create("WoodDark", new Color(0.30f, 0.18f, 0.11f)),
                Linen = create("Linen", new Color(0.72f, 0.64f, 0.54f)),
                Pillow = create("Pillow", new Color(0.86f, 0.84f, 0.78f)),
                Blanket = create("Blanket", new Color(0.55f, 0.40f, 0.42f)),
                Rug = create("Rug", new Color(0.42f, 0.24f, 0.22f)),
                Metal = create("Metal", new Color(0.72f, 0.74f, 0.76f)),
                Dark = create("Dark", new Color(0.10f, 0.11f, 0.12f)),
                Glass = create("Glass", new Color(0.55f, 0.72f, 0.80f)),
                Phone = create("Phone", new Color(0.08f, 0.09f, 0.11f)),
                Screen = create("Screen", Color.white),
                Couch = create("Couch", new Color(0.32f, 0.36f, 0.40f)),
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

            Box("Rug", root, new Vector3(7.4f, 0.015f, 4.15f), new Vector3(3.4f, 0.03f, 2.5f), palette.Rug);

            var bed = Prop("Bed", root, new Vector3(1.55f, 0f, 1.45f));
            Piece("Base", bed.transform, new Vector3(0f, 0.14f, 0.05f), new Vector3(2.05f, 0.28f, 1.7f), palette.Wood);
            Piece("Mattress", bed.transform, new Vector3(0f, 0.4f, 0.02f), new Vector3(1.95f, 0.26f, 1.6f), palette.Linen);
            Piece("Headboard", bed.transform, new Vector3(0f, 0.58f, -0.82f), new Vector3(2.1f, 1.05f, 0.1f), palette.WoodDark);
            Piece("Pillow", bed.transform, new Vector3(0f, 0.58f, -0.48f), new Vector3(1.15f, 0.14f, 0.4f), palette.Pillow);
            Piece("Blanket", bed.transform, new Vector3(0f, 0.56f, 0.28f), new Vector3(1.92f, 0.08f, 0.95f), palette.Blanket);
            SetCollider(bed, new Vector3(0f, 0.4f, 0f), new Vector3(2.1f, 0.85f, 1.8f));
            built.Bed = MakeInteractable(bed, WorldAnchorIds.Bed, "Acostarte / cerrar el día",
                "El colchón espera. Todavía no es de noche.");

            var nightstand = Prop("Nightstand", root, new Vector3(2.85f, 0f, 0.52f));
            Piece("Body", nightstand.transform, new Vector3(0f, 0.28f, 0f), new Vector3(0.48f, 0.56f, 0.46f), palette.Wood);
            Piece("Top", nightstand.transform, new Vector3(0f, 0.58f, 0f), new Vector3(0.54f, 0.05f, 0.52f), palette.WoodDark);
            Piece("Drawer", nightstand.transform, new Vector3(0f, 0.3f, 0.24f), new Vector3(0.36f, 0.16f, 0.02f), palette.WoodDark);

            var phone = Prop("Phone", root, new Vector3(2.85f, 0.63f, 0.52f));
            Piece("Body", phone.transform, new Vector3(0f, 0.008f, 0f), new Vector3(0.078f, 0.014f, 0.158f), palette.Phone);
            Piece("Screen", phone.transform, new Vector3(0f, 0.016f, 0f), new Vector3(0.064f, 0.006f, 0.13f), palette.Screen);
            SetCollider(phone, Vector3.zero, new Vector3(0.42f, 0.22f, 0.46f));
            built.PhoneVisual = phone.transform;
            built.Phone = MakeInteractable(phone, WorldAnchorIds.Phone, "Mirar el celular",
                "La pantalla está quieta. Por ahora.");

            var window = Prop("Window", root, new Vector3(0.09f, 1.48f, 3.3f));
            Piece("Sill", window.transform, new Vector3(0.05f, -0.58f, 0f), new Vector3(0.16f, 0.06f, 1.5f), palette.Wood);
            Piece("JambL", window.transform, new Vector3(0f, 0f, -0.66f), new Vector3(0.08f, 1.12f, 0.08f), palette.Wood);
            Piece("JambR", window.transform, new Vector3(0f, 0f, 0.66f), new Vector3(0.08f, 1.12f, 0.08f), palette.Wood);
            Piece("Head", window.transform, new Vector3(0f, 0.54f, 0f), new Vector3(0.08f, 0.08f, 1.4f), palette.Wood);
            Piece("Rail", window.transform, new Vector3(0f, -0.5f, 0f), new Vector3(0.08f, 0.08f, 1.4f), palette.Wood);
            Piece("Glass", window.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.03f, 0.96f, 1.24f), palette.Glass);
            Piece("MullionV", window.transform, new Vector3(0.02f, 0.02f, 0f), new Vector3(0.025f, 0.96f, 0.045f), palette.WoodDark);
            Piece("MullionH", window.transform, new Vector3(0.02f, 0.02f, 0f), new Vector3(0.025f, 0.045f, 1.24f), palette.WoodDark);
            SetCollider(window, new Vector3(0f, 0f, 0f), new Vector3(0.25f, 1.3f, 1.5f));
            built.Window = MakeInteractable(window, WorldAnchorIds.Window, "Mirar por la ventana",
                "Afuera el día ya empezó sin vos. Autos, alguien riendo, un perro. El mundo no espera.");

            var mirror = Prop("Mirror", root, new Vector3(3.2f, 1.52f, 10.3f));
            Piece("Frame", mirror.transform, Vector3.zero, new Vector3(0.82f, 1.05f, 0.05f), palette.WoodDark);
            Piece("Glass", mirror.transform, new Vector3(0f, 0.02f, -0.02f), new Vector3(0.66f, 0.86f, 0.02f), palette.Glass);
            SetCollider(mirror, Vector3.zero, new Vector3(0.9f, 1.15f, 0.25f));
            built.Mirror = MakeInteractable(mirror, WorldAnchorIds.Mirror, "Mirarte en el espejo",
                "Por un segundo no sabés bien quién sos hoy.");

            var sink = Prop("Sink", root, new Vector3(3.2f, 0f, 9.92f));
            Piece("Cabinet", sink.transform, new Vector3(0f, 0.36f, 0.02f), new Vector3(0.78f, 0.72f, 0.48f), palette.Wood);
            Piece("Counter", sink.transform, new Vector3(0f, 0.74f, 0f), new Vector3(0.86f, 0.06f, 0.54f), palette.Pillow);
            Piece("Basin", sink.transform, new Vector3(0f, 0.74f, -0.02f), new Vector3(0.42f, 0.05f, 0.28f), palette.Metal);
            Piece("Faucet", sink.transform, new Vector3(0f, 0.9f, 0.16f), new Vector3(0.06f, 0.22f, 0.06f), palette.Metal);
            Piece("Spout", sink.transform, new Vector3(0f, 0.98f, 0.04f), new Vector3(0.05f, 0.04f, 0.22f), palette.Metal);
            SetCollider(sink, new Vector3(0f, 0.45f, 0f), new Vector3(0.9f, 1.05f, 0.6f));
            built.Sink = MakeInteractable(sink, WorldAnchorIds.Sink, "Agua fría (TIPP)",
                "Agua fría en las muñecas. El cuerpo puede bajar un cambio.");

            var couch = Prop("Couch", root, new Vector3(7.15f, 0f, 4.7f));
            Piece("Seat", couch.transform, new Vector3(0f, 0.22f, 0.04f), new Vector3(1.85f, 0.32f, 0.72f), palette.Couch);
            Piece("Back", couch.transform, new Vector3(0f, 0.52f, -0.32f), new Vector3(1.9f, 0.62f, 0.18f), palette.Couch);
            Piece("ArmL", couch.transform, new Vector3(-0.9f, 0.38f, 0.02f), new Vector3(0.16f, 0.42f, 0.78f), palette.WoodDark);
            Piece("ArmR", couch.transform, new Vector3(0.9f, 0.38f, 0.02f), new Vector3(0.16f, 0.42f, 0.78f), palette.WoodDark);
            Piece("CushionL", couch.transform, new Vector3(-0.42f, 0.42f, 0.06f), new Vector3(0.78f, 0.12f, 0.58f), palette.Pillow);
            Piece("CushionR", couch.transform, new Vector3(0.42f, 0.42f, 0.06f), new Vector3(0.78f, 0.12f, 0.58f), palette.Pillow);
            SetCollider(couch, new Vector3(0f, 0.4f, 0f), new Vector3(2f, 0.85f, 0.95f));
            built.Couch = MakeInteractable(couch, WorldAnchorIds.Couch, "Sentarte un rato",
                "El sillón invita a desaparecer un rato. A veces ayuda. A veces es un pozo.");

            var table = Prop("Table", root, new Vector3(9.15f, 0f, 2.95f));
            Piece("Top", table.transform, new Vector3(0f, 0.74f, 0f), new Vector3(1.25f, 0.06f, 0.82f), palette.Wood);
            Leg(table.transform, new Vector3(-0.52f, 0.36f, -0.32f), palette.WoodDark);
            Leg(table.transform, new Vector3(0.52f, 0.36f, -0.32f), palette.WoodDark);
            Leg(table.transform, new Vector3(-0.52f, 0.36f, 0.32f), palette.WoodDark);
            Leg(table.transform, new Vector3(0.52f, 0.36f, 0.32f), palette.WoodDark);
            SetCollider(table, new Vector3(0f, 0.4f, 0f), new Vector3(1.25f, 0.8f, 0.85f));

            var laptop = Prop("Laptop", root, new Vector3(9.15f, 0.78f, 2.95f));
            Piece("Base", laptop.transform, new Vector3(0f, 0.015f, 0.02f), new Vector3(0.36f, 0.025f, 0.26f), palette.Dark);
            Piece("Lid", laptop.transform, new Vector3(0f, 0.032f, -0.01f), new Vector3(0.36f, 0.012f, 0.25f), palette.Phone);
            Piece("Notch", laptop.transform, new Vector3(0f, 0.02f, 0.15f), new Vector3(0.08f, 0.012f, 0.02f), palette.Metal);
            SetCollider(laptop, new Vector3(0f, 0.08f, 0f), new Vector3(0.55f, 0.28f, 0.48f));
            built.Laptop = MakeInteractable(laptop, WorldAnchorIds.Laptop, "Abrir el notebook",
                "La tapa cerrada. El trabajo puede esperar un poco.");

            var counter = Prop("Counter", root, new Vector3(12.95f, 0f, 3.45f));
            Piece("Cabinet", counter.transform, new Vector3(0f, 0.42f, 0f), new Vector3(0.62f, 0.84f, 1.2f), palette.Wood);
            Piece("Top", counter.transform, new Vector3(0f, 0.86f, 0f), new Vector3(0.72f, 0.05f, 1.3f), palette.WoodDark);
            Piece("Door", counter.transform, new Vector3(-0.32f, 0.4f, 0f), new Vector3(0.03f, 0.62f, 1.05f), palette.WoodDark);

            var coffee = Prop("Coffee", root, new Vector3(12.85f, 0.9f, 3.45f));
            Piece("Body", coffee.transform, new Vector3(0f, 0.16f, 0f), new Vector3(0.26f, 0.32f, 0.22f), palette.Dark);
            Piece("Carafe", coffee.transform, new Vector3(0f, 0.1f, 0f), new Vector3(0.14f, 0.16f, 0.14f), palette.Glass);
            Piece("Lid", coffee.transform, new Vector3(0f, 0.34f, 0f), new Vector3(0.28f, 0.05f, 0.24f), palette.Phone);
            Piece("Cup", coffee.transform, new Vector3(-0.24f, 0.05f, 0.12f), new Vector3(0.08f, 0.09f, 0.08f), palette.Pillow);
            SetCollider(coffee, new Vector3(0f, 0.16f, 0f), new Vector3(0.55f, 0.45f, 0.45f));
            built.Coffee = MakeInteractable(coffee, WorldAnchorIds.Coffee, "Hacer café",
                "El café ayuda un poco. No arregla la marea, pero calienta las manos.");

            var fridge = Prop("Fridge", root, new Vector3(12.75f, 0f, 4.95f));
            Piece("Body", fridge.transform, new Vector3(0.02f, 0.9f, 0f), new Vector3(0.66f, 1.8f, 0.64f), palette.Metal);
            Piece("Door", fridge.transform, new Vector3(-0.34f, 0.95f, 0f), new Vector3(0.05f, 1.62f, 0.6f), palette.Metal);
            Piece("Seam", fridge.transform, new Vector3(-0.37f, 1.35f, 0f), new Vector3(0.012f, 0.02f, 0.56f), palette.Dark);
            Piece("Handle", fridge.transform, new Vector3(-0.39f, 1.05f, 0.2f), new Vector3(0.04f, 0.36f, 0.045f), palette.Dark);
            SetCollider(fridge, new Vector3(0f, 0.9f, 0f), new Vector3(0.78f, 1.85f, 0.72f));
            built.Fridge = MakeInteractable(fridge, WorldAnchorIds.Fridge, "Abrir la heladera",
                "Luz, un resto de comida, nada que arregle el día. Igual es algo concreto.");

            // Hueco de la pared sur: x 10.35–11.65, plano z = 0. El marco y la hoja lo tapan.
            Box("DoorJambL", root, new Vector3(10.42f, 1.06f, 0f), new Vector3(0.14f, 2.12f, 0.2f), palette.WoodDark);
            Box("DoorJambR", root, new Vector3(11.58f, 1.06f, 0f), new Vector3(0.14f, 2.12f, 0.2f), palette.WoodDark);
            Box("DoorHeader", root, new Vector3(11f, 2.36f, 0f), new Vector3(1.36f, 0.58f, 0.2f), palette.WoodDark);

            var door = Prop("FrontDoor", root, new Vector3(11f, 0f, 0f));
            Piece("Leaf", door.transform, new Vector3(0f, 1.05f, 0f), new Vector3(1.08f, 2.08f, 0.055f), palette.Wood);
            Piece("PanelTop", door.transform, new Vector3(0f, 1.48f, 0.03f), new Vector3(0.72f, 0.62f, 0.018f), palette.WoodDark);
            Piece("PanelBottom", door.transform, new Vector3(0f, 0.62f, 0.03f), new Vector3(0.72f, 0.72f, 0.018f), palette.WoodDark);
            Piece("Handle", door.transform, new Vector3(0.4f, 1.02f, 0.05f), new Vector3(0.04f, 0.28f, 0.04f), palette.Metal);
            SetCollider(door, new Vector3(0f, 1.06f, 0f), new Vector3(1.28f, 2.14f, 0.22f));
            built.DoorVisual = door.transform;
            built.Door = MakeInteractable(door, WorldAnchorIds.Door, "Mirar la puerta",
                "Nadie. Solo la madera y tu alarma.");

            var lamp = Prop("CeilingLamp", root, new Vector3(6.4f, 2.65f, 4.2f));
            Piece("Cord", lamp.transform, new Vector3(0f, -0.14f, 0f), new Vector3(0.02f, 0.28f, 0.02f), palette.Dark);
            Piece("Shade", lamp.transform, new Vector3(0f, -0.32f, 0f), new Vector3(0.42f, 0.14f, 0.42f), palette.Linen);

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
            fillGo.transform.localPosition = new Vector3(6.4f, 2.15f, 4.2f);
            var fill = fillGo.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.range = 14f;
            fill.intensity = 2.6f;
            fill.color = new Color(1f, 0.9f, 0.75f);

            return built;
        }

        static void Leg(Transform parent, Vector3 pos, Material mat)
        {
            Piece("Leg", parent, pos, new Vector3(0.08f, 0.72f, 0.08f), mat);
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

        static GameObject Prop(string name, Transform parent, Vector3 pos)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }

        static void Piece(string name, Transform parent, Vector3 pos, Vector3 size, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = size;
            var collider = go.GetComponent<Collider>();
            if (collider != null)
                UnityEngine.Object.DestroyImmediate(collider);
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null && mat != null)
                renderer.sharedMaterial = mat;
        }

        static void SetCollider(GameObject go, Vector3 center, Vector3 size)
        {
            var box = go.GetComponent<BoxCollider>();
            if (box == null)
                box = go.AddComponent<BoxCollider>();
            box.center = center;
            box.size = size;
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
    }
}
