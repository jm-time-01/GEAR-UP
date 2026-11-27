
using System.Collections.Generic;
using UnityEngine;

public class GPSWaypoint : MonoBehaviour
{
    [Header("Pontos conectados")]
    public List<GPSWaypoint> conexoes = new List<GPSWaypoint>();

    [Header("Visualização")]
    public Color corPonto = Color.cyan;
    public Color corConexao = Color.yellow;

    public float tamanhoPonto = 0.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = corPonto;
        Gizmos.DrawSphere(transform.position, tamanhoPonto);

        Gizmos.color = corConexao;

        foreach (GPSWaypoint waypoint in conexoes)
        {
            if (waypoint != null)
            {
                Gizmos.DrawLine(
                    transform.position,
                    waypoint.transform.position
                );
            }
        }
    }
}
