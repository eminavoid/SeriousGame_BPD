using System;
using UnityEngine;

namespace SeriousGameBPD.Presentation
{
    /// <summary>
    /// Primera pantalla. Usa el mismo panel, fondo y botón pill que la pausa.
    /// </summary>
    public class StartMenu : MonoBehaviour
    {
        public void Build(Transform parent, Action onStart)
        {
            var card = MenuChrome.Card(parent, "StartScreen", 680f, 440f);
            MenuChrome.Title(card, "Inicio");
            MenuChrome.Gap(card, 36f);
            MenuChrome.Pill(card, "Empezar", onStart, true);
        }
    }
}
