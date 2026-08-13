using SeriousGameBPD.Data;
using UnityEngine;

namespace SeriousGameBPD.World
{
    public static class HouseBuilder
    {
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
            public Transform PhoneVisual;
            public Transform DoorVisual;
            public Light Sun;
        }

        public static BuiltHouse Build(Transform parent)
        {
            var built = new BuiltHouse();
            var root = new GameObject("Apartment").transform;
            root.SetParent(parent, false);
            built.Root = root;

            var floor = Mat(new Color(0.38f, 0.30f, 0.24f));
            var ceil = Mat(new Color(0.82f, 0.81f, 0.78f));
            var bedWall = Mat(new Color(0.58f, 0.44f, 0.42f));
            var liveWall = Mat(new Color(0.34f, 0.42f, 0.44f));
            var bathWall = Mat(new Color(0.48f, 0.54f, 0.58f));
            var wood = Mat(new Color(0.42f, 0.28f, 0.18f));
            var linen = Mat(new Color(0.62f, 0.55f, 0.48f));
            var metal = Mat(new Color(0.55f, 0.58f, 0.62f));
            var dark = Mat(new Color(0.12f, 0.13f, 0.15f));
            var glass = Mat(new Color(0.45f, 0.62f, 0.72f));
            var phoneMat = Mat(new Color(0.08f, 0.09f, 0.12f));

            const float h = 2.65f;
            Box("Floor", root, new Vector3(6.8f, -0.5f, 5.2f), new Vector3(14.2f, 1.0f, 11.0f), floor);
            Box("Ceiling", root, new Vector3(6.8f, h + 0.08f, 5.2f), new Vector3(13.8f, 0.16f, 10.6f), ceil);

            WallZ(root, 0f, 0f, 10.35f, h, bedWall);
            WallZ(root, 0f, 11.65f, 13.6f, h, liveWall);
            WallZ(root, 6.5f, 5.2f, 13.6f, h, liveWall);
            WallZ(root, 6.2f, 0f, 1.0f, h, bathWall);
            WallZ(root, 6.2f, 2.75f, 5.2f, h, bathWall);
            WallZ(root, 10.4f, 0f, 4.4f, h, bathWall);

            WallX(root, 0f, 0f, 10.4f, h, bedWall);
            WallX(root, 13.6f, 0f, 6.5f, h, liveWall);
            WallX(root, 4.4f, 6.2f, 10.4f, h, bathWall);
            WallX(root, 5.2f, 0f, 2.0f, h, liveWall);
            WallX(root, 5.2f, 4.25f, 6.2f, h, liveWall);


            var bed = Box("Bed", root, new Vector3(1.55f, 0.32f, 1.45f), new Vector3(2.05f, 0.64f, 1.75f), linen);
            built.Bed = MakeInteractable(bed, WorldAnchorIds.Bed, "Acostarte / cerrar el día",
                "El colchón espera. Todavía no es de noche.");

            Box("Nightstand", root, new Vector3(2.85f, 0.28f, 0.52f), new Vector3(0.5f, 0.56f, 0.5f), wood);
            built.PhoneVisual = Box("Phone", root, new Vector3(2.85f, 0.62f, 0.52f), new Vector3(0.09f, 0.03f, 0.18f), phoneMat).transform;
            FatCollider(built.PhoneVisual.gameObject, 0.4f);
            built.Phone = MakeInteractable(built.PhoneVisual.gameObject, WorldAnchorIds.Phone, "Mirar el celular",
                "La pantalla está quieta. Por ahora.");

            var window = Box("Window", root, new Vector3(0.12f, 1.45f, 3.3f), new Vector3(0.08f, 1.05f, 1.3f), glass);
            MakeInteractable(window, WorldAnchorIds.Window, "Mirar por la ventana",
                "Afuera el día ya empezó sin vos. Autos, alguien riendo, un perro. El mundo no espera.");

            var mirror = Box("Mirror", root, new Vector3(3.85f, 1.55f, 8.35f), new Vector3(0.06f, 0.95f, 0.7f), glass);
            FatCollider(mirror, 0.45f);
            built.Mirror = MakeInteractable(mirror, WorldAnchorIds.Mirror, "Mirarte en el espejo",
                "Por un segundo no sabés bien quién sos hoy.");

            var sink = Box("Sink", root, new Vector3(3.25f, 0.45f, 8.35f), new Vector3(0.7f, 0.9f, 0.5f), metal);
            built.Sink = MakeInteractable(sink, WorldAnchorIds.Sink, "Agua fría (TIPP)",
                "Agua fría en las muñecas. El cuerpo puede bajar un cambio.");

            var couch = Box("Couch", root, new Vector3(7.15f, 0.38f, 4.7f), new Vector3(1.9f, 0.76f, 0.85f), new Color(0.28f, 0.32f, 0.36f));
            MakeInteractable(couch, WorldAnchorIds.Couch, "Sentarte un rato",
                "El sillón invita a desaparecer un rato. A veces ayuda. A veces es un pozo.");

            Box("Table", root, new Vector3(9.15f, 0.38f, 2.95f), new Vector3(1.25f, 0.76f, 0.85f), wood);
            var laptop = Box("Laptop", root, new Vector3(9.15f, 0.82f, 2.95f), new Vector3(0.38f, 0.05f, 0.28f), dark);
            FatCollider(laptop, 0.5f);
            built.Laptop = MakeInteractable(laptop, WorldAnchorIds.Laptop, "Abrir el notebook",
                "La tapa cerrada. El trabajo puede esperar un poco.");

            var fridge = Box("Fridge", root, new Vector3(12.75f, 0.9f, 4.85f), new Vector3(0.7f, 1.8f, 0.7f), metal);
            MakeInteractable(fridge, WorldAnchorIds.Fridge, "Abrir la heladera",
                "Luz, un resto de comida, nada que arregle el día. Igual es algo concreto.");

            var coffee = Box("Coffee", root, new Vector3(12.7f, 1.05f, 3.55f), new Vector3(0.28f, 0.38f, 0.28f), dark);
            FatCollider(coffee, 0.4f);
            MakeInteractable(coffee, WorldAnchorIds.Coffee, "Hacer café",
                "El café ayuda un poco. No arregla la marea, pero calienta las manos.");

            built.DoorVisual = Box("FrontDoor", root, new Vector3(11.0f, 1.1f, 0.12f), new Vector3(1.08f, 2.2f, 0.1f), wood).transform;
            FatCollider(built.DoorVisual.gameObject, 0.35f);
            built.Door = MakeInteractable(built.DoorVisual.gameObject, WorldAnchorIds.Door, "Mirar la puerta",
                "Nadie. Solo la madera y tu alarma.");

            var sunGo = new GameObject("Sun");
            sunGo.transform.SetParent(root, false);
            sunGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.96f, 0.9f);
            sun.intensity = 1.55f;
            sun.shadows = LightShadows.Soft;
            built.Sun = sun;

            var fillGo = new GameObject("FillLight");
            fillGo.transform.SetParent(root, false);
            fillGo.transform.position = new Vector3(6f, 2.2f, 4f);
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
            go.transform.position = pos;
            go.transform.localScale = size;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null && mat != null)
                renderer.sharedMaterial = mat;
            return go;
        }

        static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, Color color)
        {
            return Box(name, parent, pos, size, Mat(color));
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
            box.size = Vector3.one + new Vector3(extra, extra, extra) / Mathf.Max(0.01f, go.transform.localScale.magnitude * 0.15f);
            box.size = new Vector3(
                1f + extra / Mathf.Max(0.05f, go.transform.localScale.x),
                1f + extra / Mathf.Max(0.05f, go.transform.localScale.y),
                1f + extra / Mathf.Max(0.05f, go.transform.localScale.z));
        }

        static Material Mat(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                         ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                         ?? Shader.Find("Universal Render Pipeline/Unlit")
                         ?? Shader.Find("Standard")
                         ?? Shader.Find("Unlit/Color")
                         ?? Shader.Find("Sprites/Default");
            if (shader == null)
                return null;

            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", 0.2f);
            return mat;
        }
    }
}
