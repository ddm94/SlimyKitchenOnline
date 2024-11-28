using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer coreMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private ParticleSystem movingParticles;

    private Material coreMaterial;
    private Material bodyMaterial;
    private Material movingParticlesMaterial;
    private float emissiveIntensity = 15f;

    private void Awake()
    {
        coreMaterial = new Material(coreMeshRenderer.material);
        coreMeshRenderer.material = coreMaterial;

        bodyMaterial = new Material(bodyMeshRenderer.material);
        bodyMeshRenderer.material = bodyMaterial;

        movingParticlesMaterial = new Material(movingParticles.GetComponent<Renderer>().material);
        movingParticles.GetComponent<Renderer>().material = movingParticlesMaterial;
    }

    public void SetPlayerColor(Color color)
    {
        coreMaterial.SetColor("_EmissionColor", color * emissiveIntensity);
        bodyMaterial.color = color;
        movingParticlesMaterial.color = color;
    }
}
