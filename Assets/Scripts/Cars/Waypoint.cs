using UnityEngine;

public class Waypoint : MonoBehaviour
{



    public float velocidadeRecomendada;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}