using UnityEngine;
using UnityEngine.UI;

namespace SeriousGameBPD.Presentation
{
    /// <summary>
    /// Sprites del menú y de los pensamientos. Viven en Resources/UI.
    /// </summary>
    public static class UiArt
    {
        public static readonly Color Ink = new Color(0.29f, 0.16f, 0.08f, 1f);
        public static readonly Color Cream = new Color(0.96f, 0.91f, 0.82f, 1f);
        public static readonly Color Plate = new Color(0.78f, 0.61f, 0.43f, 1f);
        public static readonly Color ButtonBrown = new Color(0.46f, 0.30f, 0.14f, 1f);

        public static Sprite ThoughtPhoto => Load("UI/ThoughtPhoto");
        public static Sprite ThoughtPlate => Load("UI/ThoughtPlate");
        public static Sprite PausePanel => Load("UI/PausePanel");
        public static Sprite ButtonFace => Load("UI/UiButton");

        public static void Apply(Image image, Sprite sprite, bool sliced)
        {
            if (image == null) return;
            image.sprite = sprite;
            image.type = sprite != null && sliced ? Image.Type.Sliced : Image.Type.Simple;
            image.color = Color.white;
            image.preserveAspect = false;
            if (sprite == null)
                image.color = sliced ? Plate : ButtonBrown;
        }

        static Sprite Load(string path)
        {
            var sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
                return sprite;

            var tex = Resources.Load<Texture2D>(path);
            if (tex == null)
                return null;

            return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
