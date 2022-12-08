using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    [Header("Otros")]

    [Space]

    [SerializeField] GameObject PosRayoR;
    [SerializeField] GameObject PosRayoL, SentidoRayoOBJ;

    // Privados [[[]]]
    private Vector3 mirandoA,PosR,PosL;

    [Space]
    [Space]

    [Header("Camara")]

    [Space]

    [SerializeField] Camera mainCamera; //  Camara Principal
    [SerializeField] float distanciacam;
    [SerializeField] LayerMask WaterMask;

    // Privados [[[]]]
    private Ray ray;                    //  rayo camara
    private Vector3 PuntoHit;  //  punto donde se hizo click
    private bool Move = false;

    [Space]
    [Space]

    [Header("Movimiento")]

    [Space]

    [SerializeField] float limiteVelocidad;
    [SerializeField] float speedturn, sqrMinDistace;
    [SerializeField] LayerMask IslandMask;

    // Privados[[[]]]
    private float velocidad, sqrdistanceToMove,limiteVelocidadInicial,realSpeedTurn;
    private Vector3 destino, sentido;
    [SerializeField]bool RRay,LRay;
    RaycastHit Hit,RHit,LHit;

    [Space]
    [Space]
    [Header("Ataque")]
    [Space]

    [SerializeField] protected float attackRange = 10f; // Indica el rango de disparo
    [SerializeField] protected bool drawAttackRangeGizmo = false; // Indica si dibujar el gizmo de rango de ataque.
    [SerializeField] protected CannonController cannonCtrl; // Controlador de cañon
    [SerializeField] protected List<CannonBall> cannonBallPrefabs; // Prefabs de balas de cañon
    private CannonBall currentCannonBall; // Bala de cañon actual para disparar

    void Start()
    {
        limiteVelocidadInicial = limiteVelocidad;

        // Inicializamos el prefab del proyectil que se usará para disparar..
        if (cannonBallPrefabs?.Count > 0)
            currentCannonBall = cannonBallPrefabs[0];
    }

    void Update()
    {
        Desaceleracion();
        Mouse3D();
        Sentido();
        Movimiento();
        Rotacion();
        CameraControl();
    }
    public void Desaceleracion()
    {
        if (velocidad < limiteVelocidadInicial)
        {
            limiteVelocidad = velocidad + 0.03f * Time.timeScale;
        }

        velocidad = (sentido.magnitude * 2);
        if (velocidad > limiteVelocidad)
        {
            velocidad = limiteVelocidad;
        }

    }

    public void Mouse3D()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);               // defino ray como el rayo que sale de la camara
        if (Physics.Raycast(ray, out Hit, 400, WaterMask))   // si apretan click mientras que rayo esta tocando
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                PuntoHit = Hit.point;                                            //guardo el punto que choco en una variable llamada PuntoHit
                Move = true;                                                     // el jugador podra moverse
            }
            else if(Input.GetKey(KeyCode.Mouse1))
            {
                // Cuando el jugador hace click derecho, verificamos si el controlador de cañon
                // puede disparar, y si la distancia al punto seleccionado está dentro del rango de ataque.
                // Si es asi, le pasamos el punto donde hizo click y la el prefab de la bala de cañon actual..
                if(cannonCtrl.CanFire() && Vector3.Distance(transform.position, Hit.point) <= attackRange)
                    cannonCtrl.SetTarget(currentCannonBall, Hit.point, gameObject);
            }
        }
        PuntoHit.y = transform.position.y;
    }

    public void Sentido()
    {
        sqrdistanceToMove = (PuntoHit - transform.position).sqrMagnitude;      //  distancia entre punto selecionado y jugador al cuadrado para mejorar rendimiento y evitar calculo de distancias con raices
        sentido = PuntoHit - transform.position;                               //  defino el sentido restando la posicion del jugador al punto seleccionado en la pantalla
        Debug.DrawRay(transform.position, sentido);

        mirandoA = SentidoRayoOBJ.transform.position - transform.position ;
    }

    public void Movimiento()
    {
        if (sqrdistanceToMove > sqrMinDistace && Move)                         //  solo se mueve si la distancia es mayor que la minima para que se mueva Y no hay nada que inpida su movimiento
        {
            gameObject.transform.position = transform.position + (transform.forward * velocidad * Time.deltaTime);
        }
        else // si la distancia es muy chica no se movera mas
        {
            Move = false;
            limiteVelocidad = 0;
        }
    }

    public void Rotacion()
    {
        Debug.DrawRay(transform.position, destino, Color.red, 0);
        Debug.DrawRay(PosRayoR.transform.position - mirandoA.normalized, (mirandoA.normalized + (PosRayoR.transform.position - transform.position)/2) * 7);
        if (Physics.Raycast(PosRayoR.transform.position - mirandoA.normalized, mirandoA.normalized + (PosRayoR.transform.position - transform.position)/2, out RHit, 7, IslandMask))
        {
            PosR = RHit.point;
            RRay = true;
        }
        else
        {
            RRay = false;
        }
        Debug.DrawRay(PosRayoL.transform.position - mirandoA.normalized, (mirandoA.normalized + (PosRayoL.transform.position - transform.position)/2) * 7);
        if (Physics.Raycast(PosRayoL.transform.position - mirandoA.normalized, mirandoA.normalized + (PosRayoL.transform.position - transform.position)/2, out LHit, 7, IslandMask))
        {
            PosL = LHit.point;
            LRay = true;
        }
        else
        {
            LRay = false;
        }


        if(RRay && LRay)
        {
            if ((PosR - transform.position).magnitude < (PosL - transform.position).magnitude)
            {
                realSpeedTurn = speedturn / 2;
                destino = PosR.normalized + (PosRayoL.transform.position - transform.position).normalized * 3;
            }
            if ((PosR - transform.position).magnitude > (PosL - transform.position).magnitude)
            {
                destino = PosL.normalized + (PosRayoR.transform.position - transform.position).normalized * 3;
                realSpeedTurn = speedturn / 2;
            }
        }
        else
        {
            if (RRay && !LRay)
            {
                destino = PosR.normalized + (PosRayoL.transform.position - transform.position).normalized * 3;
                realSpeedTurn = speedturn / 2;
            }
            if (!RRay && LRay)
            {
                destino = PosL.normalized + (PosRayoR.transform.position - transform.position).normalized * 3;
                realSpeedTurn = speedturn / 2;
            }
        }


        if (Move)
        {
            if(!RRay && !LRay)
            {
                realSpeedTurn = speedturn;
                destino = sentido;
            }
            Quaternion rotation = Quaternion.LookRotation(destino); // crea una variable rotation y le asigna la rotacion de Sentido
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, realSpeedTurn * Time.deltaTime); // transforma la rotacion del jugador suavemente
        }
    }

    public void CameraControl()
    {
        mainCamera.transform.position = transform.position + new Vector3(0, distanciacam, distanciacam * -1);
    }

    private void OnDrawGizmos()
    {
        // Dibujamos el rango de ataque..
        if(drawAttackRangeGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
