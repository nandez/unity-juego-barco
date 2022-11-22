using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Camara y Rayo de la camara")]
    [SerializeField] Camera mainCamera; //  Camara Principal
    [SerializeField] Ray ray;           //  rayo camara
    [SerializeField] Vector3 PuntoHit, sentido;  //  punto donde se hizo click
    [SerializeField] bool Move = false;
    [SerializeField] bool CanMove;
    [SerializeField] LayerMask Mask;
    [SerializeField] float velocidad, sqrdistanceToMove, sqrMinDistace, speedturn, distanciacam;

    RaycastHit Hit;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        ray = mainCamera.ScreenPointToRay(Input.mousePosition);               // defino ray como el rayo que sale de la camara
        if (Physics.Raycast(ray, out Hit, 400, Mask))   // si apretan click mientras que rayo esta tocando
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                PuntoHit = Hit.point;                                            //guardo el punto que choco en una variable llamada PuntoHit
                Move = true;                                                     // el jugador podra moverse
            }
        }

        PuntoHit.y = transform.position.y;                                     //  anula y actualiza cualquier seleccion en el eje "y"
        sqrdistanceToMove = (PuntoHit - transform.position).sqrMagnitude;      //  distancia entre punto selecionado y jugador al cuadrado para mejorar rendimiento y evitar calculo de distancias con raices
        sentido = PuntoHit - transform.position;                               //  defino el sentido restando la posicion del jugador al punto seleccionado en la pantalla

        if (sqrdistanceToMove > sqrMinDistace && Move)                         //  solo se mueve si la distancia es mayor que la minima para que se mueva Y no hay nada que inpida su movimiento
        {
            gameObject.transform.position = transform.position + (transform.forward * velocidad * Time.deltaTime);
        }
        else // si la distancia es muy chica no se movera mas
        {
            Move = false;
        }

        if (Move)
        {

            Quaternion rotation = Quaternion.LookRotation(sentido); // crea una variable rotation y le asigna la rotacion de Sentido
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speedturn * Time.deltaTime); // transforma la rotacion del jugador suavemente
        }

        mainCamera.transform.position = transform.position + new Vector3(0, distanciacam, distanciacam * -1);
    }
}
