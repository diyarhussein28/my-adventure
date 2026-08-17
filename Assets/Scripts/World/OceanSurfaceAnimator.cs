using UnityEngine;

namespace SeasOfLegends.World
{
    /// <summary>
    /// Provides lightweight ocean motion for the prototype water plane. It offsets the tiled
    /// material and gently displaces the visible surface while the gameplay collider remains stable.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public sealed class OceanSurfaceAnimator : MonoBehaviour
    {
        [SerializeField] private Vector2 textureScroll = new Vector2(0.006f, 0.012f);
        [SerializeField] private float waveAmplitude = 0.045f;
        [SerializeField] private float waveFrequency = 0.65f;

        private Material materialInstance;
        private Vector3 basePosition;

        private void Awake()
        {
            materialInstance = GetComponent<Renderer>().material;
            basePosition = transform.position;
        }

        private void Update()
        {
            if (materialInstance != null)
            {
                Vector2 offset = materialInstance.mainTextureOffset + textureScroll * Time.deltaTime;
                materialInstance.mainTextureOffset = new Vector2(offset.x % 1f, offset.y % 1f);
            }

            float swell = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
            transform.position = basePosition + Vector3.up * swell;
        }
    }
}
