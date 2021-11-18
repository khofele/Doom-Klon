using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 80f;
    [SerializeField] private Camera cam = null;
    [SerializeField] private ParticleSystem particleSys = null;

    private LineRenderer lineRenderer = null;


    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ShootGun();
        }
    }

    private void ShootGun()
    {
        particleSys.Play();

        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Target target = hit.collider.gameObject.GetComponent<Target>();
            Debug.Log(hit.transform.name);

            if (target != null)
            {
                Debug.Log(target.transform.name);
                target.TakeDamage(damage);
            }
        }
    }
}
