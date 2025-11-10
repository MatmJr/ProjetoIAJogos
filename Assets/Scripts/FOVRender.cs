using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EnemyFOVRenderer : MonoBehaviour
{
    [SerializeField] private float visionRange = 5f;   // distância máxima
    [SerializeField] private float visionAngle = 60f;  // ângulo do FOV
    [SerializeField] private int segments = 30;        // mais segmentos = cone mais suave
    [SerializeField] private Color fovColor = new Color(1f, 1f, 0f, 0.3f); // cor amarela semitransparente

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 2; // ponto central + bordas do cone
        lineRenderer.loop = true;                 // fecha a forma
        lineRenderer.useWorldSpace = false;       // desenhar relativo ao inimigo
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        // aplica cor única
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = fovColor;
        lineRenderer.endColor = fovColor;
    }

    void Update()
    {
        DrawFOV();
    }

    void DrawFOV()
    {
        float angleStep = visionAngle / segments;
        float startAngle = -visionAngle / 2;

        // ponto central (origem no inimigo)
        lineRenderer.SetPosition(0, Vector3.zero);

        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;
            lineRenderer.SetPosition(i + 1, dir * visionRange);
        }
    }
}
