using UnityEngine;

namespace SeriousGameBPD.World
{
    /// <summary>
    /// Referencias del departamento que ya vive en la escena.
    /// Mové los hijos en la Hierarchy; el juego lee estos enlaces al arrancar.
    /// </summary>
    public class HouseLayout : MonoBehaviour
    {
        static readonly Vector3 FallbackSpawn = new Vector3(2.6f, 1.05f, 3.8f);
        const float FallbackYaw = 90f;

        [Header("Aparición")]
        public Transform SpawnPoint;

        [Header("Luces")]
        public Light Sun;

        [Header("Anclas")]
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

        public Vector3 SpawnPosition => SpawnPoint != null ? SpawnPoint.position : FallbackSpawn;
        public float SpawnYaw => SpawnPoint != null ? SpawnPoint.eulerAngles.y : FallbackYaw;

        void OnDrawGizmos()
        {
            if (SpawnPoint == null) return;
            Gizmos.color = new Color(0.35f, 0.9f, 0.55f, 0.95f);
            Gizmos.DrawWireSphere(SpawnPoint.position, 0.28f);
            Gizmos.DrawRay(SpawnPoint.position, SpawnPoint.forward * 0.8f);
        }
    }
}
