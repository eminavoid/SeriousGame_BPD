using System;
using System.Collections.Generic;
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

        /// <summary>
        /// FBX del departamento. Cada archivo trae la posición que le dio el modelado
        /// (el origen del prefab queda en el origen de la casa).
        /// </summary>
        public class ModelLibrary
        {
            public GameObject Paredes;
            public GameObject Alfombra;
            public GameObject Banera;
            public GameObject Cafetera;
            public GameObject Cama;
            public GameObject Celular;
            public GameObject Comoda;
            public GameObject Cuaderno;
            public GameObject Espejo;
            public GameObject Heladera;
            public GameObject Inodoro;
            public GameObject Lavamanos;
            public GameObject Mesa;
            public GameObject Mesada;
            public GameObject MesaDeLuz;
            public GameObject Notebook;
            public GameObject Puerta;
            public GameObject Sillon;
            public GameObject Taza;
            public GameObject Ventana;
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

        public static BuiltHouse Build(Transform parent, Palette palette, ModelLibrary models = null)
        {
            if (models != null && models.Paredes != null)
                return BuildFromModels(parent, palette, models);

            var built = new BuiltHouse();
            var root = new GameObject("Apartment").transform;
            root.SetParent(parent, false);
            built.Root = root;

            const float h = 2.74f;
            const float half = 0.08f;
            // Piso y techo en L: el living no sigue hacia el noreste, donde no hay paredes.
            Box("Floor", root, new Vector3(6.82f, -0.5f, 3.12f), new Vector3(14.04f, 1f, 6.64f), palette.Floor);
            Box("FloorBath", root, new Vector3(2.2f, -0.5f, 8.35f), new Vector3(4.8f, 1f, 4.6f), palette.Floor);
            Box("Ceiling", root, new Vector3(6.82f, 2.82f, 3.12f), new Vector3(14.04f, 0.2f, 6.64f), palette.Ceiling);
            Box("CeilingBath", root, new Vector3(2.2f, 2.82f, 8.35f), new Vector3(4.8f, 0.2f, 4.6f), palette.Ceiling);

            // Cada tramo entra en la pared perpendicular (half) y no deja una rendija en la esquina.
            // El fondo del living y el del baño comparten z = 6.2, así no queda un escalón abierto.
            WallZ(root, "WallSouthL", 0f, 0f, 10.35f, h, palette.BedWall, half, 0.04f);
            WallZ(root, "WallSouthR", 0f, 11.65f, 13.6f, h, palette.LiveWall, 0.04f, half);
            WallZ(root, "WallLivingNorth", 6.2f, 5.2f, 13.6f, h, palette.LiveWall, 0f, half);
            WallZ(root, "WallBathSouthL", 6.2f, 0f, 1.2f, h, palette.BathWall, half, 0f);
            WallZ(root, "WallBathSouthR", 6.2f, 2.2f, 5.2f, h, palette.BathWall, 0f, 0f);
            WallZ(root, "WallBathNorth", 10.4f, 0f, 4.4f, h, palette.BathWall, half, half);

            WallX(root, "WallWest", 0f, 0f, 10.4f, h, palette.BedWall, half, half);
            WallX(root, "WallEast", 13.6f, 0f, 6.2f, h, palette.LiveWall, half, half);
            WallX(root, "WallBathEast", 4.4f, 6.2f, 10.4f, h, palette.BathWall, half, half);
            WallX(root, "WallPartitionS", 5.2f, 0f, 3.2f, h, palette.LiveWall, half, 0f);
            WallX(root, "WallPartitionN", 5.2f, 4.3f, 6.2f, h, palette.LiveWall, 0f, half);

            // Vanos interiores: marco hasta el techo, paso de un metro. No son paredes faltantes.
            Box("BathDoorJambL", root, new Vector3(1.2f, h * 0.5f, 6.2f), new Vector3(0.1f, h, 0.22f), palette.BathWall);
            Box("BathDoorJambR", root, new Vector3(2.2f, h * 0.5f, 6.2f), new Vector3(0.1f, h, 0.22f), palette.BathWall);
            Box("BathDoorHeader", root, new Vector3(1.7f, 2.4f, 6.2f), new Vector3(1.16f, 0.68f, 0.22f), palette.BathWall);
            Box("PassageJambS", root, new Vector3(5.2f, h * 0.5f, 3.2f), new Vector3(0.22f, h, 0.1f), palette.LiveWall);
            Box("PassageJambN", root, new Vector3(5.2f, h * 0.5f, 4.3f), new Vector3(0.22f, h, 0.1f), palette.LiveWall);
            Box("PassageHeader", root, new Vector3(5.2f, 2.4f, 3.75f), new Vector3(0.22f, 0.68f, 1.26f), palette.LiveWall);

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
            Box("DoorJambL", root, new Vector3(10.42f, h * 0.5f, 0f), new Vector3(0.16f, h, 0.22f), palette.WoodDark);
            Box("DoorJambR", root, new Vector3(11.58f, h * 0.5f, 0f), new Vector3(0.16f, h, 0.22f), palette.WoodDark);
            Box("DoorHeader", root, new Vector3(11f, 2.4f, 0f), new Vector3(1.42f, 0.68f, 0.22f), palette.WoodDark);

            var door = Prop("FrontDoor", root, new Vector3(11f, 0f, 0f));
            Piece("Leaf", door.transform, new Vector3(0f, 1.05f, 0f), new Vector3(1.08f, 2.08f, 0.055f), palette.Wood);
            Piece("PanelTop", door.transform, new Vector3(0f, 1.48f, 0.03f), new Vector3(0.72f, 0.62f, 0.018f), palette.WoodDark);
            Piece("PanelBottom", door.transform, new Vector3(0f, 0.62f, 0.03f), new Vector3(0.72f, 0.72f, 0.018f), palette.WoodDark);
            Piece("Handle", door.transform, new Vector3(0.4f, 1.02f, 0.05f), new Vector3(0.04f, 0.28f, 0.04f), palette.Metal);
            SetCollider(door, new Vector3(0f, 1.06f, 0f), new Vector3(1.28f, 2.14f, 0.22f));
            built.DoorVisual = door.transform;
            built.Door = MakeInteractable(door, WorldAnchorIds.Door, "Mirar la puerta",
                "Nadie. Solo la madera y tu alarma.");

            var lamp = Prop("CeilingLamp", root, new Vector3(6.4f, 2.7f, 4.2f));
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

        static BuiltHouse BuildFromModels(Transform parent, Palette palette, ModelLibrary models)
        {
            var built = new BuiltHouse();
            var root = new GameObject("Apartment").transform;
            root.SetParent(parent, false);
            built.Root = root;

            var placed = new List<GameObject>();
            var factor = 1f;

            GameObject Spawn(GameObject prefab, string name)
            {
                if (prefab == null)
                    return null;

                var go = UnityEngine.Object.Instantiate(prefab, root, false);
                go.name = name;
                go.transform.localScale = Vector3.one * factor;
                placed.Add(go);
                return go;
            }

            var paredes = Spawn(models.Paredes, "Paredes");
            factor = ShellScale(paredes);
            if (!Mathf.Approximately(factor, 1f))
            {
                foreach (var go in placed)
                    go.transform.localScale = Vector3.one * factor;
            }

            var cama = Spawn(models.Cama, "Bed");
            var mesaDeLuz = Spawn(models.MesaDeLuz, "Nightstand");
            var celular = Spawn(models.Celular, "Phone");
            var ventana = Spawn(models.Ventana, "Window");
            var espejo = Spawn(models.Espejo, "Mirror");
            var lavamanos = Spawn(models.Lavamanos, "Sink");
            var banera = Spawn(models.Banera, "Bathtub");
            var inodoro = Spawn(models.Inodoro, "Toilet");
            var comoda = Spawn(models.Comoda, "Dresser");
            var sillon = Spawn(models.Sillon, "Couch");
            var mesa = Spawn(models.Mesa, "Table");
            var notebook = Spawn(models.Notebook, "Laptop");
            var cuaderno = Spawn(models.Cuaderno, "Journal");
            var mesada = Spawn(models.Mesada, "Counter");
            var cafetera = Spawn(models.Cafetera, "Coffee");
            var taza = Spawn(models.Taza, "Cup");
            var heladera = Spawn(models.Heladera, "Fridge");
            var puerta = Spawn(models.Puerta, "FrontDoor");
            Spawn(models.Alfombra, "Rug");

            AlignToHouse(placed, paredes);

            if (paredes != null)
                CollideShell(paredes);

            // El FBX trae piso y muros, no el techo. El cielo no se ve desde la calle.
            Box("Ceiling", root, new Vector3(6.9f, 2.76f, 3.2f), new Vector3(14.2f, 0.18f, 6.8f), palette.Ceiling);
            Box("CeilingBath", root, new Vector3(2.2f, 2.76f, 8.3f), new Vector3(4.8f, 0.18f, 4.6f), palette.Ceiling);

            built.Bed = Bind(cama, WorldAnchorIds.Bed, "Acostarte / cerrar el día",
                "El colchón espera. Todavía no es de noche.", 0f);
            Block(mesaDeLuz);
            built.PhoneVisual = celular != null ? celular.transform : null;
            built.Phone = Bind(celular, WorldAnchorIds.Phone, "Mirar el celular",
                "La pantalla está quieta. Por ahora.", 0.34f);
            built.Window = Bind(ventana, WorldAnchorIds.Window, "Mirar por la ventana",
                "Afuera el día ya empezó sin vos. Autos, alguien riendo, un perro. El mundo no espera.", 0.2f);
            built.Mirror = Bind(espejo, WorldAnchorIds.Mirror, "Mirarte en el espejo",
                "Por un segundo no sabés bien quién sos hoy.", 0.2f);
            built.Sink = Bind(lavamanos, WorldAnchorIds.Sink, "Agua fría (TIPP)",
                "Agua fría en las muñecas. El cuerpo puede bajar un cambio.", 0.15f);
            Block(banera);
            Block(inodoro);
            Block(comoda);
            built.Couch = Bind(sillon, WorldAnchorIds.Couch, "Sentarte un rato",
                "El sillón invita a desaparecer un rato. A veces ayuda. A veces es un pozo.", 0f);
            Block(mesa);
            built.Laptop = Bind(notebook, WorldAnchorIds.Laptop, "Abrir el notebook",
                "La tapa cerrada. El trabajo puede esperar un poco.", 0.28f);
            Block(cuaderno);
            Block(mesada);
            built.Coffee = Bind(cafetera, WorldAnchorIds.Coffee, "Hacer café",
                "El café ayuda un poco. No arregla la marea, pero calienta las manos.", 0.2f);
            Block(taza);
            built.Fridge = Bind(heladera, WorldAnchorIds.Fridge, "Abrir la heladera",
                "Luz, un resto de comida, nada que arregle el día. Igual es algo concreto.", 0f);
            built.DoorVisual = puerta != null ? puerta.transform : null;
            built.Door = Bind(puerta, WorldAnchorIds.Door, "Mirar la puerta",
                "Nadie. Solo la madera y tu alarma.", 0.12f);

            var lamp = Prop("CeilingLamp", root, new Vector3(6.4f, 2.58f, 4.2f));
            Piece("Cord", lamp.transform, new Vector3(0f, -0.12f, 0f), new Vector3(0.02f, 0.24f, 0.02f), palette.Dark);
            Piece("Shade", lamp.transform, new Vector3(0f, -0.28f, 0f), new Vector3(0.42f, 0.14f, 0.42f), palette.Linen);

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

            LogModel("Paredes", paredes);
            LogModel("Cama", cama);
            LogModel("Puerta", puerta);
            LogModel("Celular", celular);

            var bedCenter = CenterOf(cama);
            var doorCenter = CenterOf(puerta);
            if (bedCenter.x < 0f || bedCenter.x > 5f || bedCenter.z < 0f || bedCenter.z > 4f)
                throw new InvalidOperationException($"La cama quedó en {bedCenter}, fuera del dormitorio.");
            if (doorCenter.x < 8f || doorCenter.x > 13.5f || doorCenter.z < -0.6f || doorCenter.z > 2f)
                throw new InvalidOperationException($"La puerta quedó en {doorCenter}, fuera del vano sur.");

            return built;
        }

        static float ShellScale(GameObject paredes)
        {
            if (!TryBounds(paredes, out var bounds))
                return 1f;

            var max = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (max > 80f)
                return 0.01f;
            if (max < 1.5f)
                return 100f;
            return 1f;
        }

        static void AlignToHouse(List<GameObject> placed, GameObject paredes)
        {
            if (!TryBounds(paredes, out var bounds))
                return;

            var flipX = bounds.center.x < 0f;
            var flipZ = bounds.center.z < 0f;
            if (!flipX && !flipZ)
                return;

            foreach (var go in placed)
                Reflect(go, flipX, flipZ);

            if (TryBounds(paredes, out bounds) && bounds.center.x >= 0f && bounds.center.z >= 0f)
            {
                Debug.Log("Apartment FBX: se acomodó el eje para coincidir con el departamento.");
                return;
            }

            foreach (var go in placed)
                Reflect(go, flipX, flipZ);

            foreach (var go in placed)
            {
                var scale = go.transform.localScale;
                if (flipX) scale.x = -Mathf.Abs(scale.x);
                if (flipZ) scale.z = -Mathf.Abs(scale.z);
                go.transform.localScale = scale;
                MakeDoubleSided(go);
            }

            Debug.LogWarning("Apartment FBX: el modelado entró espejado y se corrigió con la escala.");
        }

        static void Reflect(GameObject go, bool flipX, bool flipZ)
        {
            if (go.transform.childCount == 0)
            {
                ReflectTransform(go.transform, flipX, flipZ);
                return;
            }

            foreach (Transform child in go.transform)
                ReflectTransform(child, flipX, flipZ);
        }

        static void ReflectTransform(Transform target, bool flipX, bool flipZ)
        {
            var position = target.localPosition;
            if (flipX) position.x = -position.x;
            if (flipZ) position.z = -position.z;
            target.localPosition = position;

            var rotation = target.localRotation;
            if (flipX && flipZ)
                target.localRotation = new Quaternion(-rotation.x, rotation.y, -rotation.z, rotation.w);
            else if (flipZ)
                target.localRotation = new Quaternion(rotation.x, -rotation.y, -rotation.z, rotation.w);
            else if (flipX)
                target.localRotation = new Quaternion(-rotation.x, rotation.y, -rotation.z, rotation.w);
        }

        static void MakeDoubleSided(GameObject go)
        {
            foreach (var renderer in go.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.HasProperty("_Cull"))
                        material.SetFloat("_Cull", 0f);
                }
            }
        }

        static void CollideShell(GameObject paredes)
        {
            foreach (var filter in paredes.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh == null)
                    continue;

                var size = TransformBounds(filter.transform.localToWorldMatrix, filter.sharedMesh.bounds).size;
                var flatFloor = size.y < 0.25f && size.y < size.x && size.y < size.z;
                if (flatFloor)
                {
                    FitBox(filter.gameObject, 0f);
                    continue;
                }

                var mesh = filter.gameObject.GetComponent<MeshCollider>();
                if (mesh == null)
                    mesh = filter.gameObject.AddComponent<MeshCollider>();
                mesh.sharedMesh = filter.sharedMesh;
                mesh.convex = false;
            }
        }

        static Interactable Bind(GameObject go, string id, string prompt, string thought, float minWorld)
        {
            if (go == null)
                return null;

            FitBox(go, minWorld);
            return MakeInteractable(go, id, prompt, thought);
        }

        static void Block(GameObject go)
        {
            if (go == null)
                return;

            FitBox(go, 0f);
        }

        static void FitBox(GameObject go, float minWorld)
        {
            if (!TryBounds(go, out var world))
                return;

            var box = go.GetComponent<BoxCollider>();
            if (box == null)
                box = go.AddComponent<BoxCollider>();

            box.center = go.transform.InverseTransformPoint(world.center);
            var lossy = go.transform.lossyScale;
            box.size = new Vector3(
                AxisSize(world.size.x, lossy.x, minWorld),
                AxisSize(world.size.y, lossy.y, minWorld),
                AxisSize(world.size.z, lossy.z, minWorld));
        }

        static float AxisSize(float worldSize, float scale, float minWorld)
        {
            var magnitude = Mathf.Max(0.0001f, Mathf.Abs(scale));
            return Mathf.Max(worldSize, minWorld) / magnitude;
        }

        static bool TryBounds(GameObject go, out Bounds bounds)
        {
            bounds = default;
            if (go == null)
                return false;

            var any = false;
            foreach (var filter in go.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh == null)
                    continue;

                var world = TransformBounds(filter.transform.localToWorldMatrix, filter.sharedMesh.bounds);
                if (!any)
                {
                    bounds = world;
                    any = true;
                }
                else
                {
                    bounds.Encapsulate(world);
                }
            }

            return any;
        }

        static Bounds TransformBounds(Matrix4x4 matrix, Bounds local)
        {
            var center = matrix.MultiplyPoint3x4(local.center);
            var extents = local.extents;
            var axisX = matrix.MultiplyVector(new Vector3(extents.x, 0f, 0f));
            var axisY = matrix.MultiplyVector(new Vector3(0f, extents.y, 0f));
            var axisZ = matrix.MultiplyVector(new Vector3(0f, 0f, extents.z));
            var worldExtents = new Vector3(
                Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
                Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
                Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z));
            return new Bounds(center, worldExtents * 2f);
        }

        static Vector3 CenterOf(GameObject go)
        {
            return TryBounds(go, out var bounds) ? bounds.center : Vector3.zero;
        }

        static void LogModel(string label, GameObject go)
        {
            if (go == null)
            {
                Debug.LogWarning("Apartment FBX: falta " + label);
                return;
            }

            var size = TryBounds(go, out var bounds) ? bounds.size : Vector3.zero;
            Debug.Log($"Apartment FBX {label} centro={CenterOf(go):F2} tamaño={size:F2} root={go.transform.localPosition:F2} hijos={go.transform.childCount}");
        }

        static void Leg(Transform parent, Vector3 pos, Material mat)
        {
            Piece("Leg", parent, pos, new Vector3(0.08f, 0.72f, 0.08f), mat);
        }

        static void WallX(Transform parent, string name, float x, float z0, float z1, float height, Material mat, float pad0, float pad1)
        {
            float a = z0 - pad0;
            float b = z1 + pad1;
            float len = b - a;
            if (len < 0.05f) return;
            Box(name, parent, new Vector3(x, height * 0.5f, (a + b) * 0.5f), new Vector3(0.16f, height, len), mat);
        }

        static void WallZ(Transform parent, string name, float z, float x0, float x1, float height, Material mat, float pad0, float pad1)
        {
            float a = x0 - pad0;
            float b = x1 + pad1;
            float len = b - a;
            if (len < 0.05f) return;
            Box(name, parent, new Vector3((a + b) * 0.5f, height * 0.5f, z), new Vector3(len, height, 0.16f), mat);
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
