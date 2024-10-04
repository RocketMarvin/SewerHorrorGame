using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    private PostProcessVolume postProcessVolume;
    private ChromaticAberration chromaticAberration;

    [SerializeField] private float chromaticIncreaseSpeed = 5f;
    [SerializeField] private float chromaticDecreaseSpeed = 1f;
    [SerializeField] private float maxChromaticAberration = 1f;

    private void Start()
    {
        // Zoek de PostProcessVolume in de scene
        postProcessVolume = FindObjectOfType<PostProcessVolume>();

        // Controleer of ChromaticAberration aanwezig is in de PostProcessVolume
        if (postProcessVolume.profile.TryGetSettings(out chromaticAberration))
        {
            chromaticAberration.intensity.value = 0f; // Zet standaard op 0
        }
    }

    private void Update()
    {
        HandleChromaticAberration();
    }

    private void HandleChromaticAberration()
    {
        if (chromaticAberration == null) return;

        // Als de vijand rent, verhoog de chromatische aberratie snel
        if (Enemy.isRunning)
        {
            chromaticAberration.intensity.value = Mathf.MoveTowards(chromaticAberration.intensity.value, maxChromaticAberration, chromaticIncreaseSpeed * Time.deltaTime);
        }
        // Als de vijand stopt met rennen, verminder de chromatische aberratie langzaam
        else
        {
            chromaticAberration.intensity.value = Mathf.MoveTowards(chromaticAberration.intensity.value, 0f, chromaticDecreaseSpeed * Time.deltaTime);
        }
    }
}
