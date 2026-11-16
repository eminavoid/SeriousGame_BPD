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
            var card = MenuChrome.Card(parent, "StartScreen", 680f, 540f); 
            MenuChrome.Title(card, "Inicio");
            MenuChrome.Gap(card, 24f);
            
            // Botón Empezar
            MenuChrome.Pill(card, "Empezar", onStart, true);
            MenuChrome.Gap(card, 16f);

            // Botón Salir (primary: true para mantener negrita y tamaño idéntico)
            MenuChrome.Pill(card, "Salir", QuitGame, true);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}